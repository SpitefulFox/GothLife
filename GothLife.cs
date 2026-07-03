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

using static TinyLife.Skills.FoodType;
using MLEM.Maths;
using MLEM.Graphics;

namespace GothLife {
    public class GothLife : Mod {

        // the logger that we can use to log info about this mod
        public static Logger Logger { get; private set; }

        // visual data about this mod
        public override string Name => "Goth Life";
        public override string Description => "The darkness in your soul in mod form.";
        public override TextureRegion Icon => this.uiTextures[new Point(0, 0)];
        public override string IssueTrackerUrl => "https://github.com/SpitefulFox/GothLife/issues";
        public override string TestedVersionRange => "[0.49.0,0.49.2]";

        private Dictionary<Point, TextureRegion> uiTextures;
        private Dictionary<Point, TextureRegion> gothTops;
        private Dictionary<Point, TextureRegion> gothHats;
        private Dictionary<Point, TextureRegion> gothShoes;
        private Dictionary<Point, TextureRegion> gothWallpaper;


        public ColorScheme darknessScheme;
        public ColorScheme darkRainbow;

        public override void AddGameContent(GameImpl game, ModInfo info) {

            darknessScheme = ColorScheme.Create(0x101010, 0x575757, 0x8a0900, 0x390075, 0x0f450a, 0xfffeed, 0x720000, 0xff7518, 0xffadf1, 0xb2fff8, 0xcea3ff);
            darkRainbow = ColorScheme.Create(0x8a0900, 0x7b00ff, 0x0f450a, 0xfffeed, 0x720000, 0xff7518, 0xffadf1, 0xb2fff8, 0xcea3ff);

            // adding a custom furniture item
            FurnitureType.Register(new FurnitureType.TypeSettings("GothLife.Candle", new Point(1, 1), ObjectCategory.SmallObject | ObjectCategory.FireLight, 15, darknessScheme, ColorScheme.White)
            {
                Icon = this.Icon,

                DecorativeRating = f => 1,
                Tab = (FurnitureTool.Tab.Decoration | FurnitureTool.Tab.Lighting),
                LightSettings =
                {
                    CreateLights = (MapObject f) => new Light[1]
                    {
                        new Light(f.Map, f.Position, f.Floor, Light.CircleTexture, new Vector2(4f, 5f), Color.Orange)
                    },
                    FireParticleOffsets = new System.Func<MapObject, Vector2>[1]
                    {
                        f => new Vector2(0.5f, -0.50f * (float)TextureHandler.TileSize.Y)

                    },
                    IsElectrical = false,
                    Flickers = true
                }
            });

            FurnitureType.TypeSettings cauldron = new FurnitureType.TypeSettings("GothLife.Cauldron", new Point(1, 1), ObjectCategory.TeaKit | ObjectCategory.CoffeeMachine | ObjectCategory.BartendingObject, 500f, ColorScheme.White, darknessScheme, ColorScheme.SimpleWood)
            {
                Icon = this.Icon,
                Tab = (FurnitureTool.Tab.Kitchen),
                ActionSpots = new ActionSpot[1]
                {
                    new ActionSpot(Vector2.Zero, (Direction2)1)
                }
            };
            cauldron.BreakSettings.Reliability = 3;
            cauldron.BreakSettings.Behavior = (IBreakableObject.BrokenBehavior.Puddles | IBreakableObject.BrokenBehavior.Dripping | IBreakableObject.BrokenBehavior.Smoke);
            FurnitureType.Register(cauldron);

            Wallpaper.Register("GothLife.SkullWallpaper", 15, this.gothWallpaper, new Point(0, 0), new ColorScheme[2] { darknessScheme, ColorScheme.White }, this.Icon);


            // adding custom clothing
            Clothes.Register(new Clothes("GothLife.SkullShirt", ClothesLayer.Shirt,
                this.gothTops, new Point(0,0), // the top left in-world region (the rest will be auto-gathered from the atlas)
                40f, // the price
                ClothesIntention.Everyday | ClothesIntention.Workout | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Summer , // the clothes item's use cases
                StylePreference.Neutral, darknessScheme, ColorScheme.White)
                { 
                    Icon = this.Icon 
                });

            Clothes.Register(new Clothes("GothLife.WitchHat", ClothesLayer.HeadAccessories,
                this.gothHats, new Point(0, 0), // the top left in-world region (the rest will be auto-gathered from the atlas)
                100f, // the price
                ClothesIntention.Everyday | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Summer | ClothesIntention.Winter, // the clothes item's use cases
                StylePreference.Neutral, darknessScheme, darknessScheme)
            {
                Icon = this.Icon
            });

            Clothes.Register(new Clothes("GothLife.ProgrammerSocks", ClothesLayer.LegAccessories,
                this.gothShoes, new Point(0, 0), // the top left in-world region (the rest will be auto-gathered from the atlas)
                12f, // the price
                ClothesIntention.Everyday | ClothesIntention.Winter, // the clothes item's use cases
                StylePreference.Feminine, ColorScheme.White, darkRainbow)
            {
                Icon = this.Icon
            });

            /*Clothes.Register(new Clothes("GothLife.ProgrammerSocksWithShoes", ClothesLayer.Shoes,
                this.gothShoes, new Point(0, 0), // the top left in-world region (the rest will be auto-gathered from the atlas)
                60f, // the price
                ClothesIntention.Everyday | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Formal, // the clothes item's use cases
                StylePreference.Feminine, ColorScheme.White, darkRainbow, darknessScheme)
            {
                Icon = this.Icon,
                DepthFunction = Clothes.BelowDepth(ClothesLayer.Pants, 0.5f)
            });*/

            JobType.Register(new JobType("GothLife.Gravekeeper", uiTextures[new Point(2, 0)], 18f, (0, 8), System.DayOfWeek.Tuesday, System.DayOfWeek.Thursday)
            {
                RequiredPromotionSkills = new (SkillType, float)[2]
                {
                    (SkillType.Cleaning, 1f),
                    (SkillType.Repair, 0.5f)
                }
            });
            JobType.Register(new JobType("GothLife.Mortician", uiTextures[new Point(4, 0)], 25f, (10, 18), System.DayOfWeek.Monday, System.DayOfWeek.Wednesday, System.DayOfWeek.Tuesday)
            {
                RequiredPromotionSkills = new (SkillType, float)[2]
                {
                    (SkillType.Charisma, 0.25f),
                    (SkillType.Reasoning, 0.5f)
                }
            });

            FoodType.Register(new FoodType("GothLife.PBJ", 0, 5, 0.5f, FoodRestriction.None, FoodTime.Lunch));
            FoodType.Register(new FoodType("GothLife.BlackRice", 1, 7, 0.75f, FoodRestriction.None, FoodTime.None)
            {
                AdditionalObject = ObjectCategory.Stove
            });
            FoodType.Register(new FoodType("GothLife.DeviledEggs", 2, 8, 1.5f, FoodRestriction.NotVegan, FoodTime.Lunch | FoodTime.Dinner)
            {
                AdditionalObject = ObjectCategory.Stove
            });
            FoodType.Register(new FoodType("GothLife.PumpkinSpiceLatte", 5, 18, 0.4f, FoodRestriction.NotForBabies, FoodTime.Breakfast, FoodType.IngredientSource.CoffeeMachine, ColorHelper.FromHexRgb(0xff7518)));
            FoodType.Register(new FoodType("GothLife.LapsangSouchong", 0, 8, 0.3f, FoodRestriction.NotForBabies, FoodTime.Breakfast, IngredientSource.TeaKit, ColorHelper.FromHexRgb(0x605b5a)));
            //Is it blood? Is it a Bloody Mary with bacon in it? Who knoooows
            FoodType.Register(new FoodType("GothLife.Countess", 5, 30, 1f, FoodRestriction.NotForChildren | FoodRestriction.NotPescetarian, FoodTime.None, FoodType.IngredientSource.MixologyKit, ColorHelper.FromHexRgb(0x750800)));
            //Absinthe prepared classical style by pouring water through a sugar cube. Should probably be white like the actual drink once you pour it, but COLORS!
            FoodType.Register(new FoodType("GothLife.GreenFairy", 3, 15, 0.15f, FoodRestriction.NotForChildren, FoodTime.None, FoodType.IngredientSource.MixologyKit, ColorHelper.FromHexRgb(0x7cff54)));
            //Made up drink!
            FoodType.Register(new FoodType("GothLife.Ferryman", 4, 15, 0.15f, FoodRestriction.NotForChildren, FoodTime.None, FoodType.IngredientSource.MixologyKit, ColorHelper.FromHexRgb(0x3dd3d1)));
        }

        public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info) {
            Logger = logger;

            // loads a texture atlas with the given amount of separate texture regions in the x and y axes
            // we submit it to the texture packer to increase rendering performance. The callback is invoked once packing is completed
            //texturePacker.Add(content.Load<Texture2D>("CustomClothes"), r => this.customClothes = new UniformTextureAtlas(r, 4, 6));

            //texturePacker.Add(content.Load<Texture2D>("UiTextures"), r => this.uiTextures = new UniformTextureAtlas(r, 8, 8));
            texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("UiTextures"), 8, 8), r => this.uiTextures = r);
            texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("GothTops"), 8, 11), r => this.gothTops = r);
            texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("GothHats"), 8, 5), r => this.gothHats = r);
            texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("GothShoes"), 12, 6), r => this.gothShoes = r);
            texturePacker.Add(WallMode.ApplyMasks(content.Load<Texture2D>("GothWallpaper"), 4, 5), r => this.gothWallpaper = r);
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