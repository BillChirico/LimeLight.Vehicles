using System.Collections.Generic;
using Discord;
using LimeLight.Vehicles.Domain;

namespace LimeLight.Vehicles.Service.Discord
{
    public static class EmbedHelper
    {
        public static Embed GetVehicleEmbed(List<Vehicle> vehicles)
        {
            var builder = new EmbedBuilder()
                .WithTitle("LimeLight Vehicles")
                .WithDescription("All current vehicles with their price - Head down to PDM to purchase")
                .WithColor(new Color(50, 205, 50))
                .WithCurrentTimestamp();

            foreach (var vehicle in vehicles)
                builder.AddField($"{vehicle.Name}", $"Price: {vehicle.Price:C0} | Stock: {vehicle.Stock}", true);

            return builder.Build();
        }
    }
}
