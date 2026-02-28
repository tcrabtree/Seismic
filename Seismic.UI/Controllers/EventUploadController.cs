using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Models;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventUploadController(
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
                var (metadata, parseError) = await csvParsingService.ParseAndExtractMetadataAsync(file, HttpContext.RequestAborted);
                if (metadata is null)
                {
                    results.Add(new UploadMultipleResult
                    {
                        FileName = file.FileName,
                        Success = false,
                        Error = parseError ?? "CSV parse failed."
                    });
                    continue;
                }

                var storedFileName = $"{Guid.NewGuid():N}.csv";
                var storedPath = Path.Combine(eventsPath, storedFileName);
                await using (var fileStream = System.IO.File.Create(storedPath))
                {
                    await file.CopyToAsync(fileStream, HttpContext.RequestAborted);
                }

                var eventId = dataService.AddUploadedEvent(siteId.Value, file.FileName, metadata, monitorId, eventDate);

                results.Add(new UploadMultipleResult
                {
                    FileName = file.FileName,
                    Success = true,
                    EventId = eventId,
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
        var siteSegment = tail.Split('/', '?', '#', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        return int.TryParse(siteSegment, out var parsedSiteId) && parsedSiteId > 0
            ? parsedSiteId
            : null;
    }
}
