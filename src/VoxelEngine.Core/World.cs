namespace VoxelEngine.Core;

using System;
using System.Collections.Generic;

public readonly record struct ChunkPosition(int X, int Y, int Z);

public class World
{
    private readonly Dictionary<ChunkPosition, Chunk> _chunks = new();

    public Chunk GetOrCreateChunk(ChunkPosition pos)
    {
        if (!_chunks.TryGetValue(pos, out var chunk))
        {
            chunk = new Chunk();
            _chunks[pos] = chunk;
        }
        return chunk;
    }

    public byte GetVoxel(int x, int y, int z)
    {
        var pos = new ChunkPosition(DivFloor(x, Chunk.Size), DivFloor(y, Chunk.Size), DivFloor(z, Chunk.Size));
        var chunk = GetOrCreateChunk(pos);
        var lx = Mod(x, Chunk.Size);
        var ly = Mod(y, Chunk.Size);
        var lz = Mod(z, Chunk.Size);
        return chunk.GetVoxel(lx, ly, lz);
    }

    public void SetVoxel(int x, int y, int z, byte id)
    {
        var pos = new ChunkPosition(DivFloor(x, Chunk.Size), DivFloor(y, Chunk.Size), DivFloor(z, Chunk.Size));
        var chunk = GetOrCreateChunk(pos);
        var lx = Mod(x, Chunk.Size);
        var ly = Mod(y, Chunk.Size);
        var lz = Mod(z, Chunk.Size);
        chunk.SetVoxel(lx, ly, lz, id);
    }

    private static int DivFloor(int a, int b)
    {
        var div = a / b;
        if ((a ^ b) < 0 && a % b != 0)
            div--;
        return div;
    }

    private static int Mod(int a, int b)
    {
        var m = a % b;
        if (m < 0) m += b;
        return m;
    }
}