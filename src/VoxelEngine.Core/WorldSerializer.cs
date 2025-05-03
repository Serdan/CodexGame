namespace VoxelEngine.Core;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public record ChunkData(ChunkPosition Position, byte[] Voxels);
public record WorldData(List<ChunkData> Chunks);

public static class WorldSerializer
{
    /// <summary>
    /// Saves the world to a JSON file at the given path.
    /// </summary>
    public static void Save(World world, string path)
    {
        var chunks = world.GetChunks()
            .Select(c => new ChunkData(c.Position, c.Voxels))
            .ToList();
        var data = new WorldData(chunks);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Loads the world from a JSON file at the given path.
    /// </summary>
    public static World Load(string path)
    {
        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<WorldData>(json)
            ?? throw new JsonException("Failed to deserialize world data.");
        var world = new World();
        foreach (var chunkData in data.Chunks)
        {
            var chunk = Chunk.FromArray(chunkData.Voxels);
            world.AddChunk(chunkData.Position, chunk);
        }
        return world;
    }
}