using Microsoft.AspNetCore.Mvc;
using Seismic.Data.Entities;
using Seismic.Data.Interfaces;
using Seismic.UI.Models;
using Seismic.UI.Services;
using System.Globalization;

namespace Seismic.UI.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventUploadController(
    IEventRepository eventRepository,
    IWaveformRepository waveformRepository,
    ISeismicDataService dataService,
    EventCsvParsingService csvParsingService,
    IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultiple(
        List<IFormFile> files,
        string? monitorId,
        DateTime? eventDate)
    {
        if (files.Count == 0)
        {
            return BadRequest(new[]
            {
                new UploadMultipleResult
                {
                    FileName = string.Empty,
                    Success = false,
                    Error = "No files selected."
                }
            });
        }

        var siteId = ResolveSiteId();
        if (siteId is null)
        {
            return BadRequest(new[]
            {
                new UploadMultipleResult
                {
                    FileName = string.Empty,
                    Success = false,
                    Error = "Site context is missing. Open upload from /Sites/{id}/Events."
                }
            });
        }

        var results = new List<UploadMultipleResult>(files.Count);
        var eventsPath = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", "Events");
        Directory.CreateDirectory(eventsPath);

        foreach (var file in files)
        {
            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(new UploadMultipleResult
                {
                    FileName = file.FileName,
                    Success = false,
                    Error = "Only .csv files are supported."
                });
                continue;
            }

            try
            {
                var (points, metadata, parseError) = await csvParsingService.ParseWithMetadataAsync(file, HttpContext.RequestAborted);
                if (metadata is null || points is null)
                {
                    results.Add(new UploadMultipleResult
                    {
                        FileName = file.FileName,
                        Success = false,
                        Error = parseError ?? "CSV parse failed."
                    });
                    continue;
                }

                var eventGuid = Guid.NewGuid();
                var storedFileName = $"{eventGuid:N}.csv";
                var storedPath = Path.Combine(eventsPath, storedFileName);
                await using (var fileStream = System.IO.File.Create(storedPath))
                {
                    await file.CopyToAsync(fileStream, HttpContext.RequestAborted);
                }

                var monitorValue = string.IsNullOrWhiteSpace(monitorId)
                    ? $"M-{siteId.Value:D2}-UP"
                    : monitorId.Trim();

                var eventEntity = new Event
                {
                    EventId = eventGuid,
                    SiteId = siteId.Value.ToString(CultureInfo.InvariantCulture),
                    MonitorId = monitorValue,
                    EventTimestamp = eventDate ?? DateTime.UtcNow,
                    Distance = Math.Max(1, metadata.OnsetTimeSeconds * 343.0),
                    WeightPerDelay = 0,
                    ScaledDistance = 0,
                    RawCsvPath = Path.Combine("App_Data", "Events", storedFileName),
                    IsReviewed = false,
                    CreatedAt = DateTime.UtcNow,
                    ReviewedAt = null
                };

                await eventRepository.Add(eventEntity);

                var waveformEntities = points.Select(p => new Seismic.Data.Entities.WaveformPoint
                {
                    EventId = eventGuid,
                    Time = p.Time,
                    R = p.R,
                    T = p.T,
                    V = p.V,
                    A = p.A
                });

                await waveformRepository.AddRange(waveformEntities);

                dataService.AddUploadedEvent(siteId.Value, file.FileName, metadata, monitorValue, eventDate);

                results.Add(new UploadMultipleResult
                {
                    FileName = file.FileName,
                    Success = true,
                    EventId = eventGuid.ToString(),
                    Error = null
                });
            }
            catch (Exception ex)
            {
                results.Add(new UploadMultipleResult
                {
                    FileName = file.FileName,
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        return Ok(results);
    }

    private int? ResolveSiteId()
    {
        if (int.TryParse(Request.Query["siteId"], out var directSiteId) && directSiteId > 0)
        {
            return directSiteId;
        }

        var referer = Request.Headers.Referer.ToString();
        if (string.IsNullOrWhiteSpace(referer))
        {
            return null;
        }

        var marker = "/Sites/";
        var start = referer.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
        {
            return null;
        }

        var tail = referer[(start + marker.Length)..];
        var siteSegment = tail.Split(new[] { '/', '?', '#' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        return int.TryParse(siteSegment, out var parsedSiteId) && parsedSiteId > 0
            ? parsedSiteId
            : null;
    }
}
