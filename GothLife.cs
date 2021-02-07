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

        //private UniformTextureAtlas customClothes;
        public ColorScheme darknessScheme;

        public override void AddGameContent(GameImpl game) {

            darknessScheme = ColorScheme.Create(0x101010, 0x575757, 0x8a0900, 0x390075, 0x0f450a, 0xfffeed);

            // adding a custom furniture item
            FurnitureType.Register(new FurnitureType.TypeSettings("GothLife.Candle", new Point(1, 1), ObjectCategory.SmallObject, 15, darknessScheme, ColorScheme.White)
            {
                DecorativeRating = f => 1,
                Construct = (i, t, c, m, p) => new FurnitureCandle(i, t, c, m, p)
            }) ;

            /*
            // adding custom clothing
            Clothes.Register(new Clothes("ExampleMod.DarkShirt", ClothesLayer.Shirt,
                this.customClothes[0, 0], // the top left in-world region (the rest will be auto-gathered from the atlas)
                ColorScheme.WarmDark));
            */
            JobType.Register(new JobType("GothLife.Gravekeeper", 18f, new MonoGame.Extended.Range<int>(0, 8), System.DayOfWeek.Tuesday, System.DayOfWeek.Thursday));
            JobType.Register(new JobType("GothLife.Mortician", 25f, new MonoGame.Extended.Range<int>(10, 18), System.DayOfWeek.Monday, System.DayOfWeek.Wednesday, System.DayOfWeek.Tuesday));

            Cooking.RegisterFoodType(new Cooking.FoodType("GothLife.PBJ", 0, 5, 80));
            Cooking.RegisterFoodType(new Cooking.FoodType("GothLife.BlackRice", 1, 7, 90));
        }

        public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker) {
            Logger = logger;
            
            // loads a texture atlas with the given amount of separate texture regions in the x and y axes
            // we submit it to the texture packer to increase rendering performance. The callback is invoked once packing is completed
            //texturePacker.Add(content.Load<Texture2D>("CustomClothes"), r => this.customClothes = new UniformTextureAtlas(r, 4, 6));
        }

        public override IEnumerable<string> GetCustomFurnitureTextures() {
            // tell the game about our custom furniture texture
            // this needs to be a path to a data texture atlas, relative to our "Content" directory
            // the texture atlas combines the png texture and the .atlas information
            // see https://mlem.ellpeck.de/api/MLEM.Data.DataTextureAtlas.html for more info
            yield return "GothLifeFurniture";
        }

    }
}