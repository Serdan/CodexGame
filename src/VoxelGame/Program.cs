using System;
using VoxelEngine.Core;

namespace VoxelGame;

class Program
{
    static void Main()
    {
        // Create a small 2x2x2 voxel block
        var chunk = new Chunk();
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            chunk.SetVoxel(x, y, z, 1);

        var meshBuilder = new MeshBuilder();
        GameEngine engine = null!;
        engine = new GameEngine(
            update =>
            {
                // Exit on ESC key
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    engine.Stop();
            },
            () =>
            {
                var mesh = meshBuilder.GenerateMesh(chunk);
                Console.Clear();
                Console.WriteLine("Voxel Game - Mesh Info");
                Console.WriteLine($"Vertices: {mesh.Vertices.Length}");
                Console.WriteLine($"Indices: {mesh.Indices.Length}");
                Console.WriteLine("Press ESC to exit.");
            });

        Console.WriteLine("Starting Voxel Game. Press ESC to exit.");
        engine.Run();
    }
}
