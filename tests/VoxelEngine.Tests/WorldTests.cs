namespace VoxelEngine.Tests;

using VoxelEngine.Core;
using Xunit;

public class WorldTests
{
    [Fact]
    public void DefaultWorld_GetVoxel_Zero()
    {
        var world = new World();
        Assert.Equal<byte>(0, world.GetVoxel(100, 50, -25));
    }

    [Fact]
    public void SetAndGetVoxel_SingleChunk_Works()
    {
        var world = new World();
        world.SetVoxel(1, 2, 3, 7);
        Assert.Equal<byte>(7, world.GetVoxel(1, 2, 3));
    }

    [Fact]
    public void SetAndGetVoxel_DifferentChunk_Works()
    {
        var world = new World();
        int x = Chunk.Size + 1;
        int y = -(Chunk.Size + 2);
        int z = -1;
        world.SetVoxel(x, y, z, 9);
        Assert.Equal<byte>(9, world.GetVoxel(x, y, z));
    }
}