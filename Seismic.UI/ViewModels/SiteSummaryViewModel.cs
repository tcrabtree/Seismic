namespace Seismic.UI.ViewModels;

public sealed class SiteSummaryViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int EventCount { get; set; }
    public int FlaggedCount { get; set; }
    public string OverallHealth { get; init; } = "Healthy";
}
