using Microsoft.EntityFrameworkCore;
using sevaLK_service_auth.Domain.Entities;

namespace sevaLK_service_auth.Infra.Config;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.Phone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(u => u.Address)
                .HasMaxLength(500);

            entity.Property(u => u.ProfileImageKey)
                .HasMaxLength(500);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(u => u.RefreshToken)
                .HasMaxLength(500);

            entity.Property(u => u.PasswordResetToken)
                .HasMaxLength(500);

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.ToTable("Users");
        });
    }
}
