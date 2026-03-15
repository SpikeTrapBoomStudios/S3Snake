using System;
using System.Linq;
using S3Snake.GameObjects;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.Func.Managers;

public class ChunkManager : Manager
{
    private const int MaxSegmentsPerChunk = 500;
    private const int MaxFoodPerChunk = 500;
    private const int MaxAmbientFoodPerChunk = 10;
    private const int NumChunksSqrt = 10;
    private const int ChunkSize = 750;
    
    private int[,] _foodCount = new int[NumChunksSqrt, NumChunksSqrt];
    private int[,] _ambientFoodCount = new int[NumChunksSqrt, NumChunksSqrt];
    private int[,] _segmentCount = new int[NumChunksSqrt, NumChunksSqrt];
    private Food[,][] _foodChunkMap = new Food[NumChunksSqrt, NumChunksSqrt][];
    private (Vector2D segmentPos, int segmentRadius, object owner)[,][] _snakeSegmentChunkMap = new (Vector2D segmentPos, int segmentRadius, object owner)[NumChunksSqrt, NumChunksSqrt][];
    
    public override void Init()
    {
        for (int i = 0; i < NumChunksSqrt; i++)
        {
            for (int j = 0; j < NumChunksSqrt; j++)
            {
                _foodChunkMap[i, j] = new Food[MaxFoodPerChunk];
                _snakeSegmentChunkMap[i, j] = new (Vector2D segmentPos, int segmentRadius, object owner)[MaxSegmentsPerChunk];
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < NumChunksSqrt; i++)
        {
            for (int j = 0; j < NumChunksSqrt; j++)
            {
                _foodCount[i, j] = 0;
                _ambientFoodCount[i, j] = 0;
                _segmentCount[i, j] = 0;
                Array.Clear(_foodChunkMap[i, j], 0, MaxFoodPerChunk);
                
                Array.Clear(_snakeSegmentChunkMap[i, j], 0, MaxSegmentsPerChunk);
            }
        }
    }

    public void ClearSegments()
    {
        for (int i = 0; i < NumChunksSqrt; i++)
        {
            for (int j = 0; j < NumChunksSqrt; j++)
            {
                _segmentCount[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// Returns a reference to the food array in the chunk at (x, y).
    /// </summary>
    public ref Food[] GetChunksFood(int x, int y)
    {
        if (x < 0 || y < 0) return ref _foodChunkMap[0, 0];
        if (x >= _foodChunkMap.GetLength(0) || y >= _foodChunkMap.GetLength(1)) return ref _foodChunkMap[0, 0];
        return ref _foodChunkMap[x, y];
    }

    public ref (Vector2D segmentPos, int segmentRadius, object owner)[] GetChunksSnakeSegments(int x, int y, out int count)
    {
        if (x < 0 || y < 0 || x >= NumChunksSqrt || y >= NumChunksSqrt)
        {
            count = 0;
            return ref _snakeSegmentChunkMap[0, 0];
        }
        count = _segmentCount[x, y];
        return ref _snakeSegmentChunkMap[x, y];   
    }
    
    public void AddSnakeSegment(Vector2D pos, int radius, object owner)
    {
        Vector2D chunkPos = WorldPosToChunkPos(pos);
        int snappedX = (int)chunkPos.X;
        int snappedY = (int)chunkPos.Y;
        
        if (snappedX < 0 || snappedY < 0 || snappedX >= NumChunksSqrt || snappedY >= NumChunksSqrt) return;
        
        int count = _segmentCount[snappedX, snappedY];
        if (count >= MaxSegmentsPerChunk) return;
        
        _snakeSegmentChunkMap[snappedX, snappedY][count] = (pos, radius, owner);
        _segmentCount[snappedX, snappedY]++;
    }
    
    /// <summary>
    /// Attempts to add food, taking into account food per chunk limits and chunk containerization.
    /// </summary>
    public void TryAddFood(Vector2D worldPos, int foodValue, bool isAmbient = false)
    {
        Vector2D chunkPos = WorldPosToChunkPos(worldPos);
        int snappedX = (int)chunkPos.X;
        int snappedY = (int)chunkPos.Y;
        
        if (snappedX < 0 || snappedY < 0) return;
        
        if (snappedX >= _foodChunkMap.GetLength(0) || snappedY >= _foodChunkMap.GetLength(1)) return;
        
        Food[] foods = _foodChunkMap[snappedX, snappedY];
        
        int count = _foodCount[snappedX, snappedY];
        if (count >= MaxFoodPerChunk) return;

        if (isAmbient && _ambientFoodCount[snappedX, snappedY] >= MaxAmbientFoodPerChunk) return;
        
        _foodCount[snappedX, snappedY] += 1;
        if (isAmbient) _ambientFoodCount[snappedX, snappedY] += 1;
        
        Food newFood = new(worldPos, chunkPos, foodValue, isAmbient);
        Groot.FoodManager.AddFood(newFood);
        foods[count] = newFood;
    }

    
    public void RemoveFood(Food food)
    {
        int snappedX = (int)food.MyChunkPos.X;
        int snappedY = (int)food.MyChunkPos.Y;

        if (snappedX < 0 || snappedY < 0) return;
        if (snappedX >= _foodChunkMap.GetLength(0) || snappedY >= _foodChunkMap.GetLength(1)) return;

        Food[] foods = _foodChunkMap[snappedX, snappedY];
        int count = _foodCount[snappedX, snappedY];

        for (int i = 0; i < count; i++)
        {
            if (foods[i] == food)
            {
                foods[i] = foods[count - 1];
                foods[count - 1] = null!;
                
                _foodCount[snappedX, snappedY] -= 1;
                if (food.IsAmbient) _ambientFoodCount[snappedX, snappedY] -= 1;
                break;
            }
        }
    }
    
    public Vector2D WorldPosToChunkPos(Vector2D worldPos)
    {
        return new Vector2D((int)Math.Round(worldPos.X / ChunkSize), (int)Math.Round(worldPos.Y / ChunkSize));
    }
}