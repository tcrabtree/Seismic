using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seismic.Data.Context;
using Seismic.Data.Entities;

namespace Seismic.Data.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SeismicDbContext>();

        await dbContext.Database.MigrateAsync();

        if (await dbContext.Events.AnyAsync())
        {
            return;
        }

        var sampleEventId = Guid.NewGuid();

        dbContext.Events.Add(new Event
        {
            EventId = sampleEventId,
            SiteId = "SITE-001",
            MonitorId = "MON-001",
            EventTimestamp = DateTime.UtcNow.AddMinutes(-15),
            Distance = 420,
            WeightPerDelay = 150,
            ScaledDistance = 34.29,
            RawCsvPath = "App_Data/Events/sample-event.csv",
            IsReviewed = false,
            CreatedAt = DateTime.UtcNow
        });

        dbContext.MonitorHealths.Add(new MonitorHealth
        {
            MonitorId = "MON-001",
            Date = DateTime.UtcNow.Date,
            HealthScore = 92.5,
            R2Value = 0.95,
            EnergySymmetry = 0.91
        });

        dbContext.WaveformPoints.AddRange(
            new WaveformPoint { EventId = sampleEventId, Time = 0.0, R = 0.01, T = 0.00, V = 0.02, A = 0.00 },
            new WaveformPoint { EventId = sampleEventId, Time = 0.1, R = 0.03, T = 0.01, V = 0.04, A = 0.01 },
            new WaveformPoint { EventId = sampleEventId, Time = 0.2, R = 0.02, T = 0.02, V = 0.03, A = 0.01 }
        );

        await dbContext.SaveChangesAsync();
    }
}
