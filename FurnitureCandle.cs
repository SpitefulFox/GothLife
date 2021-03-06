﻿using System;
using Microsoft.Xna.Framework;
using TinyLife.Objects;
using TinyLife.World;
using TinyLife;
using MLEM.Animations;
using MLEM.Misc;

namespace GothLife
{
    public class FurnitureCandle : Furniture, IUpdatingObject
    {

        //SpriteAnimation flame;
        private TimeSpan particleTime = TimeSpan.FromSeconds(3.0);
        
        public FurnitureCandle(Guid id, FurnitureType type, int[] colors, Map map, Vector2 pos) : base(id, type, colors, map, pos)
        {
            //flame = new SpriteAnimation(new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame0"), 0.5), new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame1"), 0.5));
        }

        public void Update(GameTime time, TimeSpan span, GameSpeed speed)
        {
            particleTime += time.ElapsedGameTime * (double)speed;
            if (particleTime.TotalSeconds >= 3.0)
            {
                particleTime = TimeSpan.Zero;

                SpriteAnimation flame = new SpriteAnimation(new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame0"), 0.3), 
                    new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame1"), 0.3), 
                    new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame0"), 0.3), 
                    new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame2"), 0.3), 
                    new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame3"), 0.3))
                {
                    IsLooping = true
                };
                Particle particle = new Particle(flame, TimeSpan.FromSeconds(3.0), Position + new Vector2(ParentObjectSpot?.YOffset ?? 0))
                {
                    //DrawOffset = new Vector2(/*(float)(-Tile.Width) / 3f*/ 0f, (float)(-Tile.Height) * 1.0f),
                    DepthOffset = new Vector2(0f, 3f)//,
                    //Motion = new Vector2(0f, 0.025f)//,
                    //Friction = Vector2.One,
                    //Scale = Easings.AndReverse(Easings.ScaleOutput(Easings.InOutSine, 0f, 0.75f))
                };
                Particle.Spawn(particle);
            }
        }

    }
}