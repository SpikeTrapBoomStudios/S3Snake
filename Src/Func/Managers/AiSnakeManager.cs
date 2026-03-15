using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using S3Snake.GameObjects.Player;

namespace S3Snake.Func.Managers;

public class AiSnakeManager : Manager
{
    private const int AiSnakeCount = 25;
    
    private List<AiSnake> _aiSnakes = new();

    public void NewAiSnake()
    {
        if (_aiSnakes.Count >= AiSnakeCount) return;
        _aiSnakes.Add(new AiSnake());
    }
    
    public override void Init()
    {
        for (int i = 0; i < AiSnakeCount; i++)
        {
            NewAiSnake();
        }
    }

    public override void Update(GameTime gameTime)
    {
        for (int i = _aiSnakes.Count - 1; i >= 0; i--)
        {
            if (_aiSnakes[i].Dead)
                _aiSnakes.RemoveAt(i);
            else
                _aiSnakes[i].Update(gameTime);
        }
    }
}
