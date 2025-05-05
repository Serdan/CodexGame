using Xunit;
using VoxelEngine.WorldGeneration;

namespace VoxelEngine.WorldGeneration.Tests;

public class PerlinNoiseProviderTests
{
    [Fact]
    public void Deterministic_ForSameSeedAndCoordinates()
    {
        var p1 = new PerlinNoiseProvider(42);
        var p2 = new PerlinNoiseProvider(42);
        double v1 = p1.GetNoise(1.234, 5.678, 9.1011);
        double v2 = p2.GetNoise(1.234, 5.678, 9.1011);
        Assert.Equal(v1, v2, 6);
    }

    [Fact]
    public void DifferentSeeds_ProduceDifferentValues()
    {
        var p1 = new PerlinNoiseProvider(42);
        var p2 = new PerlinNoiseProvider(43);
        double v1 = p1.GetNoise(1.234, 5.678, 9.1011);
        double v2 = p2.GetNoise(1.234, 5.678, 9.1011);
        Assert.NotEqual(v1, v2);
    }
}