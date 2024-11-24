using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Model;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ABC123_HSZF_2024251.Persistence.MsSql
{
    public class TaxiDbContext : DbContext
    {
        // Konstruktor
        public TaxiDbContext(DbContextOptions<TaxiDbContext> options) : base(options)
        {
        }

        // DbSet-ek (táblák)
        public DbSet<TaxiCar> TaxiCars { get; set; } = null!;
        public DbSet<Fare> Fares { get; set; } = null!;

        // Modell konfiguráció
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TaxiCar konfiguráció
            modelBuilder.Entity<TaxiCar>(entity =>
            {
                entity.HasKey(tc => tc.Id);
                entity.Property(tc => tc.LicensePlate)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(tc => tc.Driver)
                    .IsRequired()
                    .HasMaxLength(100);

                // Kapcsolat
                entity.HasMany(tc => tc.Fares)
                    .WithOne(f => f.TaxiCar)
                    .HasForeignKey(f => f.TaxiCarId);
            });

            // Fare konfiguráció
            modelBuilder.Entity<Fare>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.From)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(f => f.To)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(f => f.Distance)
                    .IsRequired();
                entity.Property(f => f.PaidAmount)
                    .IsRequired();
            });
        }
    }
}
