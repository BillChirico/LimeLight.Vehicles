using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LimeLight.Vehicles.Domain;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace LimeLight.Vehicles.Service.Discord
{
    public class DiscordBot : IDiscordBot
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<DiscordBot> _logger;

        public DiscordBot(DiscordSocketClient client, ILogger<DiscordBot> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Connect(string token)
        {
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();
        }

        public async Task AnnounceCars(ulong guildId, ulong channelId, List<Vehicle> vehicles)
        {
            _logger.LogInformation($"Starting vehicle announcement for {vehicles.Count} vehicles");

            var channel = _client.GetGuild(guildId)?.GetTextChannel(channelId);

            if (channel == null)
            {
                _logger.LogError("Could not find channel to announce cars!");
                return;
            }

            // Delete all messages from the bot - Can't bulk delete because Discord doesn't allow it
            foreach (var message in await channel.GetMessagesAsync().FlattenAsync())
                if (message.Author.Id == _client.CurrentUser.Id)
                    await message.DeleteAsync();

            var batchedVehicles = vehicles.Batch(25).ToList();

            foreach (var vehiclePage in batchedVehicles)
                await _client.GetGuild(guildId).GetTextChannel(channelId)
                    .SendMessageAsync(embed: EmbedHelper.GetVehicleEmbed(vehiclePage.ToList()), text: string.Empty);

            _logger.LogInformation("Finished vehicle announcement");
        }


        private Task Log(LogMessage logMessage)
        {
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(logMessage.ToString());
                    break;
                case LogSeverity.Error:
                    _logger.LogError(logMessage.ToString());
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(logMessage.ToString());
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(logMessage.ToString());
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug(logMessage.ToString());
                    break;
                default:
                    _logger.LogCritical(logMessage.ToString());
                    break;
            }

            return Task.CompletedTask;
        }
    }
}