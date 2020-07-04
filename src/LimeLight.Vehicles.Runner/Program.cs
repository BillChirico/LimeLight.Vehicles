using Discord.WebSocket;
using LimeLight.Vehicles.Domain;
using LimeLight.Vehicles.Service.Database;
using LimeLight.Vehicles.Service.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LimeLight.Vehicles.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    // Settings
                    services.Configure<ConfigSettings>(GetSettingsFile("appsettings.json", "Settings"));

                    // Discord
                    services.AddSingleton<IDiscordBot, DiscordBot>();
                    services.AddSingleton<DiscordSocketClient>();

                    // Database
                    services.AddDbContext<VehicleContext>();
                });
        }

        private static IConfigurationSection GetSettingsFile(string file, string section)
        {
            var builder = new ConfigurationBuilder();

            builder.AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            return configuration.GetSection(section);
        }
    }
}