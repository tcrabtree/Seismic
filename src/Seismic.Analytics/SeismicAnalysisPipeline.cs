using Seismic.Analytics.Engines;
using Seismic.Analytics.Models;

namespace Seismic.Analytics;

public sealed class SeismicAnalysisPipeline
{
    private readonly OnsetDetectionEngine _onset = new();
    private readonly DistanceEstimationEngine _distance = new();
    private readonly EventClassificationEngine _classification = new();
    private readonly InstrumentIntegrityEngine _integrity = new();

    public EventAnalysisResult Analyze(
        IReadOnlyList<EventSample> samples,
        int preTriggerSamples,
        double speedOfSoundMetersPerSecond = 343.0,
        double? loggedDistanceMeters = null)
    {
        var onset = _onset.DetectOnsets(samples, preTriggerSamples);
        var peakAmplitude = samples.Max(s => Math.Max(Math.Abs(s.R), Math.Max(Math.Abs(s.T), Math.Abs(s.V))));
        var rms = Math.Sqrt(samples.Average(s => (s.R * s.R + s.T * s.T + s.V * s.V) / 3.0));
        var coherence = ComputeSimpleCoherence(samples);

        var classification = _classification.Classify(onset.deltaTSeconds, peakAmplitude, rms, coherence);
        var distance = _distance.Estimate(onset.deltaTSeconds, speedOfSoundMetersPerSecond, loggedDistanceMeters);
        var integrityScore = _integrity.ComputeHealthScore(r2: coherence, axisSymmetryRatio: 1.0, baselineDrift: 0.0);

        return new EventAnalysisResult(
            onset.seismicOnsetSeconds,
            onset.acousticOnsetSeconds,
            onset.deltaTSeconds,
            distance.estimatedDistanceMeters,
            distance.deviationPercent,
            classification.probability,
            classification.confidenceCategory,
            integrityScore);
    }

    private static double ComputeSimpleCoherence(IReadOnlyList<EventSample> samples)
    {
        if (samples.Count < 2)
        {
            return 0.0;
        }

        var r = samples.Select(s => s.R).ToArray();
        var v = samples.Select(s => s.V).ToArray();

        var rMean = r.Average();
        var vMean = v.Average();

        var numerator = 0.0;
        var rDen = 0.0;
        var vDen = 0.0;

        for (var i = 0; i < r.Length; i++)
        {
            var rd = r[i] - rMean;
            var vd = v[i] - vMean;
            numerator += rd * vd;
            rDen += rd * rd;
            vDen += vd * vd;
        }

        var denominator = Math.Sqrt(rDen * vDen);
        if (denominator <= 0)
        {
            return 0.0;
        }

        return Math.Clamp(Math.Abs(numerator / denominator), 0.0, 1.0);
    }
}
