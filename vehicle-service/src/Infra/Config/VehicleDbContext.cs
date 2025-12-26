using Microsoft.EntityFrameworkCore;
using vehicle_service.Domain.Entities;

namespace vehicle_service.Infra.Config;

public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.OwnerId)
                .IsRequired();

            entity.Property(v => v.Make)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.Year)
                .IsRequired();

            entity.Property(v => v.Category)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(v => v.Transmission)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(v => v.FuelType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(v => v.Seats)
                .IsRequired();

            entity.Property(v => v.PricePerDay)
                .IsRequired()
                .HasPrecision(10, 2);

            entity.Property(v => v.Location)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.District)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.Description)
                .HasColumnType("text");

            entity.Property(v => v.Features)
                .HasColumnType("text");

            entity.Property(v => v.Images)
                .HasColumnType("text");

            entity.Property(v => v.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(v => v.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(v => v.CreatedAt)
                .IsRequired();

            // Indexes
            entity.HasIndex(v => v.OwnerId)
                .HasDatabaseName("idx_vehicles_owner");

            entity.HasIndex(v => new { v.Location, v.District })
                .HasDatabaseName("idx_vehicles_location");

            entity.HasIndex(v => new { v.Status, v.IsActive })
                .HasDatabaseName("idx_vehicles_status");

            entity.HasIndex(v => v.PricePerDay)
                .HasDatabaseName("idx_vehicles_price");

            entity.HasIndex(v => v.Category)
                .HasDatabaseName("idx_vehicles_category");

            entity.HasIndex(v => v.CreatedAt)
                .IsDescending()
                .HasDatabaseName("idx_vehicles_created");

            entity.ToTable("Vehicles");
        });
    }
}
