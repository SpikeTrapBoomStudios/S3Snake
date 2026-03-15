using System;
using Microsoft.Xna.Framework;
using S3Snake.GameObjects.Player;
using S3Snake.Lib;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.GameObjects.Player;

public class AiSnake : Snake
{
    private float _timer = 0f;
    private float _sprintTimer = 0f;
    private Random _random = new Random();
    
    public AiSnake()
    {
        IsPlayer = false;
        HeadPosition = Math2.RandomPointInCircle(Groot.WorldCenter, Groot.WorldRadius);
        HeadRotation = (float)(_random.NextDouble() * Math.PI * 2);
        Length = Random.Shared.Next(30, 1000);
        SetRotation(HeadRotation);
    }

    public override void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _timer -= delta;
        _sprintTimer -= delta;

        if (_timer <= 0)
        {
            float randomRotation = (float)(_random.NextDouble() * Math.PI * 2);
            SetRotation(randomRotation);
            _timer = (float)(_random.NextDouble() * 2.0 + 0.5);
        }

        if (_sprintTimer <= 0)
        {
            bool shouldSprint = _random.NextDouble() > 0.8;
            ToggleSprint(shouldSprint);
            _sprintTimer = (float)(_random.NextDouble() * 3.0 + 1.0);
        }

        base.Update(gameTime);
    }
}
