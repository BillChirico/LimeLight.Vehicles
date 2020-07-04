using LimeLight.Vehicles.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LimeLight.Vehicles.Service.Discord
{
    public interface IDiscordBot
    {
        Task Connect(string token);

        Task AnnounceCars(ulong guildId, ulong channelId, List<Vehicle> vehicles);
    }
}