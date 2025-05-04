namespace VoxelEngine.WorldGeneration;

using VoxelEngine.Core;

/// <summary>
/// Generates voxel chunk data at specified chunk coordinates.
/// </summary>
public interface IWorldGenerator
{
    /// <summary>
    /// Produces a Chunk for the given chunk grid position.
    /// </summary>
    Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ);
}
