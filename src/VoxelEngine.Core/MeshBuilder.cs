namespace VoxelEngine.Core;

using System.Collections.Generic;
using System.Numerics;

public class MeshBuilder
{

    public MeshData GenerateMesh(Chunk chunk)
    {
        var vertices = new List<float>();
        var indices = new List<uint>();
        var normals = new List<float>();
        var aos = new List<float>();
        int size = Chunk.Size;

        // Greedy meshing algorithm
        for (int d = 0; d < 3; d++)
        {
            int u = (d + 1) % 3;
            int v = (d + 2) % 3;
            var mask = new byte[size, size];

            for (int side = 0; side < 2; side++)
            {
                for (int w = 0; w < size; w++)
                {
                    // Build visibility mask
                    for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                    {
                        int x = 0, y = 0, z = 0;
                        switch (d) { case 0: x = w; y = i; z = j; break; case 1: x = i; y = w; z = j; break; default: x = i; y = j; z = w; break; }
                        byte a = chunk.GetVoxel(x, y, z);
                        byte b;
                        if (side == 0)
                        {
                            // negative side neighbor
                            int nx = x - (d == 0 ? 1 : 0);
                            int ny = y - (d == 1 ? 1 : 0);
                            int nz = z - (d == 2 ? 1 : 0);
                            b = (nx >= 0 && ny >= 0 && nz >= 0) ? chunk.GetVoxel(nx, ny, nz) : (byte)0;
                        }
                        else
                        {
                            // positive side neighbor
                            int nx = x + (d == 0 ? 1 : 0);
                            int ny = y + (d == 1 ? 1 : 0);
                            int nz = z + (d == 2 ? 1 : 0);
                            b = (nx < size && ny < size && nz < size) ? chunk.GetVoxel(nx, ny, nz) : (byte)0;
                        }
                        mask[i, j] = (byte)((a != 0 && b == 0) ? a : 0);
                    }

                    // Greedy merge rectangles in mask
                    for (int j = 0; j < size; j++)
                    {
                        for (int i = 0; i < size; )
                        {
                            byte c = mask[i, j];
                            if (c != 0)
                            {
                                // Compute width
                                int wlen = 1;
                                while (i + wlen < size && mask[i + wlen, j] == c) wlen++;
                                // Compute height
                                int hlen = 1;
                                bool done = false;
                                while (j + hlen < size && !done)
                                {
                                    for (int k = 0; k < wlen; k++)
                                    {
                                        if (mask[i + k, j + hlen] != c) { done = true; break; }
                                    }
                                    if (!done) hlen++;
                                }

                                // Add quad
                                int xd, yd, zd;
                                if (d == 0) { xd = (side == 0 ? w : w + 1); yd = i; zd = j; }
                                else if (d == 1) { xd = i; yd = (side == 0 ? w : w + 1); zd = j; }
                                else { xd = i; yd = j; zd = (side == 0 ? w : w + 1); }
                                // Direction vectors
                                var du = Vector3.Zero;
                                var dv = Vector3.Zero;
                                if (u == 0) du.X = wlen;
                                else if (u == 1) du.Y = wlen;
                                else du.Z = wlen;
                                if (v == 0) dv.X = hlen;
                                else if (v == 1) dv.Y = hlen;
                                else dv.Z = hlen;

                                var baseIndex = (uint)(vertices.Count / 3);
                                // Determine normal direction for this face
                                var axis = d == 0 ? Vector3.UnitX : d == 1 ? Vector3.UnitY : Vector3.UnitZ;
                                var normalVec = side == 0 ? -axis : axis;
                                // Four corners
                                var v0 = new Vector3(xd, yd, zd);
                                var v1 = v0 + du;
                                var v2 = v0 + du + dv;
                                var v3 = v0 + dv;
                                foreach (var vt in new[] { v0, v1, v2, v3 })
                                {
                                    // Vertex position
                                    vertices.Add(vt.X);
                                    vertices.Add(vt.Y);
                                    vertices.Add(vt.Z);
                                    // Normal
                                    normals.Add(normalVec.X);
                                    normals.Add(normalVec.Y);
                                    normals.Add(normalVec.Z);
                                    // Ambient Occlusion (based on neighboring voxels)
                                    int xi = (int)vt.X;
                                    int yi = (int)vt.Y;
                                    int zi = (int)vt.Z;
                                    // Axis offsets
                                    int ox_u = u == 0 ? 1 : 0;
                                    int oy_u = u == 1 ? 1 : 0;
                                    int oz_u = u == 2 ? 1 : 0;
                                    int ox_v = v == 0 ? 1 : 0;
                                    int oy_v = v == 1 ? 1 : 0;
                                    int oz_v = v == 2 ? 1 : 0;
                                    // Sample neighbors
                                    int occ_u = 0;
                                    if (xi - ox_u >= 0 && yi - oy_u >= 0 && zi - oz_u >= 0 && xi - ox_u < size && yi - oy_u < size && zi - oz_u < size)
                                        occ_u = chunk.GetVoxel(xi - ox_u, yi - oy_u, zi - oz_u) != 0 ? 1 : 0;
                                    int occ_v = 0;
                                    if (xi - ox_v >= 0 && yi - oy_v >= 0 && zi - oz_v >= 0 && xi - ox_v < size && yi - oy_v < size && zi - oz_v < size)
                                        occ_v = chunk.GetVoxel(xi - ox_v, yi - oy_v, zi - oz_v) != 0 ? 1 : 0;
                                    int occ_uv = 0;
                                    if (xi - ox_u - ox_v >= 0 && yi - oy_u - oy_v >= 0 && zi - oz_u - oz_v >= 0 && xi - ox_u - ox_v < size && yi - oy_u - oy_v < size && zi - oz_u - oz_v < size)
                                        occ_uv = chunk.GetVoxel(xi - ox_u - ox_v, yi - oy_u - oy_v, zi - oz_u - oz_v) != 0 ? 1 : 0;
                                    float ao = 1f - (occ_u + occ_v + occ_uv) / 3f;
                                    aos.Add(ao);
                                }
                                // Two triangles
                                indices.Add(baseIndex);
                                indices.Add(baseIndex + 1);
                                indices.Add(baseIndex + 2);
                                indices.Add(baseIndex);
                                indices.Add(baseIndex + 2);
                                indices.Add(baseIndex + 3);

                                // Zero-out mask
                                for (int l = 0; l < hlen; l++)
                                    for (int k = 0; k < wlen; k++)
                                        mask[i + k, j + l] = 0;

                                i += wlen;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
            }
        }
        return new MeshData(vertices.ToArray(), indices.ToArray(), normals.ToArray(), aos.ToArray());
    }

}