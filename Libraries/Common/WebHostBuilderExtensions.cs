using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseDefaults(this WebHostBuilder webHostBuilder, string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();

            // A workaround - for some reason the builder takes the url from the json file instead of the environment variable
            // if both are present, despite the fact that the documented behaviour is different.
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
                configBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("hosting.json", optional: true);
            
            configBuilder
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;

                    config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .UseConfiguration(configBuilder.Build())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });
        }
    }
}