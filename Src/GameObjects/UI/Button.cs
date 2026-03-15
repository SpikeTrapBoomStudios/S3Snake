using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace S3Snake.GameObjects.UI;

public class Button
{
    private Texture2D _texture;
    private Rectangle _bounds;
    private bool _isHovered;

    public bool IsClicked { get; private set; }

    public Button(Texture2D texture, int x, int y)
    {
        _texture = texture;
        _bounds = new Rectangle(x, y, texture.Width, texture.Height);
    }

    public void Update(MouseState mouse)
    {
        _isHovered = _bounds.Contains(Groot.ScreenMousePosition);
        IsClicked = _isHovered && mouse.LeftButton == ButtonState.Pressed;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color tint = _isHovered ? Color.Gray : Color.White;
        spriteBatch.Draw(_texture, _bounds, tint);
    }
}