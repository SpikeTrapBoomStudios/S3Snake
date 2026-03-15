using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using S3Snake.Lib.ComplexType;
using S3Snake.Lib.PrimitiveTypes;

namespace S3Snake.GameObjects.Player;

public class Snake : Drawable
{
    public float Speed = 150f;
    public float NormalSpeed = 150f;
    public float SprintSpeed = 550f;
    public Vector2D HeadPosition;
    public float HeadRotation = 0f;
    public int Length = 1;
    public bool IsPlayer = false;
    public bool Dead = false;
    public float HeadRadius => _headRadius;
    public bool Sprinting = false;
    
    private const float SpineSpacing = 7f;
    private const float DelayTime = 0.9f;
    private static readonly Color[] RandomColorSet = new[]
    {
        Color.Red,
        Color.Blue,
        Color.LimeGreen,
        Color.Yellow,
        Color.Cyan,
        Color.Magenta,
        Color.Orange,
        Color.Purple,
        Color.HotPink,
        Color.Turquoise,
        Color.Gold,
        Color.Coral,
        Color.DodgerBlue,
        Color.MediumSpringGreen,
        Color.OrangeRed,
    };
    
    private const float VisualSpacingRatio = 0.2f;
    private const int MaxSpineGrowthPerFrame = 3;
    
    private List<Vector2D> _spinePositions = new();
    
    private List<Sprite> _visualSegments = new();
    private List<Sprite> _visualShadows = new();
    private int _activeVisualCount = 0;
    private float _visualSpacingMult = 1;
    private float _targetVisualSpacingMult = 1;

    private Sprite _headSprite;
    private Sprite _eyesSprite;
    private Sprite _headShadowSprite;

    private float _targetRotation = 0f;
    private float _headRadius = 14f;
    private float MaxTurnSpeedDegrees = 4f;
    private float MaxTurnSpeedRadians => MathF.PI / 180f * MaxTurnSpeedDegrees;

    private static bool _debugMode = false;
    private bool _debugKeyPressed = false;
    private Sprite _debugSegmentSprite;
    
    public Snake()
    {
        if (Groot.Content == null)
            throw new InvalidOperationException("Groot.Content must be initialized before constructing Snake.");

        // Seed spine with a small initial set
        int initialCount = Math.Max((int)(GetNeededSpineLength() / SpineSpacing), 1);
        for (int i = 0; i < initialCount; i++)
            _spinePositions.Add(HeadPosition);

        CreateHeadSprite();

        Groot.SnakeManager.RegisterSnake(this);
        ZIndex = 1;
        AddToBuffer();
    }

    /// <summary>
    /// How many entries in the visual segment list are actually active this frame.
    /// External code such as collision stuff should only read indices 0.._activeVisualCount-1.
    /// </summary>
    public int ActiveSegmentCount => _activeVisualCount;

    public ref List<Sprite> GetBodySegmentSprites() => ref _visualSegments;

    private void CreateHeadSprite()
    {
        var segmentTex = Groot.Content.Load<Texture2D>("sprite_textures/segment");
        var eyesTex = Groot.Content.Load<Texture2D>("sprite_textures/eyes");
        var shadowTex = Groot.Content.Load<Texture2D>("sprite_textures/segment_shadow");

        _headSprite = new Sprite(segmentTex, HeadPosition, registerSelf: false);
        _headSprite.SetUniformSize(_headRadius);
        _headSprite.Modulate = new Color(54, 0, 68);

        _eyesSprite = new Sprite(eyesTex, HeadPosition, registerSelf: false);
        _eyesSprite.SetUniformSize(_headRadius);
        _eyesSprite.ZIndex = 1000;

        _headShadowSprite = new Sprite(shadowTex, HeadPosition, registerSelf: false);
        _headShadowSprite.ZIndex = 1000;

        _debugSegmentSprite = new Sprite(segmentTex, HeadPosition, registerSelf: false);
        _debugSegmentSprite.Modulate = Color.White;
        _debugSegmentSprite.SetUniformSize(10f);
    }
    
    private int GetTargetVisualCount()
    {
        return (int)(MathF.Sqrt(Length));
    }
    
    private float GetNeededSpineLength()
    {
        int visualCount = (int)Math.Ceiling(GetTargetVisualCount() * 1.5);
        return visualCount * _headRadius * VisualSpacingRatio;
    }
    
    private float GetActualSpineLength()
    {
        if (_spinePositions.Count == 0) return 0f;

        float total = (HeadPosition - _spinePositions[0]).Length();

        for (int i = 1; i < _spinePositions.Count; i++)
            total += (_spinePositions[i] - _spinePositions[i - 1]).Length();

        return total;
    }

