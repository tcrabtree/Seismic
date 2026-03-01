namespace Seismic.UI.ViewModels;

public sealed class EventDetailViewModel
{
    public int EventId { get; init; }
    public string MonitorName { get; init; } = string.Empty;
    public double ValidityScore { get; init; }
    public string Confidence { get; init; } = "Low";
    public double DeltaTMilliseconds { get; init; }
    public double DistanceMeters { get; init; }
    public double DistanceErrorBandMeters { get; init; }

    public IReadOnlyList<string> ContributingFactors { get; init; } = [];

    public double PeakPpvR { get; init; }
    public double PeakPpvT { get; init; }
    public double PeakPpvV { get; init; }
    public double RmsEnergy { get; init; }
    public double SpectralCentroid { get; init; }
    public double AxisCorrelation { get; init; }

    public int SeismicOnsetIndex { get; init; }
    public int AcousticOnsetIndex { get; init; }
    public IReadOnlyList<double> SeismicWaveform { get; init; } = [];
    public IReadOnlyList<double> AcousticWaveform { get; init; } = [];
}
