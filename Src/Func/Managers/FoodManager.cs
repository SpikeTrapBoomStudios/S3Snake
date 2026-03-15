using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3Snake.GameObjects;
using S3Snake.GameObjects.Player;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Managers;

public class FoodManager : Manager
{
    public Texture2D FoodTexture;
    public Texture2D FoodGlowTexture;
    public ReadOnlyCollection<Food> Foods => _foods.AsReadOnly();
    
    private List<Food> _foods = new List<Food>();
    private List<FoodParticle> _particles = new List<FoodParticle>();

    public override void Init()
    {
        FoodTexture = Groot.Content.Load<Texture2D>("sprite_textures/segment");
        FoodGlowTexture = Groot.Content.Load<Texture2D>("sprite_textures/glow");
        
        SpawnRandomFood();
    }

    public void Reset()
    {
        foreach (var food in _foods)
        {
            food.DestroyInstance();
        }
        _foods.Clear();
        _particles.Clear();
    }

    public void AddFood(Food food)
    {
        _foods.Add(food);
    }

    public override void Update(GameTime gameTime)
    {
        for (int i = 0; i < 50; i++)
        {
            SpawnRandomFood();
        }
        
        for (int i = _foods.Count - 1; i >= 0; i--)
        {
            _foods[i].Update(gameTime);
            if (_foods[i].IsDestroyed)
            {
                Groot.ChunkManager.RemoveFood(_foods[i]);
                _foods[i].DestroyInstance();
                _foods.RemoveAt(i);
            }
        }

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            _particles[i].Update(gameTime);
            if (_particles[i].IsFinished)
            {
                _particles.RemoveAt(i);
            }
        }
    }

    public void DestroyFood(Food food)
    {
        food.IsDestroyed = true;
    }

    public void SpawnFoodParticle(Vector2D position, Vector2D targetPosition, float initialScale, Color color, Snake? targetSnake = null)
    {
        _particles.Add(new FoodParticle(position, targetPosition, initialScale, color, targetSnake));
    }

    public void SpawnRandomFood()
    {
        var (leftBound, rightBound, bottomBound, topBound) = (0, Groot.WorldLength, Groot.WorldLength, 0);
        const int margin = 50;
        
        Vector2D spawnPos = new Vector2D(Random.Shared.Next(leftBound - margin, rightBound + margin), Random.Shared.Next(topBound - margin, bottomBound + margin));
        Groot.ChunkManager.TryAddFood(spawnPos, Random.Shared.Next(1, 3), isAmbient: true);
    }
}