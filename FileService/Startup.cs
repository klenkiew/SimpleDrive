using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Cache;
using FileService.Cache.Redis;
using FileService.Commands;
using FileService.Database;
using FileService.Queries;
using FileService.Serialization;
using FileService.Services;
using FileService.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using File = FileService.Model.File;

namespace FileService
{
    public class Startup
    {
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

            services.AddDbContext<FileDbContext>(builder => builder.UseInMemoryDatabase("InMemoryDb"));

            services.AddTransient<ICommandHandler<AddFileCommand>, AddFileCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteFileCommand>, DeleteFileCommandHandler>();
            services.RegisterQueryHandler<FindFilesByOwnerQueryHandler, FindFilesByOwnerQuery, IEnumerable<File>>();
            
            // no caching of file content for now - query handler without decorators
            services.AddTransient<IQueryHandler<GetFileContentQuery, Stream>, GetFileContentQueryHandler>();
            
            services.AddSingleton<IFileStorage, LocalFileStorage>();
            services.AddSingleton<ISerializer, JsonSerializer>();
            services.AddSingleton<ICacheKeyConverter, CacheKeyConverter>();
            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
            services.AddSingleton<ICache<string>, RedisStringCache>();
            services.AddSingleton<ICache, ObjectCache>();
            services.AddSingleton<IUniversalCache, UniversalCache>();

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new ValidationFilter());
                    options.Filters.Add(new AuthorizeFilter());
                })
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<ValidationMessage>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors("MyPolicy");
            app.UseMvc();
        }
    }
}