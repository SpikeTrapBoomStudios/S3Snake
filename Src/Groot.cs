using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using S3Snake.Func.Managers;
using S3Snake.Func.Rendering;
using S3Snake.GameObjects.Player;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake;

public enum GameState
{
    MainMenu,
    Playing
}

/// <summary>
/// Groot, short for GameRoot, is the game master controller. It houses common variables, initializers, etc. All of its methods and properties are static.
/// </summary>
public class Groot
{
    public static GameState CurrentState = GameState.MainMenu;
    public static readonly int WorldLength = 7500;
    public static readonly Vector2D WorldCenter = new Vector2D(WorldLength / 2f, WorldLength / 2f);
    public static readonly int WorldRadius = WorldLength / 2;
    
    public static S3SnakeGame GameInstance = null!;
    public static Renderer2D Renderer2D = null!;
    public static ContentManager Content = null!;

    public static Vector2D ScreenMousePosition;
    public static Vector2D WorldMousePosition;

    public static Camera PlayerCamera = null!;

    public static FoodManager FoodManager = null!;
    public static PlayerManager PlayerManager = null!;
    public static AiSnakeManager AiSnakeManager = null!;
    public static ChunkManager ChunkManager = null!;
    public static MainMenuManager MainMenuManager = null!;
    public static SnakeManager SnakeManager = null!;
    
    public static SpriteFont ScoreFont = null!;

    public static void Init()
    {
        if (Content == null)
        {
            throw new InvalidOperationException("Groot.Content must be initialized before Groot.Init.");
        }

        FoodManager = new FoodManager();
        PlayerManager = new PlayerManager();
        SnakeManager = new SnakeManager();
        AiSnakeManager = new AiSnakeManager();
        ChunkManager = new ChunkManager();
        MainMenuManager = new MainMenuManager();

        ChunkManager.Init();
        PlayerManager.Init();
        SnakeManager.Init();
        AiSnakeManager.Init();
        FoodManager.Init();
        MainMenuManager.Init();

        ScoreFont = Content.Load<SpriteFont>("ScoreFont");
    }

    public static void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
        
        ScreenMousePosition = new Vector2D(mouseState.X, mouseState.Y);
        ScreenMousePosition -= GameInstance.WindowOffset;
        ScreenMousePosition /= GameInstance.WindowScale;

        if (CurrentState == GameState.MainMenu)
        {
            MainMenuManager.Update(gameTime);
            return;
        }

        if (FoodManager == null || PlayerManager == null) return;

        WorldMousePosition = ScreenMousePosition;
        WorldMousePosition /= PlayerCamera.Zoom;
        WorldMousePosition = WorldMousePosition.Rotated(-PlayerCamera.Rotation);
        WorldMousePosition += PlayerCamera.Position;

        FoodManager.Update(gameTime);
        PlayerManager.Update(gameTime);
        SnakeManager.Update(gameTime);
        AiSnakeManager.Update(gameTime);
    }
}
