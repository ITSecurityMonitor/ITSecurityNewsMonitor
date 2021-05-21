using ITSecurityNewsMonitor.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            ApplyPendingMigrations(host);

            host.Run();
        }

        private static async void ApplyPendingMigrations(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var identityContext = services.GetService<ApplicationDbContext>();
                    var pendingMigration = await identityContext.Database.GetPendingMigrationsAsync();

                    if(pendingMigration.Any())
                    {
                        logger.LogInformation("Pending migration found for identityContext");
                        await identityContext.Database.MigrateAsync();
                        logger.LogInformation("Pending migration applied for identityContext");
                    } else
                    {
                        logger.LogInformation("No pending migration found for identityContext");
                    }

                    var secNewsContext = services.GetService<SecNewsDbContext>();

                    pendingMigration = await secNewsContext.Database.GetPendingMigrationsAsync();
                    if(pendingMigration.Any())
                    {
                        logger.LogInformation("Pending migration found for secNewsContext");
                        secNewsContext.Database.Migrate();
                        logger.LogInformation("Pending migration applied for secNewsContext");
                    } else
                    {
                        logger.LogInformation("No pending migration found for secNewsContext");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
