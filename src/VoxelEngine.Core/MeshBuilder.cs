namespace VoxelEngine.Core;

using System.Collections.Generic;
using System.Numerics;

public class MeshBuilder
{
    private static readonly Vector3[][] FaceVertices = new Vector3[][]
    {
        new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0),
        }, // back face (negative Z)
        new[]
        {
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
        }, // front face (positive Z)
        new[]
        {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
        }, // left face (negative X)
        new[]
        {
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0),
        }, // right face (positive X)
        new[]
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1),
        }, // top face (positive Y)
        new[]
        {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 0),
        }, // bottom face (negative Y)
    };

    private static readonly uint[] FaceIndices = { 0, 1, 2, 0, 2, 3 };

    public MeshData GenerateMesh(Chunk chunk)
    {
        var vertices = new List<float>();
        var indices = new List<uint>();

        for (var x = 0; x < Chunk.Size; x++)
        for (var y = 0; y < Chunk.Size; y++)
        for (var z = 0; z < Chunk.Size; z++)
        {
            if (chunk.GetVoxel(x, y, z) == 0)
                continue;

            for (var face = 0; face < FaceVertices.Length; face++)
            {
                var neighbor = GetNeighbor(chunk, x, y, z, face);
                if (neighbor != 0)
                    continue;

                var baseIndex = (uint)(vertices.Count / 3);
                foreach (var v in FaceVertices[face])
                {
                    var pos = v + new Vector3(x, y, z);
                    vertices.Add(pos.X);
                    vertices.Add(pos.Y);
                    vertices.Add(pos.Z);
                }

                foreach (var i in FaceIndices)
                {
                    indices.Add(baseIndex + i);
                }
            }
        }

        return new MeshData(vertices.ToArray(), indices.ToArray());
    }

    private static byte GetNeighbor(Chunk chunk, int x, int y, int z, int face)
        => face switch
        {
            0 => z - 1 >= 0 ? chunk.GetVoxel(x, y, z - 1) : (byte)0,
            1 => z + 1 < Chunk.Size ? chunk.GetVoxel(x, y, z + 1) : (byte)0,
            2 => x - 1 >= 0 ? chunk.GetVoxel(x - 1, y, z) : (byte)0,
            3 => x + 1 < Chunk.Size ? chunk.GetVoxel(x + 1, y, z) : (byte)0,
            4 => y + 1 < Chunk.Size ? chunk.GetVoxel(x, y + 1, z) : (byte)0,
            5 => y - 1 >= 0 ? chunk.GetVoxel(x, y - 1, z) : (byte)0,
            _ => 0
        };
}