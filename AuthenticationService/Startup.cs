using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Database;
using FileService.Model;
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
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UserStore = FileService.Authentication.UserStore;

namespace FileService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = false; // TODO: !HostingEnvironment.IsDevelopment();
            }).AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
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

            services.AddDbContext<UserDbContext>(builder => builder.UseInMemoryDatabase("InMemoryDb"));
            services.AddDbContext<DbContext>(builder => builder.UseInMemoryDatabase("InMemoryDb"));
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<IdentityRole>, RoleStore<IdentityRole>>();
            services.AddTransient<TokenService>();

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new ValidationFilter());
                    options.Filters.Add(new AuthorizeFilter());
                })
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<ValidationMessage>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, UserDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }

            InitializeDatabase(app, env, context);
//            InitializeDatabase(app, env);
            app.UseAuthentication();
            app.UseCors("MyPolicy");
            app.UseMvc();
        }

        private void InitializeDatabase(IApplicationBuilder app, IHostingEnvironment env, UserDbContext context)
        {
            if (context.Database.EnsureCreated() && env.IsDevelopment() && !context.Users.Any())
            {
                using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>())
                        SeedDatabase(userManager);
                }
            }
        }

//        private void InitializeDatabase(IApplicationBuilder app, IHostingEnvironment env)
//        {
//            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
//            {
//                using (var context = scope.ServiceProvider.GetRequiredService<UserDbContext>())
//                {
//                    if (context.Database.EnsureCreated() && env.IsDevelopment())
//                        SeedDatabase(context, app.ApplicationServices.GetService<UserManager<User>>().PasswordHasher);
//                }
//            }
//        }

        private void SeedDatabase(UserManager<User> userManager)
        {
            var testUser = new User()
            {
                Email = "test@mail.com",
                EmailConfirmed = true,
                Id = Guid.NewGuid().ToString(),
                NormalizedEmail = "test@mail.com".ToUpper(),
                NormalizedUserName = "testUser".ToUpper(),
                UserName = "testUser"
            };
            Debug.WriteLine("Create the test user.");
            userManager.CreateAsync(testUser).Wait();
            userManager.AddPasswordAsync(testUser, "test123").Wait();
        }
    }
}