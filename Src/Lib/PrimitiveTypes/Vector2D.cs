using System;
using System.Numerics;

namespace S3Snake.Lib.PrimitiveTypes;

public struct Vector2D
{
    public static readonly Vector2D Zero = new Vector2D(0, 0);
    public static readonly Vector2D One = new Vector2D(1, 1);
    public static readonly Vector2D Up = new Vector2D(0, -1);
    public static readonly Vector2D Down = new Vector2D(0, 1);
    public static readonly Vector2D Left = new Vector2D(-1, 0);
    public static readonly Vector2D Right = new Vector2D(1, 0);
    
    public float X;
    public float Y;

    public Vector2D(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public override string ToString() => $"({X}, {Y})";

    public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
    public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);
    public static Vector2D operator *(Vector2D a, Vector2D b) => new Vector2D(a.X * b.X, a.Y * b.Y);
    public static Vector2D operator *(Vector2D a, float b) => new Vector2D(a.X * b, a.Y * b);
    public static Vector2D operator *(Vector2D a, double b) => new Vector2D(a.X * (float)b, a.Y * (float)b);
    public static Vector2D operator /(Vector2D a, float b) => new Vector2D(a.X / b, a.Y / b);
    /// <summary>
    /// Returns true if the two vectors are APPROXIMATELY equal.
    /// </summary>
    public static bool operator ==(Vector2D a, Vector2D b) => Math2.ApproxEqual(a.X, b.X) && Math2.ApproxEqual(a.Y, b.Y);
    /// <summary>
    /// Returns true if the two vectors are APPROXIMATELY unequal.
    /// </summary>
    public static bool operator !=(Vector2D a, Vector2D b) => !Math2.ApproxEqual(a.X, b.X) || !Math2.ApproxEqual(a.Y, b.Y);
    
    public readonly float AngleToPoint(Vector2D point)
    {
        float dx = point.X - X;
        float dy = point.Y - Y;

        return MathF.Atan2(dy, dx);
    }
    
    public readonly Vector2D Rotated(float rotation)
    {
        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);

        float x = X * cos - Y * sin;
        float y = X * sin + Y * cos;

        return new Vector2D(x, y);
    }
    
    public readonly Vector2D MoveToward(Vector2D target, float maxDelta)
    {
        Vector2D diff = target - this;
        float dist = diff.Length();
        
        if (dist <= maxDelta || dist == 0f) return target;
        
        return this + (diff / dist) * maxDelta;
    }
    
    public readonly Vector2D Lerp(Vector2D to, float percent)
    {
        return Lerp(this, to, percent);
    }
    
    public static Vector2D Lerp(Vector2D from, Vector2D to, float percent)
    {
        percent = Math.Clamp(percent, 0f, 1f);
        return new Vector2D(from.X + (to.X - from.X) * percent, from.Y + (to.Y - from.Y) * percent);
    }
    
    public readonly Vector2D Normalized()
    {
        float len = Length();
        if (len == 0f) return Zero;
        return new Vector2D(X / len, Y / len);
    }
    
    public float Length() => (float) Math.Sqrt(X * X + Y * Y);
    public float LengthSquared() => X * X + Y * Y;

    public readonly Vector2D Abs()
    {
        return new Vector2D(Math.Abs(X), Math.Abs(Y));
    }
    
    public static implicit operator Microsoft.Xna.Framework.Vector2(Vector2D vector2D) => new Microsoft.Xna.Framework.Vector2(vector2D.X, vector2D.Y);
    public static implicit operator Vector2D(Microsoft.Xna.Framework.Vector2 vector2) => new Vector2D(vector2.X, vector2.Y);
    public static implicit operator Vector2D(Vector2 vector2) => new Vector2D(vector2.X, vector2.Y);
    public static implicit operator Vector2(Vector2D vector2) => new Vector2(vector2.X, vector2.Y);
}