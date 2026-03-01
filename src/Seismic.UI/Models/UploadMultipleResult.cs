namespace Seismic.UI.Models;

public sealed class UploadMultipleResult
{
    public string FileName { get; init; } = string.Empty;
    public bool Success { get; init; }
    public string? EventId { get; init; }
    public string? Error { get; init; }
}
