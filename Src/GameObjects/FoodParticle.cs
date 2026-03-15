using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3Snake.GameObjects.Player;
using S3Snake.Lib;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.GameObjects;

public class FoodParticle
{
    public bool IsFinished { get; private set; } = false;

    private Sprite _thisSprite;
    private Sprite _glowSprite;
    private Vector2D _targetPosition;
    private Snake? _targetSnake;
    private float _scaleMultiplier;
    private float _initialDist;

    public FoodParticle(Vector2D position, Vector2D targetPosition, float initialScale, Color color, Snake? targetSnake = null)
    {
        _targetPosition = targetPosition;
        _targetSnake = targetSnake;
        _scaleMultiplier = initialScale;

        _glowSprite = new Sprite(Groot.FoodManager.FoodGlowTexture, position);
        _glowSprite.Modulate = new Color(138, 227, 221);
        _glowSprite.SetUniformScale(_scaleMultiplier * 2f);

        _thisSprite = new Sprite(Groot.FoodManager.FoodTexture, position);
        _thisSprite.Modulate = color;
        _thisSprite.SetUniformScale(_scaleMultiplier);

        _initialDist = (position - targetPosition).Abs().Length();
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_targetSnake != null)
        {
            _targetPosition = _targetSnake.HeadPosition;
        }

        _thisSprite.Position = Vector2D.Lerp(_thisSprite.Position, _targetPosition, 5f * delta);
        _glowSprite.Position = _thisSprite.Position;
        
        float currentDist = (_thisSprite.Position - _targetPosition).Abs().Length();
        float distRatio = currentDist / _initialDist;

        float newScale = _scaleMultiplier * distRatio;

        _thisSprite.SetUniformScale(newScale);
        _glowSprite.SetUniformScale(newScale * 2f);
        
        if (Math2.ApproxEqual(_thisSprite.Position, _targetPosition, 30f))
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        _thisSprite.FreeFromBuffer();
        _glowSprite.FreeFromBuffer();
        IsFinished = true;
    }
}
