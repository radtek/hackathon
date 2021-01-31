using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace XPInc.Hackathon.Hosts.Api
{
    public static class Program
    {
        public static async Task Main()
        {
            // Eliminating any console arguments.
            string[] argsInternal = Enumerable.Empty<string>()
                                                .ToArray();

            await CreateHostBuilder(argsInternal)
                    .UseSerilog()
                    .Build()
                    .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();

                    webBuilder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    });

                    webBuilder.UseStartup<Startup>();
                });

    }
}
