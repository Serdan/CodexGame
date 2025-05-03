namespace VoxelEngine.Core;

using System.Collections.Generic;
using System.Numerics;

public class MeshBuilder
{

    public MeshData GenerateMesh(Chunk chunk)
    {
        var vertices = new List<float>();
        var indices = new List<uint>();
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
                                // Four corners
                                var v0 = new Vector3(xd, yd, zd);
                                var v1 = v0 + du;
                                var v2 = v0 + du + dv;
                                var v3 = v0 + dv;
                                foreach (var vt in new[] { v0, v1, v2, v3 })
                                {
                                    vertices.Add(vt.X);
                                    vertices.Add(vt.Y);
                                    vertices.Add(vt.Z);
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
        return new MeshData(vertices.ToArray(), indices.ToArray());
    }

}