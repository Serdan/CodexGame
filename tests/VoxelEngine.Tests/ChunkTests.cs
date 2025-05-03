namespace VoxelEngine.Tests;

using System;
using VoxelEngine.Core;
using Xunit;

public class ChunkTests
{
    [Fact]
    public void NewChunk_HasAllZeroVoxels()
    {
        var chunk = new Chunk();
        for (var x = 0; x < Chunk.Size; x++)
        for (var y = 0; y < Chunk.Size; y++)
        for (var z = 0; z < Chunk.Size; z++)
        {
            Assert.Equal(0, chunk.GetVoxel(x, y, z));
        }
    }

    [Fact]
    public void SetAndGetVoxel_WorksCorrectly()
    {
        var chunk = new Chunk();
        chunk.SetVoxel(1, 2, 3, 5);
        Assert.Equal<byte>(5, chunk.GetVoxel(1, 2, 3));
    }

    [Fact]
    public void SetVoxel_OutOfRange_Throws()
    {
        var chunk = new Chunk();
        Assert.Throws<ArgumentOutOfRangeException>(() => chunk.SetVoxel(-1, 0, 0, 1));
    }
}