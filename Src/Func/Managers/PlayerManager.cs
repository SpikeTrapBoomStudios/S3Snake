using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using S3Snake.Func.Rendering;
using S3Snake.GameObjects.Player;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Managers;

public class PlayerManager : Manager
{
    public Vector2D Position;
    public Snake MainSnake => _snake;

    private Snake _snake = null!;
    private bool _initialized;
    private float _targetZoom = 1f;

    public void Init()
    {
        _snake = new Snake();
        _snake.IsPlayer = true;
        _snake.HeadPosition = Groot.WorldCenter;
        _snake.Length = 30;

        _initialized = true;
    }

    public void Reset()
    {
        _snake.FreeFromBuffer();
        Init();
        _targetZoom = 1f;
        Groot.PlayerCamera.Zoom = 1f;
    }

    public override void Update(GameTime gameTime)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("PlayerManager must be initialized before Update.");
        }

        _snake.ToggleSprint(Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed);

        Vector2D centerPos = Position + new Vector2D(S3SnakeGame.GameWidth, S3SnakeGame.GameHeight) / 2 / Groot.PlayerCamera.Zoom;

        float angleToMouse = centerPos.AngleToPoint(Groot.WorldMousePosition);

        _snake.SetRotation(angleToMouse);
        _snake.Update(gameTime);
        
        Position = _snake.HeadPosition - new Vector2D(S3SnakeGame.GameWidth, S3SnakeGame.GameHeight) / 2 / Groot.PlayerCamera.Zoom;
        Groot.PlayerCamera.Position = Position;
        
        _targetZoom = Math.Clamp(
            1f / (1f + (_snake.HeadRadius - 14f) * 0.04f),
            0.25f,
            1f
        );

        float zoomLerpSpeed = 2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Groot.PlayerCamera.Zoom = MathHelper.Lerp(Groot.PlayerCamera.Zoom, _targetZoom, zoomLerpSpeed);
    }
}