using Seismic.Data.Entities;

namespace Seismic.Data.Interfaces;

public interface IEventRepository
{
    Task<Event?> Get(Guid eventId);
    Task<List<Event>> ListBySite(string siteId);
    Task Add(Event entity);
    Task Update(Event entity);
    Task Delete(Guid eventId);
}
