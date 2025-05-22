using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KooliProjekt.IntegrationTests.Helpers
{
    public class TestApplicationFactory<TTestStartup> : WebApplicationFactory<TTestStartup> where TTestStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var host = Host.CreateDefaultBuilder()
                            .ConfigureWebHost(builder =>
                            {
                                builder.UseContentRoot(".");

                                // Seadistame keskkonna nime
                                builder.ConfigureAppConfiguration((c, b) =>
                                {
                                    c.HostingEnvironment.ApplicationName = "KooliProjekt";
                                });

                                builder.UseStartup<TTestStartup>();  // Kasutame FakeStartup klassi
                            })
                            .ConfigureAppConfiguration((context, conf) =>
                            {
                                var projectDir = Directory.GetCurrentDirectory();
                                var configPath = Path.Combine(projectDir, "appsettings.json");

                                conf.AddJsonFile(configPath);  // Lisa appsettings.json konfigureerimiseks
                            });
            return host;
        }
    }
}
