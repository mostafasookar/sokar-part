using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PartsUnlimited.Models;
using Microsoft.Extensions.Hosting;

namespace PartsUnlimited
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    PartsUnlimitedContext context = services.GetRequiredService<PartsUnlimitedContext>();
                    context.Database.EnsureCreated();
                    //Populates the PartsUnlimited sample data
                    SampleData.InitializePartsUnlimitedDatabaseAsync(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }


        public static IHostBuilder BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostContext, config) =>
                {
                    // This is the only configuration file used, development and production.
                    // ENHANCEMENT: Conditionally check the environment variable and load the config based on the environment.
                    config.AddJsonFile("config.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });   
        
       
    }
}
