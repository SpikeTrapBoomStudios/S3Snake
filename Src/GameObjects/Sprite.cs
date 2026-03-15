using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3Snake.Lib.ComplexType;
using S3Snake.Lib.PrimitiveTypes;
using Color = Microsoft.Xna.Framework.Color;

namespace S3Snake.GameObjects;

public class Sprite : Drawable
{
    public Texture2D Texture;

    /// <summary>
    /// World position of the sprite.
    /// If Centered is true, this is the center point.
    /// If Centered is false, this is the top-left corner.
    /// </summary>
    public Vector2D Position;

    /// <summary>
    /// Width and height scale multipliers.
    /// </summary>
    public Vector2D Scale = new Vector2D(1f, 1f);

    /// <summary>
    /// If true, Position refers to the center of the sprite.
    /// If false, Position refers to the top-left corner.
    /// </summary>
    public bool Centered = true;

    /// <summary>
    /// The rendered size of the sprite in pixels.
    /// Setting this adjusts Scale.
    /// </summary>
    public Vector2D Size
    {
        get
        {
            if (Texture == null)
                return new Vector2D(0f, 0f);

            return new Vector2D(Texture.Width * Scale.X, Texture.Height * Scale.Y);
        }
        set
        {
            if (Texture == null)
                throw new InvalidOperationException("Texture must be set before setting Size.");

            Scale = new Vector2D(
                value.X / Texture.Width,
                value.Y / Texture.Height
            );
        }
    }
    public float Rotation = 0f;
    public Color Modulate = Color.White;

    public Sprite(Texture2D texture, Vector2D position, bool centered = true, int zIndex = 0, bool registerSelf = true)
    {
        ZIndex = zIndex;
        
        Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        Position = position;
        Centered = centered;

        if (registerSelf) { AddToBuffer(); }
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Texture == null)
            throw new InvalidOperationException("Sprite.Texture cannot be null when drawing.");

        Vector2 origin = Centered
            ? new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f)
            : Vector2.Zero;
        
        spriteBatch.Draw(
            Texture,
            Position,
            null,
            Modulate,
            Rotation,
            origin,
            Scale,
            SpriteEffects.None,
            0f
        );
    }

    public void SetUniformScale(float scale)
    {
        Scale = new Vector2D(scale, scale);
    }

    public void SetUniformSize(float size)
    {
        Size = new Vector2D(size, size);
    }
}