using Seismic.UI.ViewModels;

namespace Seismic.UI.Services;

public sealed class MockSeismicDataService
{
    private readonly List<SiteSummaryViewModel> _sites =
    [
        new() { Id = 1, Name = "North Quarry", EventCount = 42, FlaggedCount = 5, OverallHealth = "Healthy" },
        new() { Id = 2, Name = "East Pit", EventCount = 30, FlaggedCount = 8, OverallHealth = "Warning" },
        new() { Id = 3, Name = "West Bench", EventCount = 18, FlaggedCount = 7, OverallHealth = "Risk" }
    ];

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

    public EventDetailViewModel GetEventDetail(int eventId)
    {
        var seismic = Enumerable.Range(0, 120).Select(i => Math.Sin(i / 8.0) * 0.8 + (i > 34 ? Math.Sin(i / 2.2) * 1.9 : 0)).ToList();
        var acoustic = Enumerable.Range(0, 120).Select(i => Math.Sin(i / 10.0) * 0.2 + (i > 45 ? Math.Sin(i / 2.6) * 1.2 : 0)).ToList();

        return new EventDetailViewModel
        {
            EventId = eventId,
            MonitorName = "M-01-02",
            ValidityScore = 84,
            Confidence = "High",
            DeltaTMilliseconds = 42,
            DistanceMeters = 14.4,
            DistanceErrorBandMeters = 1.8,
            ContributingFactors = ["Consistent onset lag", "Strong axis coherence", "Low baseline noise"],
            PeakPpvR = 2.4,
            PeakPpvT = 2.2,
            PeakPpvV = 2.8,
            RmsEnergy = 1.3,
            SpectralCentroid = 28.5,
            AxisCorrelation = 0.91,
            SeismicOnsetIndex = 34,
            AcousticOnsetIndex = 45,
            SeismicWaveform = seismic,
            AcousticWaveform = acoustic
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
