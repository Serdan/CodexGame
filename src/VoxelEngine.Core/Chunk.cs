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
}