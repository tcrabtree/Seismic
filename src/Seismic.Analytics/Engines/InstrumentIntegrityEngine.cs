namespace Seismic.Analytics.Engines;

public sealed class InstrumentIntegrityEngine
{
    public double ComputeHealthScore(double r2, double axisSymmetryRatio, double baselineDrift)
    {
        var r2Score = Math.Clamp(r2, 0.0, 1.0) * 100.0;
        var symmetryPenalty = Math.Min(Math.Abs(1.0 - axisSymmetryRatio) * 60.0, 60.0);
        var driftPenalty = Math.Min(Math.Abs(baselineDrift) * 80.0, 40.0);

        var score = r2Score - symmetryPenalty - driftPenalty;
        return Math.Clamp(score, 0.0, 100.0);
    }
}
