namespace VoxelEngine.WorldGeneration;

public interface INoiseProvider
{
    /// <summary>
    /// Returns a noise value in [0,1] for the given coordinates.
    /// </summary>
    double GetNoise(double x, double y, double z);
}
