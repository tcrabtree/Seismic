using CsvHelper;
using CsvHelper.Configuration;
using Seismic.UI.Models;
using System.Globalization;

namespace Seismic.UI.Services;

public sealed class EventCsvParsingService
{
    private static readonly string[] RequiredColumns = ["Time", "R", "T", "V", "A"];

    public async Task<(CsvEventMetadata? Metadata, string? Error)> ParseAndExtractMetadataAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var (points, metadata, error) = await ParseWithMetadataAsync(file, cancellationToken);
        return (metadata, error);
    }

    public async Task<(IReadOnlyList<WaveformPoint>? Points, CsvEventMetadata? Metadata, string? Error)> ParseWithMetadataAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header?.Trim() ?? string.Empty,
            HeaderValidated = null,
            MissingFieldFound = null
        };

        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);

        if (!await csv.ReadAsync() || !csv.ReadHeader())
        {
            return (null, null, "CSV is empty or missing a header row.");
        }

        var headers = csv.HeaderRecord?.Select(h => h.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase)
                      ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var missing = RequiredColumns.Where(col => !headers.Contains(col)).ToArray();
        if (missing.Length > 0)
        {
            return (null, null, $"Missing required columns: {string.Join(",", missing)}");
        }

        var points = new List<WaveformPoint>();

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var point = new WaveformPoint
            {
                Time = csv.GetField<double>("Time"),
                R = csv.GetField<double>("R"),
                T = csv.GetField<double>("T"),
                V = csv.GetField<double>("V"),
                A = csv.GetField<double>("A")
            };

            points.Add(point);
        }

        if (points.Count == 0)
        {
            return (null, null, "CSV contains no waveform samples.");
        }

        var maxAbsR = points.Max(p => Math.Abs(p.R));
        var maxAbsT = points.Max(p => Math.Abs(p.T));
        var maxAbsV = points.Max(p => Math.Abs(p.V));
        var maxAbsA = points.Max(p => Math.Abs(p.A));

        var minTime = points.Min(p => p.Time);
        var maxTime = points.Max(p => p.Time);
        var duration = Math.Max(0, maxTime - minTime);

        var onset = ComputeOnsetTime(points, maxAbsR, maxAbsT, maxAbsV, maxAbsA, minTime);

        return (points, new CsvEventMetadata
        {
            MaxAbsR = maxAbsR,
            MaxAbsT = maxAbsT,
            MaxAbsV = maxAbsV,
            MaxAbsA = maxAbsA,
            DurationSeconds = duration,
            OnsetTimeSeconds = onset
        }, null);
    }

    private static double ComputeOnsetTime(
        IReadOnlyList<WaveformPoint> points,
        double maxAbsR,
        double maxAbsT,
        double maxAbsV,
        double maxAbsA,
        double defaultTime)
    {
        var thresholdR = maxAbsR * 0.05;
        var thresholdT = maxAbsT * 0.05;
        var thresholdV = maxAbsV * 0.05;
        var thresholdA = maxAbsA * 0.05;

        foreach (var point in points.OrderBy(p => p.Time))
        {
            if (Math.Abs(point.R) > thresholdR ||
                Math.Abs(point.T) > thresholdT ||
                Math.Abs(point.V) > thresholdV ||
                Math.Abs(point.A) > thresholdA)
            {
                return point.Time;
            }
        }

        return defaultTime;
    }
}
