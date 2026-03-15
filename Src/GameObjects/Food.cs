using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3Snake.Lib;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.GameObjects;

public class Food
{
    public Vector2D Position;
    public Vector2D MyChunkPos;
    public int FoodValue;
    public bool IsAmbient;
    public bool IsDestroyed = false;

    private Sprite _thisSprite;
    private Sprite _glowSprite;
    private float _wiggleTime;
    private float _wiggleOffsetX;
    private float _wiggleOffsetY;
    private float _targetScale = 1f;
    private float _scaleMultiplier = 0f;

    public Color Modulate => _thisSprite.Modulate;
    public float ScaleMultiplier => _scaleMultiplier;

    public Food(Vector2D position, Vector2D myChunkPos, int foodValue, bool isAmbient = false)
    {
        FoodValue = foodValue;
        Position = position;
        MyChunkPos = myChunkPos;
        IsAmbient = isAmbient;
        
        _wiggleOffsetX = (float)(Random.Shared.NextDouble() * MathF.Tau);
        _wiggleOffsetY = (float)(Random.Shared.NextDouble() * MathF.Tau);
        
        float randomSize = Random.Shared.Next(10, 20);
        
        _glowSprite = new Sprite(Groot.FoodManager.FoodGlowTexture, position);
        _glowSprite.Modulate = new Color(138,227,221);
        _glowSprite.SetUniformSize(_scaleMultiplier);
        
        _thisSprite = new Sprite(Groot.FoodManager.FoodTexture, position);
        _thisSprite.Modulate = new Color(20, 235, 148);
        _thisSprite.SetUniformSize(randomSize);
        _targetScale = _thisSprite.Scale.X;
        _thisSprite.SetUniformScale(_scaleMultiplier);
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        _scaleMultiplier = float.Lerp(_scaleMultiplier, _targetScale, 0.3f * delta);
        _wiggleTime += delta;

        float dx = MathF.Sin(_wiggleTime * 3.0f + _wiggleOffsetX) * 5.0f;
        float dy = MathF.Sin(_wiggleTime * 2.3f + _wiggleOffsetY) * 5.0f;

        _thisSprite.Position = Position + new Vector2D(dx, dy);
        _glowSprite.Position = _thisSprite.Position;
        
        _thisSprite.Visible = !IsDestroyed;
        _glowSprite.Visible = !IsDestroyed;

        _thisSprite.SetUniformScale(_scaleMultiplier);
        _glowSprite.SetUniformScale(_scaleMultiplier * 2f);
    }

    public void DestroyInstance()
    {
        _thisSprite.FreeFromBuffer();
        _glowSprite.FreeFromBuffer();
    }
}