namespace Seismic.UI.Models;

public sealed class CsvEventMetadata
{
    public double MaxAbsR { get; init; }
    public double MaxAbsT { get; init; }
    public double MaxAbsV { get; init; }
    public double MaxAbsA { get; init; }
    public double DurationSeconds { get; init; }
    public double OnsetTimeSeconds { get; init; }
}
