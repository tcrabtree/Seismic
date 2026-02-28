using System.ComponentModel.DataAnnotations;

namespace Seismic.UI.Models;

public sealed class GenerateReportRequest
{
    [Required]
    public int SiteId { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    public bool IncludeEventSummary { get; init; } = true;
    public bool IncludeDistanceAnalysis { get; init; } = true;
    public bool IncludeInstrumentHealth { get; init; } = true;
}
