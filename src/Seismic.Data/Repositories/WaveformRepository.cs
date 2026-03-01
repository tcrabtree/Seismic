using Microsoft.EntityFrameworkCore;
using Seismic.Data.Context;
using Seismic.Data.Entities;
using Seismic.Data.Interfaces;

namespace Seismic.Data.Repositories;

public sealed class WaveformRepository(SeismicDbContext context) : IWaveformRepository
{
    public async Task<List<WaveformPoint>> ListByEvent(Guid eventId)
    {
        return await context.WaveformPoints
            .Where(p => p.EventId == eventId)
            .OrderBy(p => p.Time)
            .ToListAsync();
    }

    public async Task AddRange(IEnumerable<WaveformPoint> points)
    {
        await context.WaveformPoints.AddRangeAsync(points);
        await context.SaveChangesAsync();
    }
}
