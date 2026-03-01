using Microsoft.EntityFrameworkCore;
using Seismic.Data.Context;
using Seismic.Data.Entities;
using Seismic.Data.Interfaces;

namespace Seismic.Data.Repositories;

public sealed class MonitorHealthRepository(SeismicDbContext context) : IMonitorHealthRepository
{
    public async Task<List<MonitorHealth>> ListByMonitor(string monitorId)
    {
        return await context.MonitorHealths
            .Where(h => h.MonitorId == monitorId)
            .OrderByDescending(h => h.Date)
            .ToListAsync();
    }

    public async Task Add(MonitorHealth entity)
    {
        await context.MonitorHealths.AddAsync(entity);
        await context.SaveChangesAsync();
    }
}
