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


        //private UniformTextureAtlas customClothes;
        public ColorScheme darknessScheme;

        public override void AddGameContent(GameImpl game, ModInfo info) {

            darknessScheme = ColorScheme.Create(0x101010, 0x575757, 0x8a0900, 0x390075, 0x0f450a, 0xfffeed);

            // adding a custom furniture item
            FurnitureType.Register(new FurnitureType.TypeSettings("GothLife.Candle", new Point(1, 1), ObjectCategory.SmallObject | ObjectCategory.Lamp, 15, darknessScheme, ColorScheme.White)
            {
                DecorativeRating = f => 1,
                Icon = this.Icon,
                Tab = TinyLife.Tools.FurnitureTool.Tab.Lighting,
                ConstructedType = typeof(FurnitureCandle)
            });

            
            // adding custom clothing
            Clothes.Register(new Clothes("GothLife.SkullShirt", ClothesLayer.Shirt,
                this.gothTops[0, 0], // the top left in-world region (the rest will be auto-gathered from the atlas)
                100, // the price
                ClothesIntention.Everyday | ClothesIntention.Workout | ClothesIntention.Work | ClothesIntention.Party | ClothesIntention.Summer , // the clothes item's use cases
                darknessScheme, ColorScheme.White)
                { 
                    Icon = this.Icon 
                });
            
            JobType.Register(new JobType("GothLife.Gravekeeper", uiTextures[2, 0], 18f, new MonoGame.Extended.Range<int>(0, 8), System.DayOfWeek.Tuesday, System.DayOfWeek.Thursday));
            JobType.Register(new JobType("GothLife.Mortician", uiTextures[3, 0], 25f, new MonoGame.Extended.Range<int>(10, 18), System.DayOfWeek.Monday, System.DayOfWeek.Wednesday, System.DayOfWeek.Tuesday));

            FoodType.Register(new FoodType("GothLife.PBJ", 0, 5, 80, FoodType.FoodIntolerance.None));
            FoodType.Register(new FoodType("GothLife.BlackRice", 1, 7, 90, FoodType.FoodIntolerance.None));
        }

        public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info) {
            Logger = logger;

            // loads a texture atlas with the given amount of separate texture regions in the x and y axes
            // we submit it to the texture packer to increase rendering performance. The callback is invoked once packing is completed
            //texturePacker.Add(content.Load<Texture2D>("CustomClothes"), r => this.customClothes = new UniformTextureAtlas(r, 4, 6));

            texturePacker.Add(content.Load<Texture2D>("UiTextures"), r => this.uiTextures = new UniformTextureAtlas(r, 8, 8));
            texturePacker.Add(content.Load<Texture2D>("GothTops"), r => this.gothTops = new UniformTextureAtlas(r, 8, 8));
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