using Common;
using Microsoft.AspNetCore.Hosting;

namespace FileService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseDefaults(args)
                .UseStartup<Startup>()
                .Build();
    }
}