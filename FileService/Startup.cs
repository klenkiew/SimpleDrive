using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cache;
using CommonEvents;
using EventBus;
using FileService.Commands;
using FileService.Commands.InvalidationKeysProviders;
using FileService.Configuration;
using FileService.Database;
using FileService.Infrastructure;
using FileService.Infrastructure.Middlewares;
using FileService.Model;
using FileService.Queries;
using FileService.Requests;
using FileService.Services;
using FileService.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Redis;
using Redis.Cache;
using Serialization;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using JsonSerializer = Serialization.JsonSerializer;

namespace FileService
{
    public class Startup
    {
        private readonly Container container = new Container();
        private ILogger Logger { get; }                         
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.ConfigureApplicationCookie(options => options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            });
            
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new ValidationFilter());
                    options.Filters.Add(new AuthorizeFilter());
                })
                .AddJsonOptions(options => 
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                )
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<ValidationMessage>());

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            services.AddDbContext<FileDbContext>(builder => builder.UseInMemoryDatabase("InMemoryDb")); 
            services.AddSingleton<JsonSerializer>();
            
            IntegrateSimpleInjector(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app);
            container.Verify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors("MyPolicy");
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMvc();

            container.GetInstance<IEventDispatcher>().SubscribeToEvents();
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));

            services.EnableSimpleInjectorCrossWiring(container);
            services.UseSimpleInjectorAspNetRequestScoping(container);
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            container.RegisterMvcControllers(app);

            // Add application services:
            container.Register(typeof(ICommandHandler<>), typeof(ICommandHandler<>).Assembly);
            container.Register(typeof(IInvalidationKeysProvider<>), typeof(IInvalidationKeysProvider<>).Assembly);
            container.RegisterDecorator(typeof(ICommandHandler<>),typeof(CacheInvalidationHandler<>), 
                context => ShouldCommandInvalidateCache(context.ServiceType));

            container.Register(typeof(IQueryHandler<,>), typeof(IQueryHandler<,>).Assembly);
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(LoggedQuery<,>));
            container.RegisterDecorator(
                typeof(IQueryHandler<,>),
                typeof(CachedQuery<,>),
                context => ShouldQueryHandlerBeCached(context.ServiceType));

            container.Register<ICurrentUser, CurrentUser>(Lifestyle.Singleton);
            container.Register<IFileStorage, LocalFileStorage>(Lifestyle.Singleton);
            container.Register<ISerializer, JsonSerializer>(Lifestyle.Singleton);
            container.Register<IObjectConverter, ObjectConverter>(Lifestyle.Singleton);

            container.Register<IEventBus, StringEventBusAdapter>(Lifestyle.Singleton);
            container.Register<ITypedEventBus<string>, RedisEventBus>(Lifestyle.Singleton);
            container.Register<IEventBusWrapper, EventBusWrapper>(Lifestyle.Singleton);
            
            container.Register<IMessageHandler<UserInfo>, UserRegisteredEventHandler>(Lifestyle.Scoped);
            container.Register<IEventDispatcher, EventDispatcher>(Lifestyle.Singleton);
            
            RegisterCache();
            
            // Allow Simple Injector to resolve services from ASP.NET Core.
            container.AutoCrossWireAspNetComponents(app);
        }

        private void RegisterCache()
        {
            var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
            container.Register<IUniversalCache, UniversalCache>(Lifestyle.Singleton);
            
            if (redisConfiguration.ConnectionFailedFallback == ConnectionFailedFallback.Ignore)
            {
                AddRedis(redisConfiguration);
                return;
            }

            try
            {
                var redisConnectionFactory = new RedisConnectionFactory(redisConfiguration);
                redisConnectionFactory.Connection.GetDatabase().Ping();
                AddRedis(redisConfiguration);
            }
            // catching Exception and not RedisException because of the issue with strongly named StackExchange.Redis
            // assembly - classes (with their fully qualified names) are duplicated. There is a workaround which requires
            // editing manually the .csproj file and it works for builds but Rider doesn't handle it and shows errors
            // which makes auto completion and inspections useless
            catch (Exception ex)
            {
                switch (redisConfiguration.ConnectionFailedFallback)
                {
                    case ConnectionFailedFallback.Error:
                        throw;
                    case ConnectionFailedFallback.InMemoryCache:
                        Logger.LogWarning(ex, "Redis connection failed, using InMemoryCache as fallback.");
                        AddInMemoryCache();
                        return;
                    case ConnectionFailedFallback.TryStart:
                        // TODO try start the redis server - probably just a 'redis-server' command and check the result
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void AddInMemoryCache()
        {
            container.Register<IMemoryCache, MemoryCache>(Lifestyle.Singleton);
            container.Register<ICache, InMemoryObjectCache>(Lifestyle.Singleton);
        }

        private void AddRedis(RedisConfiguration redisConfiguration)
        {            
            container.Register<IRedisConfiguration>(() => redisConfiguration, Lifestyle.Singleton);
            
            container.Register<IRedisConnectionFactory, RedisConnectionFactory>(Lifestyle.Singleton);
            container.Register<ICache<string>, RedisStringCache>(Lifestyle.Singleton);
            container.Register<ICache, ObjectCache>(Lifestyle.Singleton);
        }

        private bool ShouldCommandInvalidateCache(Type serviceType)
        {
            return !(serviceType== typeof(ICommandHandler<ShareFileCommand>)
                   || serviceType == typeof(ICommandHandler<AddFileRequest>)
                   || serviceType == typeof(ICommandHandler<ShareFileRequest>));
        }

        private bool ShouldQueryHandlerBeCached(Type serviceType)
        {
            return serviceType == typeof(IQueryHandler<FindFilesByUserQuery, IEnumerable<File>>);
        }
    }
}