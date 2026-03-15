using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using S3Snake.GameObjects;
using S3Snake.GameObjects.UI;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Managers;

public class MainMenuManager : Manager
{
    private Button _playButton;

    public override void Init()
    {
        var buttonTex = Groot.Content.Load<Texture2D>("sprite_textures/playbutton");
        int x = S3SnakeGame.GameWidth / 2 - buttonTex.Width / 2;
        int y = S3SnakeGame.GameHeight / 2 - buttonTex.Height / 2;
        _playButton = new Button(buttonTex, x, y);
    }

    public override void Update(GameTime gameTime)
    {
        _playButton.Update(Mouse.GetState());

        if (_playButton.IsClicked)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Groot.FoodManager.Reset();
        Groot.PlayerManager.Reset();
        Groot.ChunkManager.Reset();
        Groot.CurrentState = GameState.Playing;
    }

    public void DrawUi(SpriteBatch spriteBatch)
    {
        _playButton.Draw(spriteBatch);
    }
}