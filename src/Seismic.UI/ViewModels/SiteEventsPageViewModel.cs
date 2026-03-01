namespace Seismic.UI.ViewModels;

public sealed class SiteEventsPageViewModel
{
    public int SiteId { get; init; }
    public string SiteName { get; init; } = string.Empty;
    public string SelectedConfidence { get; init; } = "All";
    public bool ShowOnlyFlagged { get; init; }

    public int TotalEvents { get; init; }
    public double AverageValidityScore { get; init; }
    public int LowConfidenceCount { get; init; }
    public int DistanceDeviationCount { get; init; }

    public IReadOnlyList<EventListItemViewModel> Events { get; init; } = [];
}
