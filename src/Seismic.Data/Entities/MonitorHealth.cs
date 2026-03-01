namespace Seismic.Data.Entities;

public sealed class MonitorHealth
{
    public int Id { get; set; }
    public string MonitorId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double HealthScore { get; set; }
    public double R2Value { get; set; }
    public double EnergySymmetry { get; set; }
}
