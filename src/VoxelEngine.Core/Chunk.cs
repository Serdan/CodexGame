namespace VoxelEngine.Core;

using System;

public class Chunk
{
    public const int Size = 16;
    private readonly byte[,,] _voxels = new byte[Size, Size, Size];

    public byte GetVoxel(int x, int y, int z)
    {
        ValidateCoordinates(x, y, z);
        return _voxels[x, y, z];
    }

    public void SetVoxel(int x, int y, int z, byte id)
    {
        ValidateCoordinates(x, y, z);
        _voxels[x, y, z] = id;
    }

    private static void ValidateCoordinates(int x, int y, int z)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size || z < 0 || z >= Size)
            throw new ArgumentOutOfRangeException($"Coordinates out of range: ({x}, {y}, {z})");
    }
    /// <summary>
    /// Returns a flat copy of voxel data in x,y,z order.
    /// </summary>
    public byte[] ToArray()
    {
        var data = new byte[Size * Size * Size];
        var index = 0;
        for (var x = 0; x < Size; x++)
            for (var y = 0; y < Size; y++)
                for (var z = 0; z < Size; z++)
                    data[index++] = _voxels[x, y, z];
        return data;
    }

    /// <summary>
    /// Creates a chunk from flat voxel data array.
    /// </summary>
    public static Chunk FromArray(byte[] data)
    {
        if (data.Length != Size * Size * Size)
            throw new ArgumentException($"Data length must be {Size*Size*Size}", nameof(data));
        var chunk = new Chunk();
        var index = 0;
        for (var x = 0; x < Size; x++)
            for (var y = 0; y < Size; y++)
                for (var z = 0; z < Size; z++)
                    chunk._voxels[x, y, z] = data[index++];
        return chunk;
    }
}