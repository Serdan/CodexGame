namespace VoxelEngine.Tests;

using System;
using System.IO;
using VoxelEngine.Core;
using Xunit;

public class WorldSerializationTests
{
    [Fact]
    public void SaveAndLoad_World_PreservesVoxels()
    {
        var world = new World();
        // Set voxels across chunk boundaries
        world.SetVoxel(0, 0, 0, 1);
        world.SetVoxel(Chunk.Size - 1, Chunk.Size - 1, Chunk.Size - 1, 2);
        world.SetVoxel(Chunk.Size, 0, 0, 3);

        var tempPath = Path.GetTempFileName();
        try
        {
            WorldSerializer.Save(world, tempPath);
            var loaded = WorldSerializer.Load(tempPath);
            Assert.Equal<byte>(1, loaded.GetVoxel(0, 0, 0));
            Assert.Equal<byte>(2, loaded.GetVoxel(Chunk.Size - 1, Chunk.Size - 1, Chunk.Size - 1));
            Assert.Equal<byte>(3, loaded.GetVoxel(Chunk.Size, 0, 0));
            // Unset voxel remains zero
            Assert.Equal<byte>(0, loaded.GetVoxel(1, 1, 1));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}