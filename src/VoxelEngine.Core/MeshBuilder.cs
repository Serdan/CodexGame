namespace VoxelEngine.Core;

using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// Builds optimized mesh data from voxel chunks using a greedy meshing algorithm.
/// Generates vertex positions, indices, normals, and ambient occlusion values.
/// </summary>
public class MeshBuilder
{

    /// <summary>
    /// Generates mesh data for the given voxel chunk.
    /// </summary>
    /// <param name="chunk">The voxel chunk to mesh.</param>
    /// <returns>A MeshData record containing vertices, indices, normals, and ambient occlusion.</returns>
    public MeshData GenerateMesh(Chunk chunk)
    {
        var vertices = new List<float>();
        var indices = new List<uint>();
        var normals = new List<float>();
        var aos = new List<float>();
        var colors = new List<float>();
        int size = Chunk.Size;

        // For each voxel, generate quads for exposed faces
        for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
        for (int z = 0; z < size; z++)
        {
            byte id = chunk.GetVoxel(x, y, z);
            if (id == 0) continue;
            // Choose color by ID
            Vector3 col = id switch
            {
                1 => new Vector3(0.2f, 0.8f, 0.2f),
                2 => new Vector3(0.6f, 0.4f, 0.2f),
                3 => new Vector3(0.5f, 0.5f, 0.5f),
                _ => new Vector3(1f, 1f, 1f)
            };
            // Directions: 0:-Z,1:+Z,2:-X,3:+X,4:+Y,5:-Y
            Vector3[] normalsLookup = new Vector3[] { new Vector3(0,0,-1), new Vector3(0,0,1), new Vector3(-1,0,0), new Vector3(1,0,0), new Vector3(0,1,0), new Vector3(0,-1,0) };
            Vector3[][] verticesLookup = new Vector3[][]
            {
                new Vector3[]{ new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(0,1,0) },
                new Vector3[]{ new Vector3(1,0,1), new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(1,1,1) },
                new Vector3[]{ new Vector3(0,0,1), new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(0,1,1) },
                new Vector3[]{ new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(1,1,0) },
                new Vector3[]{ new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(1,1,1), new Vector3(0,1,1) },
                new Vector3[]{ new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(1,0,0), new Vector3(0,0,0) }
            };
            for (int d = 0; d < 6; d++)
            {
                var nrm = normalsLookup[d];
                int nx = x + (int)nrm.X;
                int ny = y + (int)nrm.Y;
                int nz = z + (int)nrm.Z;
                byte neighbor = (nx>=0&&ny>=0&&nz>=0&&nx<size&&ny<size&&nz<size)
                                ? chunk.GetVoxel(nx,ny,nz) : (byte)0;
                if (neighbor != 0) continue;
                uint baseIndex = (uint)(vertices.Count/3);
                // Add quad
                foreach (var vo in verticesLookup[d])
                {
                    var vpos = vo + new Vector3(x,y,z);
                    vertices.Add(vpos.X); vertices.Add(vpos.Y); vertices.Add(vpos.Z);
                    normals.Add(nrm.X); normals.Add(nrm.Y); normals.Add(nrm.Z);
                    aos.Add(1f);
                    colors.Add(col.X); colors.Add(col.Y); colors.Add(col.Z);
                }
                // Indices (two triangles)
                indices.Add(baseIndex); indices.Add(baseIndex+1); indices.Add(baseIndex+2);
                indices.Add(baseIndex); indices.Add(baseIndex+2); indices.Add(baseIndex+3);
            }
        }
        return new MeshData(vertices.ToArray(), indices.ToArray(), normals.ToArray(), aos.ToArray(), colors.ToArray());
    }
}