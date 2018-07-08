﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FileService.Cache;
using FileService.Cache.Redis;
using FileService.Commands;
using FileService.Database;
using FileService.Middlewares;
using FileService.Model;
using FileService.Queries;
using FileService.Requests;
using FileService.Serialization;
using FileService.Services;
using FileService.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using JsonSerializer = FileService.Serialization.JsonSerializer;

namespace FileService
{
    public class Startup
    {
        private readonly Container container = new Container();
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseCors("MyPolicy");
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMvc();
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));

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
                context => ShouldCommandInvalidateCache(context));

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
            container.Register<IRedisConnectionFactory, RedisConnectionFactory>(Lifestyle.Singleton);
            container.Register<ICache<string>, RedisStringCache>(Lifestyle.Singleton);
            container.Register<ICache, ObjectCache>(Lifestyle.Singleton);
            container.Register<IUniversalCache, UniversalCache>(Lifestyle.Singleton);

            // Allow Simple Injector to resolve services from ASP.NET Core.
            container.AutoCrossWireAspNetComponents(app);
        }

        private bool ShouldCommandInvalidateCache(DecoratorPredicateContext context)
        {
            return !(context.ServiceType == typeof(ICommandHandler<ShareFileCommand>)
                   || context.ServiceType == typeof(ICommandHandler<AddFileRequest>)
                   || context.ServiceType == typeof(ICommandHandler<ShareFileRequest>));
        }

        private bool ShouldQueryHandlerBeCached(Type serviceType)
        {
            return serviceType == typeof(IQueryHandler<FindFilesByUserQuery, IEnumerable<File>>);
        }
    }
}