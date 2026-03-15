using Microsoft.Xna.Framework.Graphics;

namespace S3Snake.Lib.ComplexType;

public abstract class Drawable
{
    public int ZIndex = 0;
    public bool Visible = true;
    
    public abstract void Draw(SpriteBatch spriteBatch);

    public void AddToBuffer()
    {
        Groot.Renderer2D.RegisterDrawable(this, ZIndex);
    }

    public void FreeFromBuffer()
    {
        Groot.Renderer2D.UnregisterDrawable(this);
    }
}