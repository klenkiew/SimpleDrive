using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cache;
using Common.Validation;
using CommonEvents;
using EventBus;
using FileService.Commands;
using FileService.Commands.Decorators;
using FileService.Commands.Validators;
using FileService.Configuration;
using FileService.Database;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Events;
using FileService.Events.Handlers;
using FileService.Infrastructure;
using FileService.Infrastructure.HttpClient;
using FileService.Infrastructure.Middlewares;
using FileService.Infrastructure.ScopedServices;
using FileService.Infrastructure.WebSockets;
using FileService.Model;
using FileService.Queries;
using FileService.Queries.Decorators;
using FileService.Services;
using FileService.WebSocketHandlers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Npgsql;
using Npgsql.Logging;
using Redis;
using Redis.Cache;
using Serialization;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using JsonSerializer = Serialization.JsonSerializer;
using WebSocketManager = FileService.Infrastructure.WebSockets.WebSocketManager;

namespace FileService
{
    public class Startup
    {
        private readonly Container container = new Container();
        private readonly ILoggerFactory loggerFactory;
        private ILogger Logger { get; }                         
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            this.loggerFactory = loggerFactory;
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
                    .AllowAnyHeader()
                    .AllowCredentials();
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
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddSignalR();
            
            services.AddEntityFrameworkNpgsql().AddDbContext<FileDbContext>(options => options
                    .UseNpgsql("Host=localhost;Database=FileServiceDb;Username=dotnetUser;Pooling=true;"));
            NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Info, true, true);
            
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
                if (UsingInMemoryDb(app))
                    ClearRedisCache();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthentication();
            app.UseCors("MyPolicy");
            
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024,
            };
            app.UseWebSockets(webSocketOptions);
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<FileContentChangedNotificationHub>("/contentChangesHub");
            });
            
            app.UseMiddleware<WebSocketsMiddleware>(container);
            
            var webSocketHandlerRegistry = container.GetInstance<IWebSocketHandlerRegistrar>();
            var webSocketHandler = container.GetInstance<IWebSocketHandler<CurrentLockNotificationsSubscriptionMessage>>();
            webSocketHandlerRegistry.RegisterHandler("/ws/fileLocks", webSocketHandler);
            
            app.UseMvc();
            
            container.GetInstance<EventDispatcher>().SubscribeToEvents();
            container.GetInstance<UsersIntegrationService>().Run();
        }

        private bool UsingInMemoryDb(IApplicationBuilder app)
        {   
            using (var scope = app.ApplicationServices.CreateScope())
            {
                return scope.ServiceProvider.GetService<DbContextOptions>().Extensions
                    .Any(extension => extension is InMemoryOptionsExtension);
            }
        }

        private void ClearRedisCache()
        {
            Logger.LogInformation("Clearing the redis cache [development mode]");
            const string defaultRedisPort = "6379";
            var redisConfiguration = container.GetInstance<IRedisConfiguration>();
            var redisHost = redisConfiguration.Host + ":" + defaultRedisPort;
            var connectionFactory = new AdminModeRedisConnectionFactory(redisConfiguration);
            connectionFactory.Connection.GetServer(redisHost).FlushAllDatabases();
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
            
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(TransactionCommandHandlerDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(PostCommitCommandHandlerDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandValidatorDecorator<>));

            container.Register(typeof(IQueryHandler<,>), typeof(IQueryHandler<,>).Assembly);
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(LoggedQuery<,>));

            container.Register<PostCommitRegistratorImpl>(Lifestyle.Scoped);
            container.Register<IPostCommitRegistrator>(() => container.GetInstance<PostCommitRegistratorImpl>());
            container.Register<IPostCommitEventPublisher, PostCommitEventPublisher>();

            container.Register(typeof(AbstractValidator<>), typeof(NullCommandValidator<>).Assembly);
            container.RegisterConditional(typeof(AbstractValidator<>), typeof(NullCommandValidator<>),
                context => !context.Handled);

            container.Register<IFileRepository, FileRepository>(Lifestyle.Scoped);
            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);

            container.Register<ITransactionProvider, TransactionProvider>(Lifestyle.Scoped);
            container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);
            
            container.Register<IFileLockingService, FileLockingService>(Lifestyle.Singleton);
            container.Register<IFileLockExpiryNotificator, FileLockExpiryNotificator>(Lifestyle.Singleton);
            container.Register<IFileStorage, LocalFileStorage>(Lifestyle.Singleton);
            container.RegisterDecorator<IFileStorage, LockingFileStorage>();
            container.Register<IMimeTypeMap, MimeTypeMap>();
            
            container.Register<ICurrentUser, CurrentUser>(Lifestyle.Singleton);
            container.Register<ICurrentUserSource, CurrentUserSource>(Lifestyle.Scoped);
            var storageConfiguration = Configuration.GetSection("Storage").Get<StorageConfiguration>();
            container.RegisterInstance(storageConfiguration);
            container.Register<ISerializer, JsonSerializer>(Lifestyle.Singleton);
            container.Register<IObjectConverter, ObjectConverter>(Lifestyle.Singleton);

            container.Register<IEventBus, StringEventBusAdapter>(Lifestyle.Singleton);
            container.Register<ITypedEventBus<string>, RedisEventBus>(Lifestyle.Singleton);
            container.Register<IEventBusWrapper, EventBusWrapper>(Lifestyle.Singleton);
            container.Register<IPublisherWrapper>(() => container.GetInstance<IEventBusWrapper>(), Lifestyle.Singleton);
            container.Register<ISubscriberWrapper>(() => container.GetInstance<IEventBusWrapper>(), Lifestyle.Singleton);
            
            container.Register<IEventHandler<UserRegisteredEvent, UserInfo>, UserRegisteredEventHandler>(Lifestyle.Scoped);
            
            container.Register<EventDispatcher>(Lifestyle.Singleton);
            container.Register<UsersIntegrationService>(Lifestyle.Singleton);
            container.Register<IHttpClientAccessor, HttpClientAccessor>(Lifestyle.Singleton);
            container.Register<IHttpClientWrapper, HttpClientWrapper>(Lifestyle.Singleton);
            container.Register<IScopedServiceFactory<FileDbContext>, ScopedServiceFactory<FileDbContext>>(Lifestyle.Singleton);

            var registration = Lifestyle.Singleton.CreateRegistration<WebSocketHandlerRegistry>(container);
            container.AddRegistration(typeof(IWebSocketHandlerRegistry), registration);
            container.AddRegistration(typeof(IWebSocketHandlerRegistrar), registration);
            
            container.Register<IWebSocketManager, WebSocketManager>(Lifestyle.Singleton);
            container.Register<IWebSocketObjectMessageSender, WebSocketObjectMessageSender>(Lifestyle.Singleton);
            container.Register<IWebSocketBinaryMessageSender, WebSocketBinaryMessageSender>(Lifestyle.Singleton);
            container.Register<IWebSocketTextMessageSender, WebSocketTextMessageSender>(Lifestyle.Singleton);
            container.Register<IClientGroupsManager, ClientGroupsManager>(Lifestyle.Singleton);
                        
            container.Register<IWebSocketHandler<CurrentLockNotificationsSubscriptionMessage>,
                FileLockChangedNotificator>(Lifestyle.Singleton);

            container.Register<IDbConnection>(() => new NpgsqlConnection("Host=localhost;Database=FileServiceDb;Username=dotnetUser;Pooling=true;"), Lifestyle.Scoped);
            
            AddAutoMapper();
            container.RegisterSingleton(typeof(IMapper<,>), typeof(Mapper<,>));
            
            AddRedis();
            ConfigureCache();
            
            // Allow Simple Injector to resolve services from ASP.NET Core.
            container.AutoCrossWireAspNetComponents(app);
        }

        private void AddAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FileDto, File>(MemberList.Source);
                cfg.CreateMap<UserDto, User>(MemberList.Source);
            });
            config.AssertConfigurationIsValid();
            container.RegisterSingleton<IMapper>(() => new Mapper(config));
        }

        private void AddRedis()
        {
            var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
            container.Register<IRedisConfiguration>(() => redisConfiguration, Lifestyle.Singleton);
            container.Register<IRedisConnectionFactory, RedisConnectionFactory>(Lifestyle.Singleton);
            
            var redisConnectionFactory = new RedisConnectionFactory(redisConfiguration);
            redisConnectionFactory.Connection.GetDatabase().Ping();
        }

        private void ConfigureCache()
        {
            var cacheConfig = Configuration.GetSection("Cache").Get<CacheConfiguration>();
                       
            if (cacheConfig.CacheType == CacheType.None)
                return;
            
            container.Register<IUniversalCache, UniversalCache>(Lifestyle.Singleton);
            
            RegisterCacheInvalidationHandlers();

            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(CachedQuery<,>),
                context => ShouldQueryHandlerBeCached(context.ServiceType));
            
            if (cacheConfig.CacheType == CacheType.Redis)
                AddRedisCache();
            else if (cacheConfig.CacheType == CacheType.InMemory)
                AddInMemoryCache();
        }

        private void AddInMemoryCache()
        {
            container.Register<IMemoryCache, MemoryCache>(Lifestyle.Singleton);
            container.Register<ICache, InMemoryObjectCache>(Lifestyle.Singleton);
        }

        private void AddRedisCache()
        {            
            container.Register<ICache<string>, RedisStringCache>(Lifestyle.Singleton);
            container.Register<ICache, ObjectCache>(Lifestyle.Singleton);
        }

        private void RegisterCacheInvalidationHandlers()
        {
            container.Register<IEventHandler<FileAddedEvent, File>, FileAddedEventHandler>(Lifestyle.Scoped);

            var fileEditedRegistration = Lifestyle.Singleton.CreateRegistration<FileEditedEventHandler>(container);
            container.AddRegistration(typeof(IEventHandler<FileEditedEvent, File>), fileEditedRegistration);
            container.AddRegistration(typeof(IEventHandler<FileDeletedEvent, File>), fileEditedRegistration);
            
            var fileSharesChangedRegistration = Lifestyle.Singleton
                .CreateRegistration<FileSharesChangedEventHandler>(container);
            
            container.AddRegistration(
                typeof(IEventHandler<FileSharedEvent, FileSharesChangedMessage>), fileSharesChangedRegistration);
            container.AddRegistration(
                typeof(IEventHandler<FileUnsharedEvent, FileSharesChangedMessage>), fileSharesChangedRegistration);
        }

        private bool ShouldQueryHandlerBeCached(Type serviceType)
        {
            return serviceType == typeof(IQueryHandler<FindFilesByUserQuery, IEnumerable<FileDto>>)
                   || serviceType == typeof(IQueryHandler<FindFileByIdQuery, FileDto>)
                   || serviceType == typeof(IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<UserDto>>);
        }
    }
}