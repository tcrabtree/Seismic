namespace Seismic.Analytics.Engines;

public sealed class EventClassificationEngine
{
    public (double probability, string confidenceCategory) Classify(
        double deltaTSeconds,
        double peakAmplitude,
        double rmsEnergy,
        double axisCoherence)
    {
        // Lightweight baseline logistic-style score for initial integration scaffolding.
        var linearScore =
            1.8 * Normalize(deltaTSeconds, 0.0, 1.0) +
            1.2 * Normalize(peakAmplitude, 0.0, 10.0) +
            1.1 * Normalize(rmsEnergy, 0.0, 10.0) +
            1.4 * Normalize(axisCoherence, 0.0, 1.0) -
            2.4;

        var probability = 1.0 / (1.0 + Math.Exp(-linearScore));

        var category = probability switch
        {
            >= 0.80 => "High",
            >= 0.60 => "Medium",
            _ => "Low"
        };

        return (probability, category);
    }

    private static double Normalize(double value, double min, double max)
    {
        if (max <= min)
        {
            return 0.0;
        }

        var normalized = (value - min) / (max - min);
        return Math.Clamp(normalized, 0.0, 1.0);
    }
}
