using Common;
using Microsoft.AspNetCore.Hosting;

namespace AngularApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseDefaults(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}