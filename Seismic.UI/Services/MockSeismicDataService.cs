using Seismic.UI.ViewModels;

namespace Seismic.UI.Services;

public sealed class MockSeismicDataService
{
    private readonly List<SiteSummaryViewModel> _sites = new()
    {
        new() { Id = 1, Name = "North Quarry", EventCount = 42, FlaggedCount = 5, OverallHealth = "Healthy" },
        new() { Id = 2, Name = "East Pit", EventCount = 30, FlaggedCount = 8, OverallHealth = "Warning" },
        new() { Id = 3, Name = "West Bench", EventCount = 18, FlaggedCount = 7, OverallHealth = "Risk" }
    };

    public IReadOnlyList<SiteSummaryViewModel> GetSites() => _sites;

    public SiteEventsPageViewModel GetSiteEvents(int siteId, string confidence = "All", bool flaggedOnly = false)
    {
        var site = _sites.FirstOrDefault(s => s.Id == siteId) ?? _sites[0];

        var events = Enumerable.Range(1, 16).Select(i =>
        {
            var score = 52 + (i * 3 % 45);
            var confidenceLevel = score >= 80 ? "High" : score >= 60 ? "Medium" : "Low";
            return new EventListItemViewModel
            {
                Id = siteId * 1000 + i,
                TimestampUtc = DateTime.UtcNow.AddMinutes(-i * 37),
                MonitorId = $"M-{siteId:D2}-{(i % 4) + 1:D2}",
                DeltaTMilliseconds = 12 + i * 1.7,
                EstimatedDistanceMeters = 90 + i * 6.2,
                ValidityScore = score,
                Confidence = confidenceLevel,
                DistanceDeviationFlag = i % 5 == 0,
                HealthWarningFlag = i % 7 == 0
            };
        }).ToList();

        if (!string.Equals(confidence, "All", StringComparison.OrdinalIgnoreCase))
        {
            events = events.Where(e => string.Equals(e.Confidence, confidence, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (flaggedOnly)
        {
            events = events.Where(e => e.DistanceDeviationFlag || e.HealthWarningFlag).ToList();
        }

        return new SiteEventsPageViewModel
        {
            SiteId = site.Id,
            SiteName = site.Name,
            SelectedConfidence = confidence,
            ShowOnlyFlagged = flaggedOnly,
            TotalEvents = events.Count,
            AverageValidityScore = events.Count == 0 ? 0 : events.Average(e => e.ValidityScore),
            LowConfidenceCount = events.Count(e => e.Confidence == "Low"),
            DistanceDeviationCount = events.Count(e => e.DistanceDeviationFlag),
            Events = events
        };
    }

    public SeismographEventReportViewModel GetSeismographEventReport(int eventId)
    {
        const int sampleRate = 512;
        const double durationSeconds = 10.0;
        const int totalSamples = (int)(sampleRate * durationSeconds);

        var time = Enumerable.Range(0, totalSamples).Select(i => i / (double)sampleRate).ToList();
        var radial = time.Select(t => Math.Sin(2 * Math.PI * 8.0 * t) * 0.04 + (t > 3.12 ? Math.Sin(2 * Math.PI * 19 * t) * 0.14 : 0)).ToList();
        var transverse = time.Select(t => Math.Sin(2 * Math.PI * 10.5 * t) * 0.03 + (t > 3.16 ? Math.Sin(2 * Math.PI * 23 * t) * 0.11 : 0)).ToList();
        var vertical = time.Select(t => Math.Sin(2 * Math.PI * 7.0 * t) * 0.05 + (t > 3.14 ? Math.Sin(2 * Math.PI * 17 * t) * 0.16 : 0)).ToList();
        var air = time.Select(t => Math.Sin(2 * Math.PI * 2.1 * t) * 0.01 + (t > 3.24 ? Math.Sin(2 * Math.PI * 6 * t) * 0.08 : 0)).ToList();

        return new SeismographEventReportViewModel
        {
            Header = new SeismographEventReportViewModel.EventHeader
            {
                MonitoringCompany = "GeoSignal Monitoring Ltd.",
                ClientCompany = "Atlas Blasting Services",
                SiteName = "North Quarry Phase 2",
                MonitorId = "Unit 07",
                OperatorName = "R. Patel",
                EventId = $"EVT-{eventId}",
                EventTimestamp = DateTime.UtcNow.AddMinutes(-37),
                RecordDuration = TimeSpan.FromSeconds(durationSeconds),
                SampleRate = sampleRate,
                LastCalibration = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-21))
            },
            BlastSummary = new SeismographEventReportViewModel.BlastSummaryData
            {
                Distance = 428.0,
                WeightPerDelay = 185.0,
                ScaledDistance = 31.47,
                TriggerLevel = 0.080
            },
            Measurements = new SeismographEventReportViewModel.MeasurementMatrix
            {
                RadialVelocity = 0.327,
                TransverseVelocity = 0.289,
                VerticalVelocity = 0.352,
                AirPsi = 0.124,
                RadialFrequency = 19.3,
                TransverseFrequency = 22.8,
                VerticalFrequency = 16.7,
                AirFrequency = 6.4,
                RadialDisplacement = 0.0027,
                TransverseDisplacement = 0.0020,
                VerticalDisplacement = 0.0034,
                AirDbL = 132.6,
                RadialAcceleration = 0.0081,
                TransverseAcceleration = 0.0075,
                VerticalAcceleration = 0.0102,
                TriggerToPeakSeconds = 0.118
            },
            WaveformData = new SeismographEventReportViewModel.WaveformDataSet
            {
                TimeSeconds = time,
                RadialSeries = radial,
                TransverseSeries = transverse,
                VerticalSeries = vertical,
                AirSeries = air,
                SeismicOnsetTime = 3.14,
                AirOnsetTime = 3.24
            },
            FrequencyPlotData = new SeismographEventReportViewModel.FrequencyPlotDataSet
            {
                RPoints = [new() { Frequency = 12.5, Velocity = 0.19 }, new() { Frequency = 19.3, Velocity = 0.327 }, new() { Frequency = 28.0, Velocity = 0.24 }],
                TPoints = [new() { Frequency = 10.2, Velocity = 0.15 }, new() { Frequency = 22.8, Velocity = 0.289 }, new() { Frequency = 30.7, Velocity = 0.22 }],
                VPoints = [new() { Frequency = 9.6, Velocity = 0.17 }, new() { Frequency = 16.7, Velocity = 0.352 }, new() { Frequency = 25.1, Velocity = 0.27 }],
                USBMCurve = [new() { Frequency = 1.0, Velocity = 0.12 }, new() { Frequency = 4.0, Velocity = 0.30 }, new() { Frequency = 10.0, Velocity = 0.50 }, new() { Frequency = 40.0, Velocity = 1.20 }, new() { Frequency = 100.0, Velocity = 2.0 }]
            }
        };
    }

    public MonitorHealthViewModel GetMonitorHealth(int monitorId)
    {
        var labels = Enumerable.Range(0, 30).Select(i => DateTime.UtcNow.AddDays(-29 + i).ToString("MM-dd")).ToList();
        var scores = Enumerable.Range(0, 30).Select(i => 88 + Math.Sin(i / 4.0) * 8 - (i % 11 == 0 ? 12 : 0)).ToList();

        return new MonitorHealthViewModel
        {
            MonitorId = monitorId,
            MonitorName = $"Monitor {monitorId}",
            HealthScore = Math.Round(scores.Last(), 1),
            R2Value = 0.94,
            AxisSlopeRatios = "R:T 1.02, R:V 0.97",
            EnergySymmetryRatio = 0.93,
            TrendLabels = labels,
            TrendScores = scores,
            Anomalies =
            [
                (DateTime.UtcNow.AddDays(-14), "Warning", "Axis symmetry drift"),
                (DateTime.UtcNow.AddDays(-8), "Risk", "Transient baseline deviation"),
                (DateTime.UtcNow.AddDays(-2), "Warning", "R² dip below threshold")
            ]
        };
    }
}
