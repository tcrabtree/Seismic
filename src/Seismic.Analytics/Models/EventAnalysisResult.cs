namespace Seismic.Analytics.Models;

public sealed record EventAnalysisResult(
    double SeismicOnsetSeconds,
    double AcousticOnsetSeconds,
    double DeltaTSeconds,
    double EstimatedDistanceMeters,
    double? DistanceDeviationPercent,
    double ValidBlastProbability,
    string ConfidenceCategory,
    double InstrumentHealthScore
);