    private float GetTargetRadius()
    {
        if (Length <= 0) return 14f;

        const float minRadius = 2f;
        const float maxRadius = 100f;
        const float logScale = 0.01f;
        const float logOffset = -0.029f;
        float curveOutputMin = logScale * MathF.Log(1f) + logOffset;
        float curveOutputMax = logScale * MathF.Log(100_000f) + logOffset;

        float logValue = logScale * MathF.Log(Length) + logOffset;
        float normalizedT = (logValue - curveOutputMin) / (curveOutputMax - curveOutputMin);
        float clampedT = Math.Clamp(normalizedT, 0f, 1f);

        return minRadius + clampedT * (maxRadius - minRadius);
    }

    public void SetRotation(float newRotation)
    {
        _targetRotation = newRotation;
    }

    public void ToggleSprint(bool sprinting)
    {
        Speed = sprinting ? SprintSpeed : NormalSpeed;
        Sprinting = sprinting;
    }

    public virtual void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (IsPlayer)
        {
            var ks = Keyboard.GetState();
            bool debugKeysDown = ks.IsKeyDown(Keys.X) && ks.IsKeyDown(Keys.C) && ks.IsKeyDown(Keys.V);
            if (debugKeysDown && !_debugKeyPressed)
            {
                _debugMode = !_debugMode;
                _debugKeyPressed = true;
            }
            else if (!debugKeysDown)
            {
                _debugKeyPressed = false;
            }
        }

        // -- Grow spine gradually based on actual measured length --
        float neededLength = GetNeededSpineLength();
        float actualLength = GetActualSpineLength();

        if (actualLength < neededLength)
        {
            int added = 0;
            while (added < MaxSpineGrowthPerFrame)
            {
                _spinePositions.Add(_spinePositions[^1]);
                added++;
            }
        }

        // -- Update width --
        float targetRadius = GetTargetRadius();
        float widthLerpSpeed = 1.5f + (_headRadius / 20f);
        _headRadius = MathHelper.Lerp(_headRadius, targetRadius, widthLerpSpeed * delta);

        MaxTurnSpeedDegrees = 160f / _headRadius;

        // -- Head movement --
        float deltaRot = _targetRotation - HeadRotation;
        while (deltaRot > MathF.PI) deltaRot -= MathF.Tau;
        while (deltaRot < -MathF.PI) deltaRot += MathF.Tau;
        deltaRot = Math.Clamp(deltaRot, -MaxTurnSpeedRadians, MaxTurnSpeedRadians);
        HeadRotation += deltaRot;

        HeadPosition += Vector2D.Right.Rotated(HeadRotation) * Speed * delta;

        // -- Head sprites --
        _headSprite.Position = HeadPosition;
        _headSprite.Rotation = HeadRotation + MathF.PI / 2;
        _headSprite.SetUniformSize(_headRadius);

        _eyesSprite.Position = HeadPosition;
        _eyesSprite.Rotation = HeadRotation + MathF.PI / 2;
        _eyesSprite.Scale = _headSprite.Scale;

        // -- Spine simulation --
        Vector2D prevPosition = HeadPosition;
        Vector2D prevForward = Vector2D.Right.Rotated(HeadRotation);

        for (int i = 0; i < _spinePositions.Count; i++)
        {
            Vector2D current = _spinePositions[i];

            Vector2D ideal = prevPosition - prevForward * SpineSpacing;
            Vector2D blended = ideal + (current - ideal) * DelayTime;

            _spinePositions[i] = blended;

            Vector2D towardPrev = (prevPosition - blended).Normalized();
            prevForward = towardPrev;
            prevPosition = blended;
        }

        // -- Sample spine for visuals --
        SampleVisuals();

