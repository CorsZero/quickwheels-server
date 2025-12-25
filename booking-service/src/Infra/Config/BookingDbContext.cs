using Microsoft.EntityFrameworkCore;
using booking_service.Domain.Entities;

namespace booking_service.Infra.Config;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.RenterId)
                .IsRequired();

            entity.Property(b => b.VehicleId)
                .IsRequired();

            entity.Property(b => b.StartDate)
                .IsRequired();

            entity.Property(b => b.EndDate)
                .IsRequired();

            entity.Property(b => b.Days)
                .IsRequired();

            entity.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(b => b.Notes)
                .HasMaxLength(1000);

            entity.Property(b => b.RejectionReason)
                .HasMaxLength(500);

            entity.Property(b => b.CreatedAt)
                .IsRequired();

            entity.HasIndex(b => b.RenterId)
                .HasDatabaseName("idx_bookings_renter");

            entity.HasIndex(b => b.VehicleId)
                .HasDatabaseName("idx_bookings_vehicle");

            entity.HasIndex(b => b.Status)
                .HasDatabaseName("idx_bookings_status");

            entity.HasIndex(b => new { b.StartDate, b.EndDate })
                .HasDatabaseName("idx_bookings_dates");

            entity.ToTable("Bookings");
        });
    }
}
