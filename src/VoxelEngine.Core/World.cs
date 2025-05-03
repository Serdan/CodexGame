namespace VoxelEngine.Core;

using System;
using System.Collections.Generic;

public readonly record struct ChunkPosition(int X, int Y, int Z);

/// <summary>
/// Represents a paged collection of voxel chunks organized in a 3D grid.
/// Allows setting and retrieving voxels in world coordinates across multiple chunks.
/// </summary>
public class World
{
    private readonly Dictionary<ChunkPosition, Chunk> _chunks = new();

    /// <summary>
    /// Retrieves an existing chunk at the specified grid position or creates a new one if not present.
    /// </summary>
    /// <param name="pos">Chunk grid position.</param>
    /// <returns>The chunk at the specified position.</returns>
    public Chunk GetOrCreateChunk(ChunkPosition pos)
    {
        if (!_chunks.TryGetValue(pos, out var chunk))
        {
            chunk = new Chunk();
            _chunks[pos] = chunk;
        }
        return chunk;
    }

    /// <summary>
    /// Gets the voxel ID at the given world-space coordinates.
    /// </summary>
    /// <param name="x">World X-coordinate.</param>
    /// <param name="y">World Y-coordinate.</param>
    /// <param name="z">World Z-coordinate.</param>
    /// <returns>The voxel ID at the coordinates.</returns>
    public byte GetVoxel(int x, int y, int z)
    {
        var pos = new ChunkPosition(DivFloor(x, Chunk.Size), DivFloor(y, Chunk.Size), DivFloor(z, Chunk.Size));
        var chunk = GetOrCreateChunk(pos);
        var lx = Mod(x, Chunk.Size);
        var ly = Mod(y, Chunk.Size);
        var lz = Mod(z, Chunk.Size);
        return chunk.GetVoxel(lx, ly, lz);
    }

    /// <summary>
    /// Sets the voxel ID at the given world-space coordinates.
    /// </summary>
    /// <param name="x">World X-coordinate.</param>
    /// <param name="y">World Y-coordinate.</param>
    /// <param name="z">World Z-coordinate.</param>
    /// <param name="id">Voxel ID to set.</param>
    public void SetVoxel(int x, int y, int z, byte id)
    {
        var pos = new ChunkPosition(DivFloor(x, Chunk.Size), DivFloor(y, Chunk.Size), DivFloor(z, Chunk.Size));
        var chunk = GetOrCreateChunk(pos);
        var lx = Mod(x, Chunk.Size);
        var ly = Mod(y, Chunk.Size);
        var lz = Mod(z, Chunk.Size);
        chunk.SetVoxel(lx, ly, lz, id);
    }
    /// <summary>
    /// Enumerates all stored chunks and their flattened voxel data.
    /// </summary>
    /// <summary>
    /// Enumerates all stored chunks and their flattened voxel data.
    /// </summary>
    /// <returns>A sequence of chunk positions with their corresponding voxel arrays.</returns>
    public IEnumerable<(ChunkPosition Position, byte[] Voxels)> GetChunks()
    {
        foreach (var kvp in _chunks)
            yield return (kvp.Key, kvp.Value.ToArray());
    }

    /// <summary>
    /// Adds or replaces a chunk at the given position.
    /// </summary>
    /// <summary>
    /// Adds a chunk at the specified grid position, replacing any existing chunk.
    /// </summary>
    /// <param name="pos">Chunk grid position.</param>
    /// <param name="chunk">Chunk instance to add.</param>
    public void AddChunk(ChunkPosition pos, Chunk chunk)
    {
        _chunks[pos] = chunk;
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