        CheckForFood();
        CheckForBoundsDeath();
    }

    /// <summary>
    /// Walks along the spine chain and places visual sprites at radius-proportional intervals
    /// </summary>
    private void SampleVisuals()
    {
        int targetVisualCount = GetTargetVisualCount();
        _targetVisualSpacingMult =  Sprinting ? 1.5f : 1;
        _visualSpacingMult = MathHelper.Lerp(_visualSpacingMult, _targetVisualSpacingMult, 0.02f);
        float visualSpacing = _headRadius * VisualSpacingRatio * _visualSpacingMult;
        
        // Ensure the sprite pool has enough
        while (_visualSegments.Count < targetVisualCount)
            CreateVisualSprite();
        
        _activeVisualCount = 0;

        int breakoutIndex = 0;
        int spineIndex = 0;
        float distAccum = 0;
        Vector2D prevSpinePos = HeadPosition;
        
        while (spineIndex < _spinePositions.Count)
        {
            breakoutIndex++;
            if (breakoutIndex >= 100000) break;
            
            Vector2D spinePos = _spinePositions[spineIndex];
            
            float currentDist = (spinePos - prevSpinePos).Length();
            distAccum += currentDist;

            if (_activeVisualCount >= targetVisualCount) break;
            
            if (distAccum >= visualSpacing)
            {
                float angleToPrev = spinePos.AngleToPoint(prevSpinePos);
                float distDiff = distAccum - visualSpacing;
                Vector2D placedSpritePos = spinePos + Vector2D.Right.Rotated(angleToPrev) * distDiff;
                
                Sprite visSegSprite = _visualSegments[_activeVisualCount];
                visSegSprite.Position = placedSpritePos;
                visSegSprite.Rotation = angleToPrev;
                visSegSprite.SetUniformSize(_headRadius);
                
                _activeVisualCount++;
                
                distAccum = 0;
                prevSpinePos = placedSpritePos;
            }
            else
            {
                prevSpinePos = spinePos;
                spineIndex++;
            }
        }
    }

    private void CreateVisualSprite()
    {
        var segmentTex = Groot.Content.Load<Texture2D>("sprite_textures/segment");
        Sprite seg = new Sprite(segmentTex, HeadPosition, registerSelf: false);
        seg.SetUniformSize(_headRadius);
        seg.Modulate = RandomColorSet[Random.Shared.Next(RandomColorSet.Length)];
        _visualSegments.Add(seg);

        var shadowTex = Groot.Content.Load<Texture2D>("sprite_textures/segment_shadow");
        Sprite shadow = new Sprite(shadowTex, HeadPosition, registerSelf: false);
        _visualShadows.Add(shadow);
    }

    public void CheckForFood()
    {
        Vector2D headChunkPos = Groot.ChunkManager.WorldPosToChunkPos(HeadPosition);
        ref Food[] foods = ref Groot.ChunkManager.GetChunksFood((int)headChunkPos.X, (int)headChunkPos.Y);

        foreach (var food in foods)
        {
            if (food == null) continue;
            if (food.IsDestroyed) continue;

            float distSquared = (food.Position - HeadPosition).LengthSquared();
            float headRadiusWithMargin = _headRadius + 50;
            if (distSquared < (headRadiusWithMargin * headRadiusWithMargin))
                EatFood(food);
        }
    }

    public void CheckForBoundsDeath()
    {
        float dX = HeadPosition.X - Groot.WorldCenter.X;
        float dY = HeadPosition.Y - Groot.WorldCenter.Y;
        float dist = new Vector2D(dX, dY).Length();

        bool isTouching = Math.Abs((dist + _headRadius) - Groot.WorldRadius) < 0.0001;
        bool isContained = dist + _headRadius <= Groot.WorldRadius;

        if (isTouching || !isContained) Die();
    }

    public void Die()
    {
        Console.WriteLine("Dying");
        if (IsPlayer)
        {
            Groot.CurrentState = GameState.MainMenu;
            return;
        }

        const int foodPerSegment = 2;
        
        int foodValueEst = Length / (_activeVisualCount * foodPerSegment) / 2;
        foodValueEst = Math.Max(foodValueEst, 1);
        
        Dead = true;
        for (int i = 0; i < _activeVisualCount; i++)
        {
            Vector2D segPos = _visualSegments[i].Position;

            for (int j = 0; j < foodPerSegment; j++)
            {
                int dX = Random.Shared.Next(-10, 10);
                int dY = Random.Shared.Next(-10, 10);
                segPos += new Vector2D(dX, dY);

                Groot.ChunkManager.TryAddFood(segPos, foodValueEst, isAmbient: false);
            }
        }
        FreeFromBuffer();
    }

    public void EatFood(Food food)
    {
        Groot.FoodManager.SpawnFoodParticle(food.Position, HeadPosition, food.ScaleMultiplier, food.Modulate, this);
        Groot.FoodManager.DestroyFood(food);
        Length += food.FoodValue;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw only the active visual segments, back to front
        for (int i = _activeVisualCount - 1; i >= 0; i--)
        {
            Sprite shadow = _visualShadows[i];
            shadow.Position = _visualSegments[i].Position;
            shadow.Scale = _visualSegments[i].Scale * 1.95f;
            shadow.Draw(spriteBatch);

            _visualSegments[i].Draw(spriteBatch);
        }

        _headShadowSprite.Position = HeadPosition;
        _headShadowSprite.Scale = _headSprite.Scale * 1.95f;
        _headShadowSprite.Draw(spriteBatch);
        _headSprite.Draw(spriteBatch);
        _eyesSprite.Draw(spriteBatch);

        if (_debugMode)
        {
            foreach (var pos in _spinePositions)
            {
                _debugSegmentSprite.Position = pos;
                _debugSegmentSprite.Draw(spriteBatch);
            }
        }
    }
}