namespace Seismic.Analytics.Models;

public sealed record EventSample(
    double TimeSeconds,
    double R,
    double T,
    double V,
    double A
);
