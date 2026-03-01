namespace Seismic.Data.Entities;

public sealed class Event
{
    public Guid EventId { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string MonitorId { get; set; } = string.Empty;
    public DateTime EventTimestamp { get; set; }
    public double Distance { get; set; }
    public double WeightPerDelay { get; set; }
    public double ScaledDistance { get; set; }
    public string RawCsvPath { get; set; } = string.Empty;
    public bool IsReviewed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public ICollection<WaveformPoint> WaveformPoints { get; set; } = new List<WaveformPoint>();
}
