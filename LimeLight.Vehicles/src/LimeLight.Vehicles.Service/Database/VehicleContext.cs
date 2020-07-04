using LimeLight.Vehicles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LimeLight.Vehicles.Service.Database
{
    public class VehicleContext : DbContext
    {
        private readonly ConfigSettings _settings;

        public VehicleContext(IOptions<ConfigSettings> settings)
        {
            _settings = settings.Value;
        }
        
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_settings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property<string>("model");
                entity.HasKey("model");
            });
        }
    }
}