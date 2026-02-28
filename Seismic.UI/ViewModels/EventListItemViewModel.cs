namespace Seismic.UI.ViewModels;

public sealed class EventListItemViewModel
{
    public int Id { get; init; }
    public DateTime TimestampUtc { get; init; }
    public string MonitorId { get; init; } = string.Empty;
    public double DeltaTMilliseconds { get; init; }
    public double EstimatedDistanceMeters { get; init; }
    public double ValidityScore { get; init; }
    public string Confidence { get; init; } = "Low";
    public bool DistanceDeviationFlag { get; init; }
    public bool HealthWarningFlag { get; init; }
}
