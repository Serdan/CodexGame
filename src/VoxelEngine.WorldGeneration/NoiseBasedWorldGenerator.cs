namespace VoxelEngine.WorldGeneration;

using VoxelEngine.Core;
using System.Collections.Generic;

/// <summary>
/// Default world generator that uses layered noise to build simple terrain.
/// </summary>
public class NoiseBasedWorldGenerator : IWorldGenerator
{
    private readonly WorldGenerationConfig _config;
    private readonly INoiseProvider _noise;

    public NoiseBasedWorldGenerator(WorldGenerationConfig config)
    {
        _config = config;
        // Example layered noise: one octave Perlin; can add more layers.
        _noise = new LayeredNoiseProvider(new List<(INoiseProvider, double)>
        {
            (new PerlinNoiseProvider(config.Seed), 1.0)
        });
    }

    public Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ)
    {
        var chunk = new Chunk();
        int size = Chunk.Size;
        for (int x = 0; x < size; x++)
        for (int z = 0; z < size; z++)
        {
            double worldX = (chunkX * size + x) * _config.Scale;
            double worldZ = (chunkZ * size + z) * _config.Scale;
            double noiseVal = _noise.GetNoise(worldX, 0, worldZ);
            int height = (int)(noiseVal * _config.HeightScale);
            for (int y = 0; y < size; y++)
            {
                int worldY = chunkY * size + y;
                byte id = 0;
                if (worldY <= height)
                {
                    if (worldY == height)
                        id = _config.SurfaceBlockId;
                    else if (worldY >= height - 2)
                        id = _config.SubSurfaceBlockId;
                    else
                        id = _config.UnderBlockId;
                }
                chunk.SetVoxel(x, y, z, id);
            }
        }
        return chunk;
    }
}
