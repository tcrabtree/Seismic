using Seismic.Analytics.Models;

namespace Seismic.Analytics.Engines;

public sealed class OnsetDetectionEngine
{
    public (double seismicOnsetSeconds, double acousticOnsetSeconds, double deltaTSeconds) DetectOnsets(
        IReadOnlyList<EventSample> samples,
        int preTriggerSamples,
        double k = 4.0,
        int sustainedSamples = 5)
    {
        if (samples.Count < 2)
        {
            throw new ArgumentException("At least 2 samples are required.", nameof(samples));
        }

        if (sustainedSamples <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sustainedSamples), "Sustained sample window must be greater than 0.");
        }

        if (k < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(k), "Threshold multiplier k must be non-negative.");
        }

        preTriggerSamples = Math.Clamp(preTriggerSamples, 1, samples.Count - 1);

        var seismicSeries = samples.Select(s => Math.Max(Math.Abs(s.R), Math.Max(Math.Abs(s.T), Math.Abs(s.V)))).ToArray();
        var acousticSeries = samples.Select(s => Math.Abs(s.A)).ToArray();

        var seismicOnset = DetectFirstSustainedCrossing(samples, seismicSeries, preTriggerSamples, k, sustainedSamples);
        var acousticOnset = DetectFirstSustainedCrossing(samples, acousticSeries, preTriggerSamples, k, sustainedSamples);

        return (seismicOnset, acousticOnset, acousticOnset - seismicOnset);
    }

    private static double DetectFirstSustainedCrossing(
        IReadOnlyList<EventSample> samples,
        IReadOnlyList<double> signal,
        int preTriggerSamples,
        double k,
        int sustainedSamples)
    {
        var baseline = signal.Take(preTriggerSamples).ToArray();
        var mean = baseline.Average();
        var variance = baseline.Select(v => (v - mean) * (v - mean)).Average();
        var stdDev = Math.Sqrt(variance);
        var threshold = mean + k * stdDev;

        for (var i = preTriggerSamples; i <= signal.Count - sustainedSamples; i++)
        {
            var sustained = true;
            for (var j = 0; j < sustainedSamples; j++)
            {
                if (signal[i + j] <= threshold)
                {
                    sustained = false;
                    break;
                }
            }

            if (sustained)
            {
                return samples[i].TimeSeconds;
            }
        }

        return samples[^1].TimeSeconds;
    }
}
