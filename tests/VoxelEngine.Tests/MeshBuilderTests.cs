namespace VoxelEngine.Tests;

using VoxelEngine.Core;
using Xunit;

public class MeshBuilderTests
{
    [Fact]
    public void SingleVoxel_ProducesCubeMesh()
    {
        var chunk = new Chunk();
        chunk.SetVoxel(0, 0, 0, 1);
        var mesh = new MeshBuilder().GenerateMesh(chunk);
        // 6 faces, 4 vertices per face = 24 vertices (3 floats each => 72 floats),
        // 6 faces * 2 triangles * 3 indices = 36 indices
        Assert.Equal(72, mesh.Vertices.Length);
        Assert.Equal(36, mesh.Indices.Length);
    }
}