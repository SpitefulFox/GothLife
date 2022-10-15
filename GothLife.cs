using System.Collections.Generic;
using ExtremelySimpleLogger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Data;
using MLEM.Data.Content;
using MLEM.Textures;
using TinyLife;
using TinyLife.Mods;
using TinyLife.Objects;
using TinyLife.Utilities;
using TinyLife.Goals;
using TinyLife.Skills;
using TinyLife.Tools;
using TinyLife.World;
using MLEM.Extensions;
using MLEM.Misc;

namespace GothLife {
    public class GothLife : Mod {

        // the logger that we can use to log info about this mod
        public static Logger Logger { get; private set; }

        // visual data about this mod
        public override string Name => "Goth Life";
        public override string Description => "The darkness in your soul in mod form.";
        public override TextureRegion Icon => this.uiTextures[0, 0];

        private UniformTextureAtlas uiTextures;
        private UniformTextureAtlas gothTops;
        private UniformTextureAtlas gothHats;
        private UniformTextureAtlas gothShoes;


        public ColorScheme darknessScheme;
        public ColorScheme darkRainbow;

        public override void AddGameContent(GameImpl game, ModInfo info) {

            darknessScheme = ColorScheme.Create(0x101010, 0x575757, 0x8a0900, 0x390075, 0x0f450a, 0xfffeed, 0x720000, 0xff7518, 0xffadf1, 0xb2fff8, 0xcea3ff);
            darkRainbow = ColorScheme.Create(0x8a0900, 0x7b00ff, 0x0f450a, 0xfffeed, 0x720000, 0xff7518, 0xffadf1, 0xb2fff8, 0xcea3ff);

            // adding a custom furniture item
            FurnitureType.Register(new FurnitureType.TypeSettings("GothLife.Candle", new Point(1, 1), ObjectCategory.SmallObject | ObjectCategory.FireLight, 15, darknessScheme, ColorScheme.White)
            {
                Icon = this.Icon,
                //ConstructedType = typeof(FurnitureCandle)

                DecorativeRating = f => 1,
                Tab = (FurnitureTool.Tab.Decoration | FurnitureTool.Tab.Lighting),
                LightSettings =
                {
                    CreateLights = (LightFurniture f) => new Light[1]
                    {
                        new Light(f.Map, f.Position, Light.CircleTexture, new Vector2(4f, 5f), Color.Orange)
                    },
                    FireParticleOffsets = new System.Func<LightFurniture, Vector2>[1]
                    {
                        f => new Vector2(0.5f, -0.50f * (float)Tile.Height)

                    },
                    IsElectrical = false,
                    Flickers = true
                }
            });

            
            // adding custom clothing
            Clothes.Register(new Clothes("GothLife.SkullShirt", ClothesLayer.Shirt,
                this.gothTops[0, 0], // the top left in-world region (the rest will be auto-gathered from the atlas)
                100f, // the price
                ClothesIntention.Everyday | ClothesIntention.Workout | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Summer , // the clothes item's use cases
                darknessScheme, ColorScheme.White)
                { 
                    Icon = this.Icon 
                });

            Clothes.Register(new Clothes("GothLife.WitchHat", ClothesLayer.HeadAccessories,
                this.gothHats[0, 0], // the top left in-world region (the rest will be auto-gathered from the atlas)
                100f, // the price
                ClothesIntention.Everyday | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Summer | ClothesIntention.Winter, // the clothes item's use cases
                darknessScheme, darknessScheme)
            {
                Icon = this.Icon
            });

            Clothes.Register(new Clothes("GothLife.ProgrammerSocks", ClothesLayer.Shoes,
                this.gothShoes[0, 0], // the top left in-world region (the rest will be auto-gathered from the atlas)
                100f, // the price
                ClothesIntention.Everyday | ClothesIntention.Sleep, // the clothes item's use cases
                ColorScheme.White, darkRainbow)
            {
                Icon = this.Icon,
                DepthFunction = (Person.Pose _, Direction2 _) => 0f
            });

            Clothes.Register(new Clothes("GothLife.ProgrammerSocksWithShoes", ClothesLayer.Shoes,
                this.gothShoes[0, 0], // the top left in-world region (the rest will be auto-gathered from the atlas)
                100f, // the price
                ClothesIntention.Everyday | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Formal, // the clothes item's use cases
                ColorScheme.White, darkRainbow, darknessScheme)
            {
                Icon = this.Icon,
                DepthFunction = (Person.Pose _, Direction2 _) => 0f
            });

            JobType.Register(new JobType("GothLife.Gravekeeper", uiTextures[2, 0], 18f, new MonoGame.Extended.Range<int>(0, 8), System.DayOfWeek.Tuesday, System.DayOfWeek.Thursday)
            {
                RequiredPromotionSkills = new (SkillType, float)[2]
                {
                    (SkillType.Fitness, 1f),
                    (SkillType.Repair, 0.5f)
                }
            });
            JobType.Register(new JobType("GothLife.Mortician", uiTextures[4, 0], 25f, new MonoGame.Extended.Range<int>(10, 18), System.DayOfWeek.Monday, System.DayOfWeek.Wednesday, System.DayOfWeek.Tuesday)
            {
                RequiredPromotionSkills = new (SkillType, float)[2]
                {
                    (SkillType.Charisma, 0.25f),
                    (SkillType.Reasoning, 0.5f)
                }
            });

            FoodType.Register(new FoodType("GothLife.PBJ", 0, 5, 80, FoodType.FoodIntolerance.None));
            FoodType.Register(new FoodType("GothLife.BlackRice", 1, 7, 90, FoodType.FoodIntolerance.None));
            FoodType.Register(new FoodType("GothLife.DeviledEggs", 2, 8, 100, FoodType.FoodIntolerance.NotVegan));
            FoodType.Register(new FoodType("GothLife.PumpkinSpiceLatte", 5, 15, 35, FoodType.FoodIntolerance.None, FoodType.IngredientSource.CoffeeMachine, ColorHelper.FromHexRgb(0xff7518)));
        }

        public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info) {
            Logger = logger;

            // loads a texture atlas with the given amount of separate texture regions in the x and y axes
            // we submit it to the texture packer to increase rendering performance. The callback is invoked once packing is completed
            //texturePacker.Add(content.Load<Texture2D>("CustomClothes"), r => this.customClothes = new UniformTextureAtlas(r, 4, 6));

            texturePacker.Add(content.Load<Texture2D>("UiTextures"), r => this.uiTextures = new UniformTextureAtlas(r, 8, 8));
            texturePacker.Add(content.Load<Texture2D>("GothTops"), r => this.gothTops = new UniformTextureAtlas(r, 8, 8));
            texturePacker.Add(content.Load<Texture2D>("GothHats"), r => this.gothHats = new UniformTextureAtlas(r, 8, 6));
            texturePacker.Add(content.Load<Texture2D>("GothShoes"), r => this.gothShoes = new UniformTextureAtlas(r, 12, 6));
        }

        public override IEnumerable<string> GetCustomFurnitureTextures(ModInfo info)
        {
            // tell the game about our custom furniture texture
            // this needs to be a path to a data texture atlas, relative to our "Content" directory
            // the texture atlas combines the png texture and the .atlas information
            // see https://mlem.ellpeck.de/api/MLEM.Data.DataTextureAtlas.html for more info
            yield return "GothLifeFurniture";
        }

    }
}