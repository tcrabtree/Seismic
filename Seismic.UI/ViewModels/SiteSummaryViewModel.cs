namespace Seismic.UI.ViewModels;

public sealed class SiteSummaryViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int EventCount { get; init; }
    public int FlaggedCount { get; init; }
    public string OverallHealth { get; init; } = "Healthy";
}
