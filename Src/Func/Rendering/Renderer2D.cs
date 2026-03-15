using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using S3Snake.GameObjects;
using S3Snake.GameObjects.Player;
using S3Snake.Lib.ComplexType;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Rendering;

public class Renderer2D
{
    private SortedDictionary<int, List<Drawable>> _layers = new();

    public void Init()
    {
    }
    
    public void RegisterDrawable(Drawable drawable, int zIndex) {
        if (!_layers.ContainsKey(zIndex))
            _layers[zIndex] = new();
        
        _layers[zIndex].Add(drawable);
    }
    
    public void UnregisterDrawable(Drawable drawable) {
        foreach (List<Drawable> layer in _layers.Values)
        {
            layer.Remove(drawable);
        }
    }

    public void DrawBackground(SpriteBatch spriteBatch)
    {
        var (leftBound, rightBound, bottomBound, topBound) = Groot.PlayerCamera.GetCameraBounds();

        int spacing = 300;
        float radius = 100f;
        
        int startX = (leftBound / spacing) * spacing - spacing * 4;
        int startY = (topBound / spacing) * spacing - spacing * 4;

        Color fillColor = new Color(22, 34, 48);
        Color outlineColor = new Color(9, 11, 18);

        for (int x = startX; x < rightBound + spacing * 4; x += spacing)
        {
            for (int y = startY; y < bottomBound + spacing * 4; y += spacing)
            {
                Vector2 pos = new Vector2(x, y);
                spriteBatch.DrawCircle(pos, radius, 9, fillColor, radius);
                spriteBatch.DrawCircle(pos, radius, 9, outlineColor, 4f);
            }
        }
    }

    public void DrawBarrier(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawCircle(Groot.WorldCenter, Groot.WorldRadius + 10, 200, Color.DarkRed, 10f);
        spriteBatch.DrawCircle(Groot.WorldCenter, Groot.WorldRadius + 10 + 5000, 200, new Color(255,73,73), 5000f);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        DrawBackground(spriteBatch);
        
        var (left, right, bottom, top) = Groot.PlayerCamera.GetCameraBounds();
        float padding = 500f;

        foreach (List<Drawable> layer in _layers.Values)
        {
            foreach (var drawable in layer)
            {
                if (!drawable.Visible) continue;

                if (drawable is Sprite sprite)
                {
                    if (sprite.Position.X < left - padding || sprite.Position.X > right + padding ||
                        sprite.Position.Y < top - padding || sprite.Position.Y > bottom + padding)
                    {
                        continue;
                    }
                }

                drawable.Draw(spriteBatch);
            }
        }
        
        DrawBarrier(spriteBatch);
    }
}