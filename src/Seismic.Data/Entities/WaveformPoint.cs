namespace Seismic.Data.Entities;

public sealed class WaveformPoint
{
    public long Id { get; set; }
    public Guid EventId { get; set; }
    public double Time { get; set; }
    public double R { get; set; }
    public double T { get; set; }
    public double V { get; set; }
    public double A { get; set; }

    public Event? Event { get; set; }
}
