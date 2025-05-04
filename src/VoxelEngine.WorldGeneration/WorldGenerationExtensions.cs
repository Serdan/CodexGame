namespace VoxelEngine.WorldGeneration;

using VoxelEngine.Core;

/// <summary>
/// Provides extension methods for populating a Core.World using a world generator.
/// </summary>
public static class WorldGenerationExtensions
{
    public static void Populate(this World world, IWorldGenerator generator, int extentX, int extentY, int extentZ)
    {
        for (int cx = -extentX; cx <= extentX; cx++)
        for (int cy = -extentY; cy <= extentY; cy++)
        for (int cz = -extentZ; cz <= extentZ; cz++)
        {
            var chunk = generator.GenerateChunk(cx, cy, cz);
            world.AddChunk(new ChunkPosition(cx, cy, cz), chunk);
        }
    }
}
