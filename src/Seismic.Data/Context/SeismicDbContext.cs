using Microsoft.EntityFrameworkCore;
using Seismic.Data.Entities;

namespace Seismic.Data.Context;

public sealed class SeismicDbContext(DbContextOptions<SeismicDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<WaveformPoint> WaveformPoints => Set<WaveformPoint>();
    public DbSet<MonitorHealth> MonitorHealths => Set<MonitorHealth>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Events");
            entity.HasKey(x => x.EventId);
            entity.HasIndex(x => x.SiteId);
            entity.HasIndex(x => x.EventTimestamp);

            entity.Property(x => x.SiteId).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MonitorId).HasMaxLength(100).IsRequired();
            entity.Property(x => x.RawCsvPath).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<WaveformPoint>(entity =>
        {
            entity.ToTable("WaveformPoints");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.EventId);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.WaveformPoints)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MonitorHealth>(entity =>
        {
            entity.ToTable("MonitorHealths");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.MonitorId, x.Date });

            entity.Property(x => x.MonitorId).HasMaxLength(100).IsRequired();
        });
    }
}
