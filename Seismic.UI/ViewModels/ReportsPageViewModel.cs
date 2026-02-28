namespace Seismic.UI.ViewModels;

public sealed class ReportsPageViewModel
{
    public IReadOnlyList<SiteSummaryViewModel> Sites { get; init; } = [];
}
