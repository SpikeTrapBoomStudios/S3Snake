using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using S3Snake.GameObjects;
using S3Snake.GameObjects.Player;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Managers;

public class SnakeManager : Manager
{
    private List<Snake> _snakes = new(); 
    
    public void RegisterSnake(Snake snake)
    {
        _snakes.Add(snake);
    }
    
    bool CirclesOverlap(Vector2D centerA, float radiusA, Vector2D centerB, float radiusB)
    {
        float radiiSum = (radiusA + radiusB) * 0.5f;
        return (centerB - centerA).LengthSquared() <= radiiSum * radiiSum;
    }

    public override void Update(GameTime gameTime)
    {
        Groot.ChunkManager.ClearSegments();
        foreach (Snake snake in _snakes)
        {
            if (snake.Dead) continue;
            List<Sprite> bodySegments = snake.GetBodySegmentSprites();
            for (int i = 0; i < snake.ActiveSegmentCount; i++)
            {
                Sprite segment = bodySegments[i];
                Groot.ChunkManager.AddSnakeSegment(segment.Position, (int)snake.HeadRadius, snake);
            }
        }

        List<Snake> snakesToRemove = new();
        int snakesToRegiser = 0;
        foreach (Snake snake in _snakes)
        {
            if (snake.Dead) continue;

            Vector2D headChunkPos = Groot.ChunkManager.WorldPosToChunkPos(snake.HeadPosition);
            int startX = (int)headChunkPos.X - 1;
            int endX = (int)headChunkPos.X + 1;
            int startY = (int)headChunkPos.Y - 1;
            int endY = (int)headChunkPos.Y + 1;

            bool collided = false;
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    var segments = Groot.ChunkManager.GetChunksSnakeSegments(x, y, out int count);
                    for (int i = 0; i < count; i++)
                    {
                        var segment = segments[i];
                        if (segment.owner == snake) continue;
                        
                        float radiiSum = (snake.HeadRadius + segment.segmentRadius) * 0.5f;
                        float distSquared = (segment.segmentPos - snake.HeadPosition).LengthSquared();
                        if (distSquared <= radiiSum * radiiSum)
                        {
                            snakesToRemove.Add(snake);
                            snake.Die();
                            snakesToRegiser++;
                            collided = true;
                            break;
                        }
                    }
                    if (collided) break;
                }
                if (collided) break;
            }
        }
        for (int _ = 0; _ < snakesToRegiser; _++) Groot.AiSnakeManager.NewAiSnake();
        foreach (Snake snake in snakesToRemove)
        {
            _snakes.Remove(snake);
        }
    }
}