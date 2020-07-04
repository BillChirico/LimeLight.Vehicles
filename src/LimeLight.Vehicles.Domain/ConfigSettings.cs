namespace LimeLight.Vehicles.Domain
{
    public class ConfigSettings
    {
        public string DiscordBotToken { get; set; }

        public ulong DiscordGuildId { get; set; }

        public ulong DiscordChannelId { get; set; }

        public string ConnectionString { get; set; }
    }
}