using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Commands;
using FileService.Database;
using FileService.Model;
using FileService.Queries;
using FileService.Services;
using FileService.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FileService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            services.AddDbContext<FileDbContext>(builder => builder.UseInMemoryDatabase("InMemoryDb"), 
                optionsLifetime: ServiceLifetime.Scoped);

            services.AddTransient<ICommandHandler<AddFileCommand>, AddFileCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteFileCommand>, DeleteFileCommandHandler>();
            services.AddTransient<IQueryHandler<FindFilesByOwnerQuery, IEnumerable<File>>, FindFilesByOwnerQueryHandler>();
            services.AddSingleton<IFileStorage, LocalFileStorage>();
            
            services
                .AddMvc(options => options.Filters.Add(new ValidationFilter()))
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