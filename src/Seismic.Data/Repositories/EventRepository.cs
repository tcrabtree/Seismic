using Microsoft.EntityFrameworkCore;
using Seismic.Data.Context;
using Seismic.Data.Entities;
using Seismic.Data.Interfaces;

namespace Seismic.Data.Repositories;

public sealed class EventRepository(SeismicDbContext context) : IEventRepository
{
    public async Task<Event?> Get(Guid eventId)
    {
        return await context.Events
            .Include(e => e.WaveformPoints)
            .FirstOrDefaultAsync(e => e.EventId == eventId);
    }

    public async Task<List<Event>> ListBySite(string siteId)
    {
        return await context.Events
            .Where(e => e.SiteId == siteId)
            .OrderByDescending(e => e.EventTimestamp)
            .ToListAsync();
    }

    public async Task Add(Event entity)
    {
        await context.Events.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(Event entity)
    {
        context.Events.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid eventId)
    {
        var entity = await context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
        if (entity is null)
        {
            return;
        }

        context.Events.Remove(entity);
        await context.SaveChangesAsync();
    }
}
