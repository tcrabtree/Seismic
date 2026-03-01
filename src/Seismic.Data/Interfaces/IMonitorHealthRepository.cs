using Seismic.Data.Entities;

namespace Seismic.Data.Interfaces;

public interface IMonitorHealthRepository
{
    Task<List<MonitorHealth>> ListByMonitor(string monitorId);
    Task Add(MonitorHealth entity);
}
