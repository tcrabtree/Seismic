using Seismic.UI.Models;
using Seismic.UI.ViewModels;

namespace Seismic.UI.Services;

public interface ISeismicDataService
{
    IReadOnlyList<SiteSummaryViewModel> GetSites();
    SiteEventsPageViewModel? GetSiteEvents(int siteId, string confidence = "All", bool flaggedOnly = false);
    SeismographEventReportViewModel? GetSeismographEventReport(int eventId);
    MonitorHealthViewModel? GetMonitorHealth(int monitorId);
    int AddUploadedEvent(int siteId, string sourceFileName, CsvEventMetadata metadata, string? monitorId, DateTime? eventDate);
}
