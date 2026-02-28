using System.ComponentModel.DataAnnotations;

namespace Seismic.UI.Models;

public sealed class SaveOverrideRequest
{
    [Required]
    [RegularExpression("^(Valid|False|Needs Review)$")]
    public string Decision { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; init; }
}
