using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Model;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ABC123_HSZF_2024251.Persistence.MsSql
{
    public class TaxiDbContext : DbContext
    {
        public DbSet<TaxiCar> Cars { get; set; }
        public DbSet<Fare> Fares { get; set; }
        public DbSet<TaxiCar> TaxiCars { get; set; } = null!;

        public TaxiDbContext(DbContextOptions<TaxiDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxiCar>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LicensePlate).IsRequired();
                entity.Property(e => e.Driver).IsRequired();

                // Egy autóhoz több fuvar tartozhat
                entity.HasMany(e => e.Fares)
                      .WithOne(e => e.Car)
                      .HasForeignKey(e => e.TaxiCarId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Fare>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.From).IsRequired();
                entity.Property(e => e.To).IsRequired();
                entity.Property(e => e.Distance).IsRequired();
                entity.Property(e => e.PaidAmount).IsRequired();
            });
        }
    }
}
