namespace VoxelEngine.WorldGeneration;

using System;

/// <summary>
/// Classic Perlin noise implementation in 3D.
/// Produces values in [0,1].
/// </summary>
public class PerlinNoiseProvider : INoiseProvider
{
    private readonly int[] _perm = new int[512];

    public PerlinNoiseProvider(int seed)
    {
        var random = new Random(seed);
        var p = new int[256];
        for (int i = 0; i < 256; i++)
            p[i] = i;
        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            (p[i], p[j]) = (p[j], p[i]);
        }
        for (int i = 0; i < 512; i++)
            _perm[i] = p[i & 255];
    }

    public double GetNoise(double x, double y, double z)
    {
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;
        int zi = (int)Math.Floor(z) & 255;
        double xf = x - Math.Floor(x);
        double yf = y - Math.Floor(y);
        double zf = z - Math.Floor(z);
        double u = Fade(xf);
        double v = Fade(yf);
        double w = Fade(zf);

        int aaa = _perm[_perm[_perm[xi] + yi] + zi];
        int aba = _perm[_perm[_perm[xi] + yi + 1] + zi];
        int aab = _perm[_perm[_perm[xi] + yi] + zi + 1];
        int abb = _perm[_perm[_perm[xi] + yi + 1] + zi + 1];
        int baa = _perm[_perm[_perm[xi + 1] + yi] + zi];
        int bba = _perm[_perm[_perm[xi + 1] + yi + 1] + zi];
        int bab = _perm[_perm[_perm[xi + 1] + yi] + zi + 1];
        int bbb = _perm[_perm[_perm[xi + 1] + yi + 1] + zi + 1];

        double x1 = Lerp(Grad(aaa, xf, yf, zf), Grad(baa, xf - 1, yf, zf), u);
        double x2 = Lerp(Grad(aba, xf, yf - 1, zf), Grad(bba, xf - 1, yf - 1, zf), u);
        double y1 = Lerp(x1, x2, v);

        double x3 = Lerp(Grad(aab, xf, yf, zf - 1), Grad(bab, xf - 1, yf, zf - 1), u);
        double x4 = Lerp(Grad(abb, xf, yf - 1, zf - 1), Grad(bbb, xf - 1, yf - 1, zf - 1), u);
        double y2 = Lerp(x3, x4, v);

        double result = Lerp(y1, y2, w);
        return (result + 1) / 2;
    }

    private static double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
    private static double Lerp(double a, double b, double t) => a + t * (b - a);
    private static double Grad(int hash, double x, double y, double z)
    {
        int h = hash & 15;
        double u = h < 8 ? x : y;
        double v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
