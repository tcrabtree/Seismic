using Seismic.Data.Entities;

namespace Seismic.Data.Interfaces;

public interface IWaveformRepository
{
    Task<List<WaveformPoint>> ListByEvent(Guid eventId);
    Task AddRange(IEnumerable<WaveformPoint> points);
}
