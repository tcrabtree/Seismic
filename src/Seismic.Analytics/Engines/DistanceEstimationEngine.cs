namespace Seismic.Analytics.Engines;

public sealed class DistanceEstimationEngine
{
    public (double estimatedDistanceMeters, double? deviationPercent) Estimate(
        double deltaTSeconds,
        double speedOfSoundMetersPerSecond = 343.0,
        double? loggedDistanceMeters = null)
    {
        var estimated = Math.Max(0, deltaTSeconds) * speedOfSoundMetersPerSecond;

        if (loggedDistanceMeters is null || loggedDistanceMeters.Value <= 0)
        {
            return (estimated, null);
        }

        var deviation = ((estimated - loggedDistanceMeters.Value) / loggedDistanceMeters.Value) * 100.0;
        return (estimated, deviation);
    }
}
