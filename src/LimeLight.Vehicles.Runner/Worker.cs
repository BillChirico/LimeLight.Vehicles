using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using LimeLight.Vehicles.Domain;
using LimeLight.Vehicles.Service.Database;
using LimeLight.Vehicles.Service.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LimeLight.Vehicles.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IDiscordBot _discordBot;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly ConfigSettings _settings;

        public Worker(ILogger<Worker> logger, IOptions<ConfigSettings> settings, IDiscordBot discordBot,
            DiscordSocketClient discordSocketClient, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _discordBot = discordBot;
            _discordSocketClient = discordSocketClient;
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Reset event
            var mre = new ManualResetEvent(false);

            await _discordBot.Connect(_settings.DiscordBotToken);

            _discordSocketClient.Ready += () =>
            {
                _logger.LogInformation("Discord client is ready");

                return Task.CompletedTask;
            };

            _discordSocketClient.GuildAvailable += guild =>
            {
                _logger.LogInformation("Discord guild is ready");

                mre.Set();

                return Task.CompletedTask;
            };

            // Wait for all connectable services to be ready
            mre.WaitOne();

            // Announce vehicles
            Task.Run(async () =>
            {
                while (true)
                {
                    using var scope = _serviceProvider.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<VehicleContext>();

                    var vehicles = await context.Vehicles.ToListAsync(stoppingToken);

                    await _discordBot.AnnounceCars(_settings.DiscordGuildId, _settings.DiscordChannelId, vehicles);

                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}