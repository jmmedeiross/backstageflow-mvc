using BackstageFlow.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BackstageFlow.Web.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<ArtistBooking> Artists => Set<ArtistBooking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(120).IsRequired();
            entity.Property(item => item.Venue).HasMaxLength(120).IsRequired();
            entity.Property(item => item.City).HasMaxLength(80).IsRequired();
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<ArtistBooking>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.StageName);
            entity.HasIndex(item => item.PerformanceTime);
            entity.Property(item => item.StageName).HasMaxLength(100).IsRequired();
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(30);
            entity.HasOne(item => item.Event)
                .WithMany(item => item.Artists)
                .HasForeignKey(item => item.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
