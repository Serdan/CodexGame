namespace VoxelEngine.Core;

/// <summary>
/// Holds mesh buffer data generated from a voxel chunk.
/// </summary>
/// <param name="Vertices">Flat array of vertex positions (XYZ triples).</param>
/// <param name="Indices">Array of indices defining mesh triangles.</param>
/// <param name="Normals">Flat array of normal vectors corresponding to each vertex.</param>
/// <param name="AmbientOcclusion">Flat array of ambient occlusion factors per vertex (0.0–1.0).</param>
public record MeshData(float[] Vertices, uint[] Indices, float[] Normals, float[] AmbientOcclusion);