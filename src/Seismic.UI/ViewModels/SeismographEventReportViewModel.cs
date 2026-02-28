namespace Seismic.UI.ViewModels;

public sealed class SeismographEventReportViewModel
{
    public EventHeader Header { get; init; } = new();
    public BlastSummaryData BlastSummary { get; init; } = new();
    public MeasurementMatrix Measurements { get; init; } = new();
    public WaveformDataSet WaveformData { get; init; } = new();
    public FrequencyPlotDataSet FrequencyPlotData { get; init; } = new();

    public sealed class EventHeader
    {
        public string MonitoringCompany { get; init; } = string.Empty;
        public string ClientCompany { get; init; } = string.Empty;
        public string SiteName { get; init; } = string.Empty;
        public string MonitorId { get; init; } = string.Empty;
        public string OperatorName { get; init; } = string.Empty;
        public string EventId { get; init; } = string.Empty;
        public DateTime EventTimestamp { get; init; }
        public TimeSpan RecordDuration { get; init; }
        public int SampleRate { get; init; }
        public DateOnly LastCalibration { get; init; }
    }

    public sealed class BlastSummaryData
    {
        public double Distance { get; init; }
        public double WeightPerDelay { get; init; }
        public double ScaledDistance { get; init; }
        public double TriggerLevel { get; init; }
    }

    public sealed class MeasurementMatrix
    {
        public double RadialVelocity { get; init; }
        public double TransverseVelocity { get; init; }
        public double VerticalVelocity { get; init; }
        public double AirPsi { get; init; }

        public double RadialFrequency { get; init; }
        public double TransverseFrequency { get; init; }
        public double VerticalFrequency { get; init; }
        public double AirFrequency { get; init; }

        public double RadialDisplacement { get; init; }
        public double TransverseDisplacement { get; init; }
        public double VerticalDisplacement { get; init; }
        public double AirDbL { get; init; }

        public double RadialAcceleration { get; init; }
        public double TransverseAcceleration { get; init; }
        public double VerticalAcceleration { get; init; }
        public double TriggerToPeakSeconds { get; init; }
    }

    public sealed class WaveformDataSet
    {
        public IReadOnlyList<double> TimeSeconds { get; init; } = [];
        public IReadOnlyList<double> RadialSeries { get; init; } = [];
        public IReadOnlyList<double> TransverseSeries { get; init; } = [];
        public IReadOnlyList<double> VerticalSeries { get; init; } = [];
        public IReadOnlyList<double> AirSeries { get; init; } = [];
        public double SeismicOnsetTime { get; init; }
        public double AirOnsetTime { get; init; }
    }

    public sealed class FrequencyPlotDataSet
    {
        public IReadOnlyList<FrequencyPoint> RPoints { get; init; } = [];
        public IReadOnlyList<FrequencyPoint> TPoints { get; init; } = [];
        public IReadOnlyList<FrequencyPoint> VPoints { get; init; } = [];
        public IReadOnlyList<FrequencyPoint> USBMCurve { get; init; } = [];
    }

    public sealed class FrequencyPoint
    {
        public double Frequency { get; init; }
        public double Velocity { get; init; }
    }
}
