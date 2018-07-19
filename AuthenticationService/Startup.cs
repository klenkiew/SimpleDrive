using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Configuration;
using AuthenticationService.Database;
using AuthenticationService.Model;
using AuthenticationService.Services;
using AuthenticationService.Validation;
using CommonEvents;
using EventBus;
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
using Redis;
using Serialization;
using UserStore = AuthenticationService.Authentication.UserStore;

namespace AuthenticationService
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
            var requireEmailConfirmation = Convert.ToBoolean(Configuration["Security:EmailConfirmation:RequireEmailConfirmation"]);
            
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = requireEmailConfirmation;
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
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUsersService, UsersService>();
            
            if (requireEmailConfirmation)
                services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
            else
                services.AddScoped<IEmailConfirmationService, EmptyEmailConfirmationService>();            
            
            services.AddSingleton<IEmailService, EmailService>();

            services.AddSingleton(provider => Configuration.GetSection("Security").GetSection("EmailConfirmation")
                .Get<EmailConfirmationConfiguration>());
            services.AddSingleton(provider => Configuration.GetSection("EmailClient").Get<EmailClientConfiguration>());
            
            services.AddSingleton<IRedisConfiguration, RedisConfiguration>(
                provider => new RedisConfiguration(Configuration["Redis:Host"]));
            services.AddSingleton<IEventBus, StringEventBusAdapter>();
            services.AddSingleton<ITypedEventBus<string>, RedisEventBus>();
            services.AddSingleton<IEventBusWrapper, EventBusWrapper>();
            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
            services.AddSingleton<ISerializer, JsonSerializer>();
            
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
            app.UseAuthentication();
            app.UseCors("MyPolicy");
            app.UseMvc();

            var eventBus = app.ApplicationServices.GetService<IEventBusWrapper>();
            eventBus.Publish<IEvent<AuthenticationServiceStarted>, AuthenticationServiceStarted>(AuthenticationServiceStartedEvent.Create());
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

        private void SeedDatabase(UserManager<User> userManager)
        {
            var testUser = new User()
            {
                Email = "test@mail.com",
                EmailConfirmed = true,
                Id = Guid.NewGuid().ToString(),
                NormalizedEmail = "test@mail.com".ToUpper(),
                NormalizedUsername = "testUser".ToUpper(),
                Username = "testUser"
            };
            Debug.WriteLine("Create the test user.");
            userManager.CreateAsync(testUser).Wait();
            userManager.AddPasswordAsync(testUser, "test123").Wait();
        }
    }
}