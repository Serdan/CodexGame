namespace VoxelEngine.Core;

using System;

/// <summary>
/// Represents a fixed-size 16×16×16 block of voxels.
/// Provides methods to get, set, and serialize voxel data.
/// </summary>
public class Chunk
{
    public const int Size = 16;
    private readonly byte[,,] _voxels = new byte[Size, Size, Size];

    /// <summary>
    /// Gets the voxel ID at the specified local coordinates within the chunk.
    /// </summary>
    /// <param name="x">X-coordinate (0–15).</param>
    /// <param name="y">Y-coordinate (0–15).</param>
    /// <param name="z">Z-coordinate (0–15).</param>
    /// <returns>The voxel ID at the given coordinates.</returns>
    public byte GetVoxel(int x, int y, int z)
    {
        ValidateCoordinates(x, y, z);
        return _voxels[x, y, z];
    }

    /// <summary>
    /// Sets the voxel ID at the specified local coordinates within the chunk.
    /// </summary>
    /// <param name="x">X-coordinate (0–15).</param>
    /// <param name="y">Y-coordinate (0–15).</param>
    /// <param name="z">Z-coordinate (0–15).</param>
    /// <param name="id">The voxel ID to set.</param>
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
    /// <summary>
    /// Flattens the chunk's voxel data into a linear byte array in X-Y-Z order.
    /// </summary>
    /// <returns>A byte array of length Size³ containing voxel IDs.</returns>
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
    /// <summary>
    /// Creates a new chunk from a flat voxel data array in X-Y-Z order.
    /// </summary>
    /// <param name="data">Flat array of voxel IDs (length must be Size³).</param>
    /// <returns>A Chunk populated with the provided voxel data.</returns>
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