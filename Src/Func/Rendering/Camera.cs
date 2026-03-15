using Microsoft.Xna.Framework;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Rendering;

public class Camera
{
    public Vector2D Position;
    public float Rotation = 0f;
    public float Zoom = 1f;

    public Camera(Vector2D position)
    {
        Position = position;
    }
    
    public Matrix GetMatrix()
    {
        return
            Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1f);
    }
    
    public (int leftBound, int rightBound, int bottomBound, int topBound) GetCameraBounds()
    {
        int leftBound = (int)(Position.X - S3SnakeGame.GameWidth / Zoom);
        int rightBound = (int)(Position.X + S3SnakeGame.GameWidth / Zoom);
        int topBound = (int)(Position.Y - S3SnakeGame.GameHeight / Zoom);
        int bottomBound = (int)(Position.Y + S3SnakeGame.GameHeight / Zoom);
        
        return (leftBound, rightBound, bottomBound, topBound);
    }
}