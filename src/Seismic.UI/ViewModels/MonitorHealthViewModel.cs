namespace Seismic.UI.ViewModels;

public sealed class MonitorHealthViewModel
{
    public int MonitorId { get; init; }
    public string MonitorName { get; init; } = string.Empty;
    public double HealthScore { get; init; }
    public double R2Value { get; init; }
    public string AxisSlopeRatios { get; init; } = string.Empty;
    public double EnergySymmetryRatio { get; init; }

    public IReadOnlyList<string> TrendLabels { get; init; } = [];
    public IReadOnlyList<double> TrendScores { get; init; } = [];
    public IReadOnlyList<(DateTime DateUtc, string Severity, string Message)> Anomalies { get; init; } = [];
}
