using Xunit;
using VoxelEngine.WorldGeneration;
using VoxelEngine.Core;

namespace VoxelEngine.WorldGeneration.Tests;

public class NoiseBasedWorldGeneratorTests
{
    [Fact]
    public void GenerateChunk_ScaleZero_ProducesFlatSurface()
    {
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
        var chunk = generator.GenerateChunk(0, 0, 0);
        for (int x = 0; x < Chunk.Size; x++)
        for (int z = 0; z < Chunk.Size; z++)
        {
            Assert.Equal(config.SurfaceBlockId, chunk.GetVoxel(x, 0, z));
            for (int y = 1; y < Chunk.Size; y++)
                Assert.Equal((byte)0, chunk.GetVoxel(x, y, z));
        }
    }

    [Fact]
    public void GenerateChunk_HeightScale3_VerifyBlockAssignments()
    {
        var config = new WorldGenerationConfig
        {
            Seed = 0,
            Scale = 0,
            HeightScale = 3,
            SurfaceBlockId = 1,
            SubSurfaceBlockId = 2,
            UnderBlockId = 3
        };
        var generator = new NoiseBasedWorldGenerator(config);
        var chunk = generator.GenerateChunk(0, 0, 0);
        for (int x = 0; x < Chunk.Size; x++)
        for (int z = 0; z < Chunk.Size; z++)
        {
            Assert.Equal((byte)1, chunk.GetVoxel(x, 1, z));
            Assert.Equal((byte)2, chunk.GetVoxel(x, 0, z));
            for (int y = 2; y < Chunk.Size; y++)
                Assert.Equal((byte)0, chunk.GetVoxel(x, y, z));
        }
    }
}