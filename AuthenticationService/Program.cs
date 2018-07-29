using Common;
using Microsoft.AspNetCore.Hosting;

namespace AuthenticationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseDefaults(args)
                .UseStartup<Startup>()
                .Build();
    }
}