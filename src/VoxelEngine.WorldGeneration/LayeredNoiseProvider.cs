namespace VoxelEngine.WorldGeneration;

using System.Collections.Generic;

/// <summary>
/// Combines multiple INoiseProvider instances with weights into a single noise output.
/// </summary>
public class LayeredNoiseProvider : INoiseProvider
{
    private readonly List<(INoiseProvider Provider, double Weight)> _layers;
    private readonly double _totalWeight;

    public LayeredNoiseProvider(IEnumerable<(INoiseProvider Provider, double Weight)> layers)
    {
        _layers = new List<(INoiseProvider, double)>(layers);
        double sum = 0;
        foreach (var layer in _layers)
            sum += layer.Weight;
        _totalWeight = sum > 0 ? sum : 1;
    }

    public double GetNoise(double x, double y, double z)
    {
        double accum = 0;
        foreach (var (prov, weight) in _layers)
            accum += prov.GetNoise(x, y, z) * weight;
        return accum / _totalWeight;
    }
}
