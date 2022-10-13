using System;
using Microsoft.Xna.Framework;
using TinyLife.Objects;
using TinyLife.World;
using TinyLife;
using MLEM.Animations;
using MLEM.Misc;
using System.Collections.Generic;

namespace GothLife
{
    public class FurnitureCandle : LightFurniture
    {

        //SpriteAnimation flame;
        private TimeSpan particleTime = TimeSpan.FromSeconds(3.0);
        //public override bool IsElectrical => false;
        
        public FurnitureCandle(Guid id, FurnitureType type, int[] colors, Map map, Vector2 pos) : base(id, type, colors, map, pos)
        {
            //flame = new SpriteAnimation(new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame0"), 0.5), new AnimationFrame(FurnitureType.GetTexture("GothLife.CandleFlame1"), 0.5));
        }

        public Particle Flame(Vector2 pos)
        {
            SpriteAnimation flame = new SpriteAnimation(new AnimationFrame(0.3, FurnitureType.GetTexture("GothLife.CandleFlame0")),
                                new AnimationFrame(0.3, FurnitureType.GetTexture("GothLife.CandleFlame1")),
                                new AnimationFrame(0.3, FurnitureType.GetTexture("GothLife.CandleFlame0")),
                                new AnimationFrame(0.3, FurnitureType.GetTexture("GothLife.CandleFlame2")),
                                new AnimationFrame(0.3, FurnitureType.GetTexture("GothLife.CandleFlame3")))
            {
                IsLooping = true
            };
            Particle particle = new Particle(flame, TimeSpan.FromSeconds(3.0), pos)
            {
                //DrawOffset = new Vector2(/*(float)(-Tile.Width) / 3f*/ 0f, (float)(-Tile.Height) * 1.0f),
                DepthPosOffset = new Vector2(0f, 3f)//,
                //Motion = new Vector2(0f, 0.025f)//,
                //Friction = Vector2.One,
                //Scale = Easings.AndReverse(Easings.ScaleOutput(Easings.InOutSine, 0f, 0.75f))
            };
            return particle;
        }

        public override void Update(GameTime time, TimeSpan span, float speed)
        {
            particleTime += time.ElapsedGameTime * (double)speed;
            if (!IsDisabled && particleTime.TotalSeconds >= 3.0)
            {
                particleTime = TimeSpan.Zero;

                
                Particle.Spawn(Position + new Vector2(ParentObjectSpot?.YOffset ?? 0), Flame);
            }
        }

        protected override IEnumerable<Light> CreateLights()
        {
            return new Light[1]
            {
                new Light(Map, Position, Light.CircleTexture, new Vector2(5f, 10f), Color.Orange)
            };
        }

    }
}