namespace VoxelEngine.WorldGeneration;

/// <summary>
/// Configuration parameters for world generation.
/// </summary>
public record WorldGenerationConfig
{
    /// <summary>
    /// Random seed for noise generation.
    /// </summary>
    public int Seed { get; init; }

    /// <summary>
    /// Scale factor converting world coordinates to noise space.
    /// </summary>
    public double Scale { get; init; } = 1.0;

    /// <summary>
    /// Maximum height (in world units) for terrain.
    /// </summary>
    public double HeightScale { get; init; } = VoxelEngine.Core.Chunk.Size;

    /// <summary>
    /// Block ID to use for surface layer (e.g., grass).
    /// </summary>
    public byte SurfaceBlockId { get; init; } = 1;

    /// <summary>
    /// Block ID to use for subsurface layer (e.g., dirt).
    /// </summary>
    public byte SubSurfaceBlockId { get; init; } = 2;

    /// <summary>
    /// Block ID to use for under layer (e.g., stone).
    /// </summary>
    public byte UnderBlockId { get; init; } = 3;
}
