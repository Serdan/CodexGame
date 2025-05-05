using Xunit;
using VoxelEngine.WorldGeneration;
using VoxelEngine.Core;

namespace VoxelEngine.WorldGeneration.Tests;

public class WorldGenerationExtensionsTests
{
    [Fact]
    public void Populate_AddsExpectedChunks()
    {
        var world = new World();
        var config = new WorldGenerationConfig
        {
            Seed = 0,
            Scale = 0,
            HeightScale = 1,
            SurfaceBlockId = 1,
            SubSurfaceBlockId = 2,
            UnderBlockId = 3
        };
        var generator = new NoiseBasedWorldGenerator(config);
        world.Populate(generator, extentX: 1, extentY: 0, extentZ: 1);
        int count = 0;
        foreach (var _ in world.GetChunks())
            count++;
        Assert.Equal(3 * 1 * 3, count);
    }
}