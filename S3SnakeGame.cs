using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using S3Snake.Func.Rendering;
using S3Snake.Lib;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake;

public class S3SnakeGame : Game
{
    public const int GameWidth = 1080;
    public const int GameHeight = 720;
    public const float AspectRatio = GameWidth / (float)GameHeight;
    
    public float WindowScale = 1f;
    public Vector2D WindowOffset = Vector2D.Zero;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget;
    
    private Renderer2D _renderer2D;
    private Camera _playerCamera;

    public S3SnakeGame()
    {
        Groot.GameInstance = this;
        
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        
        _renderer2D = new Renderer2D();
        _playerCamera = new Camera(new Vector2D(0, 0));
    }

    protected override void Initialize()
    {
        Groot.PlayerCamera = _playerCamera;
        
        Groot.Content = Content;
        Groot.Renderer2D = _renderer2D;
        
        Groot.Renderer2D.Init();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _renderTarget = new RenderTarget2D(
            GraphicsDevice,
            GameWidth,
            GameHeight,
            false,
            SurfaceFormat.Color,
            DepthFormat.None
        );
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        Groot.Init();
    }

    protected override void Update(GameTime gameTime)
    {
        float scaleX = Window.ClientBounds.Width / (float)GameWidth;
        float scaleY = Window.ClientBounds.Height / (float)GameHeight;
        WindowScale = MathF.Min(scaleX, scaleY);
        
        float viewportWidth = GameWidth * WindowScale;
        float viewportHeight = GameHeight * WindowScale;
    
        float offsetX = (Window.ClientBounds.Width - viewportWidth) * 0.5f;
        float offsetY = (Window.ClientBounds.Height - viewportHeight) * 0.5f;
        WindowOffset = new Vector2D(offsetX, offsetY);

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Groot.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Matrix windowMatrix =
            Matrix.CreateScale(WindowScale, WindowScale, 1f) *
            Matrix.CreateTranslation(WindowOffset.X, WindowOffset.Y, 0f);
        
        Matrix finalMatrix =
            Groot.PlayerCamera.GetMatrix() *
            windowMatrix;
        
        GraphicsDevice.Clear(new Color(13, 22, 31));

        if (Groot.CurrentState == GameState.Playing)
        {
            _spriteBatch.Begin(
                samplerState: SamplerState.LinearClamp,
                transformMatrix: finalMatrix
            );
            
            _renderer2D.Draw(_spriteBatch);
            
            _spriteBatch.End();
        }
        else if (Groot.CurrentState == GameState.MainMenu)
        {
            _spriteBatch.Begin(
                samplerState: SamplerState.LinearClamp,
                transformMatrix: windowMatrix
            );
            
            Groot.MainMenuManager.DrawUi(_spriteBatch);
            
            _spriteBatch.End();
        }
        
        if (Groot.CurrentState == GameState.Playing)
        {
            _spriteBatch.Begin(
                samplerState: SamplerState.LinearClamp,
                transformMatrix: windowMatrix
            );

            string lengthText = $"Length: {(int)Groot.PlayerManager.MainSnake.Length}";
            Vector2 textSize = Groot.ScoreFont.MeasureString(lengthText);
            Vector2 textPos = new Vector2(GameWidth / 2f - textSize.X / 2f, GameHeight - textSize.Y - 20);

            _spriteBatch.DrawString(Groot.ScoreFont, lengthText, textPos, Color.White);

            _spriteBatch.End();
        }
        
        base.Draw(gameTime);
    }
}