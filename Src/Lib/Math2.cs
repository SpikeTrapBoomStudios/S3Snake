using System;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Lib;

public class Math2
{
    public static bool ApproxEqual(float a, float b, float maxDiff = 0.0001f)
    {
        return Math.Abs(a - b) < maxDiff;
    }

    public static bool ApproxEqual(Vector2D a, Vector2D b, float maxDiff)
    {
        return ApproxEqual(a.X, b.X, maxDiff) && ApproxEqual(a.Y, b.Y, maxDiff);
    }
    
    public static Vector2D RandomPointInCircle(Vector2D center, float radius)
    {
        Random rng = new();
        float angle = rng.NextSingle() * MathF.PI * 2f;
        float dist  = MathF.Sqrt(rng.NextSingle()) * radius;

        return new Vector2D(
            center.X + MathF.Cos(angle) * dist,
            center.Y + MathF.Sin(angle) * dist
        );
    }
}