using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Channels;
using static PuertoRicoAPI.Utility;

namespace PuertoRicoAPI.Types
{
    public class BuildingType
    {
        public BuildingName Name { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public GoodType Good { get; set; }
        public ColorName Color { get; set; }
        public int Price { get; set; }
        public int VictoryScore { get; set; }
        public bool IsProduction { get; set; }
        public int Slots { get; set; }
        public int size { get; set; }
        public int StartingQuantity { get; set; }
        public int Expansion { get; set; }
    }

    public enum BuildingName
    {
        SmallIndigoPlant,
        SmallSugarMill,
        SmallMarket,
        Aqueduct,//1
        Hacienda,
        ConstructionHut,
        ForestHouse,//1
        BlackMarket,//1
        SmallWarehouse, 
        Storehouse,//1
        LandOffice,//2
        Chapel,//2
        LargeIndigoPlant,
        LargeSugarMill,
        Hospice,
        GuestHouse,//1
        Office,
        LargeMarket,
        TradingPost,//1
        Church,//1
        LargeWarehouse,
        SmallWharf,//1
        HuntingLodge,//2
        ZoningOffice,//2
        RoyalSupplier,//2
        TobaccoStorage,
        CoffeeRoaster,
        Univercity,
        Lighthouse,//1
        Factory,
        Harbor,
        SpecialtyFactory,//1
        Library,//1
        Wharf,
        UnionHall,//1
        Villa,//2
        Jeweler,//2
        GuildHall,
        Residence,
        Fortress,
        CustomsHouse,
        CityHall,
        Statue,//1
        Cloister,//1
        RoyalGarden,//2
    }

    public static class BuildingTypes
    {
        public static BuildingName[] ProdBuildings =
   {
        BuildingName.SmallIndigoPlant,
        BuildingName.SmallSugarMill,
        BuildingName.LargeIndigoPlant,
        BuildingName.LargeSugarMill,
        BuildingName.TobaccoStorage,
        BuildingName.CoffeeRoaster,
    };

        public static List<BuildingType> getAll()
        {
            return new List<BuildingType> 
            {
                smallIndigoPlant,
                smallSugarMill,
                smallMarket,
                aqueduct,//1
                hacienda,
                constructionHut,
                forestHouse,//1
                blackMarket,//1
                smallWarehouse,
                storehouse,//1
                landOffice,//2
                chapel,//2
                largeIndigoPlant,
                largeSugarMill,
                hospice,
                guestHouse,//1
                office,
                largeMarket,
                tradingPost,//1
                church,//1
                largeWarehouse,
                smallWharf,//1
                huntingLodge,//2
                zoningOffice,//2
                royalSupplier,//2
                tobaccoStorage,
                coffeeRoaster,
                univercity,
                lighthouse,//1
                factory,
                harbor,
                specialtyFactory,//1
                library,//1
                wharf,
                unionHall,//1
                villa,//2
                jeweler,//2
                guildHall,
                residence,
                fortress,
                customsHouse,
                cityHall,
                statue,//1
                cloister,//1
                royalGarden,//2
            };
        }

        public static BuildingType getBuildingType(BuildingName buildingName)
        {
            return getAll()[(int)buildingName];
        }

        public static BuildingType smallIndigoPlant { get; } = new BuildingType
        {
            Name = BuildingName.SmallIndigoPlant,
            DisplayName = "small indigo plant",
            Good = GoodType.Indigo,
            Color = ColorName.blue,
            Price = 1,
            VictoryScore = 1,
            IsProduction = true,
            Slots = 1,
            size = 1,
            StartingQuantity = 4,
            Expansion = 0,
        };

        public static BuildingType smallSugarMill { get; } = new BuildingType
        {
            Name = BuildingName.SmallSugarMill,
            DisplayName = "small sugar mill",
            Good = GoodType.Sugar,
            Color = ColorName.white,
            Price = 2,
            VictoryScore = 1,
            IsProduction = true,
            Slots = 1,
            size = 1,
            StartingQuantity = 4,
            Expansion = 0,
        };

        public static BuildingType largeIndigoPlant { get; } = new BuildingType
        {
            Name = BuildingName.LargeIndigoPlant,
            DisplayName = "large indigo plant",
            Good = GoodType.Indigo,
            Color = ColorName.blue,
            Price = 3,
            VictoryScore = 2,
            IsProduction = true,
            Slots = 3,
            size = 1,
            StartingQuantity = 3,
            Expansion = 0,
        };

        public static BuildingType largeSugarMill { get; } = new BuildingType
        {
            Name = BuildingName.LargeSugarMill,
            DisplayName = "large sugar mill",
            Good = GoodType.Sugar,
            Color = ColorName.white,
            Price = 4,
            VictoryScore = 2,
            IsProduction = true,
            Slots = 3,
            size = 1,
            StartingQuantity = 3,
            Expansion = 0,
        };

        public static BuildingType tobaccoStorage { get; } = new BuildingType
        {
            Name = BuildingName.TobaccoStorage,
            DisplayName = "tobacco storage",
            Good = GoodType.Tobacco,
            Color = ColorName.burlywood,
            Price = 5,
            VictoryScore = 3,
            IsProduction = true,
            Slots = 3,
            size = 1,
            StartingQuantity = 3,
            Expansion = 0,
        };

        public static BuildingType coffeeRoaster { get; } = new BuildingType
        {
            Name = BuildingName.CoffeeRoaster,
            DisplayName = "coffee roaster",
            Good = GoodType.Coffee,
            Color = ColorName.black,
            Price = 6,
            VictoryScore = 3,
            IsProduction = true,
            Slots = 2,
            size = 1,
            StartingQuantity = 3,
            Expansion = 0,
        };

        public static BuildingType smallMarket { get; } = new BuildingType
        {
            Name = BuildingName.SmallMarket,
            DisplayName = "small market",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 1,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType hacienda { get; } = new BuildingType
        {
            Name = BuildingName.Hacienda,
            DisplayName = "hacienda",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 2,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType constructionHut { get; } = new BuildingType
        {
            Name = BuildingName.ConstructionHut,
            DisplayName = "construction hut",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 2,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType smallWarehouse { get; } = new BuildingType
        {
            Name = BuildingName.SmallWarehouse,
            DisplayName = "small warehouse",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 3,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType hospice { get; } = new BuildingType
        {
            Name = BuildingName.Hospice,
            DisplayName = "hospice",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 4,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType office { get; } = new BuildingType
        {
            Name = BuildingName.Office,
            DisplayName = "office",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 5,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType largeMarket { get; } = new BuildingType
        {
            Name = BuildingName.LargeMarket,
            DisplayName = "large market",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 5,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType largeWarehouse { get; } = new BuildingType
        {
            Name = BuildingName.LargeWarehouse,
            DisplayName = "large warehouse",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 6,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType univercity { get; } = new BuildingType
        {
            Name = BuildingName.Univercity,
            DisplayName = "univercity",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 7,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType factory { get; } = new BuildingType
        {
            Name =BuildingName.Factory,
            DisplayName = "factory",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 8,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType harbor { get; } = new BuildingType
        {
            Name = BuildingName.Harbor,
            DisplayName = "harbor",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 8,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType wharf { get; } = new BuildingType
        {
            Name = BuildingName.Wharf,
            DisplayName = "wharf",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 9,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 0,
        };

        public static BuildingType guildHall { get; } = new BuildingType
        {
            Name = BuildingName.GuildHall,
            DisplayName = "guild hall",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 2,
            StartingQuantity = 1,
            Expansion = 0,
        };

        public static BuildingType residence { get; } = new BuildingType
        {
            Name = BuildingName.Residence,
            DisplayName = "residence",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 2,
            StartingQuantity = 1,
            Expansion = 0,
        };

        public static BuildingType fortress { get; } = new BuildingType
        {
            Name = BuildingName.Fortress,
            DisplayName = "fortress",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 2,
            StartingQuantity = 1,
            Expansion = 0,
        };

        public static BuildingType customsHouse { get; } = new BuildingType
        {
            Name = BuildingName.CustomsHouse,
            DisplayName = "customs house",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 2,
            StartingQuantity = 1,
            Expansion = 0,
        };

        public static BuildingType cityHall { get; } = new BuildingType
        {
            Name = BuildingName.CityHall,
            DisplayName = "city hall",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 2,
            StartingQuantity = 1,
            Expansion = 0,
        };

        //expansion 1 buildings

        public static BuildingType aqueduct { get; } = new BuildingType
        {
            Name = BuildingName.Aqueduct,
            DisplayName = "aqueduct",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 1,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType forestHouse { get; } = new BuildingType
        {
            Name = BuildingName.ForestHouse,
            DisplayName = "forest house",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 2,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType blackMarket { get; } = new BuildingType
        {
            Name = BuildingName.BlackMarket,
            DisplayName = "black market",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 2,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType storehouse { get; } = new BuildingType
        {
            Name = BuildingName.Storehouse,
            DisplayName = "storehouse",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 3,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType guestHouse { get; } = new BuildingType
        {
            Name = BuildingName.GuestHouse,
            DisplayName = "guest house",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 4,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 2,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType tradingPost { get; } = new BuildingType
        {
            Name = BuildingName.TradingPost,
            DisplayName = "trading post",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 5,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType church { get; } = new BuildingType
        {
            Name = BuildingName.Church,
            DisplayName = "church",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 5,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType smallWharf { get; } = new BuildingType
        {
            Name = BuildingName.SmallWharf,
            DisplayName = "small wharf",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 6,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType lighthouse { get; } = new BuildingType
        {
            Name = BuildingName.Lighthouse,
            DisplayName = "lighthouse",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 7,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType specialtyFactory { get; } = new BuildingType
        {
            Name = BuildingName.SpecialtyFactory,
            DisplayName = "specialty factory",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 8,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType library { get; } = new BuildingType
        {
            Name = BuildingName.Library,
            DisplayName = "library",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 8,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType unionHall { get; } = new BuildingType
        {
            Name = BuildingName.UnionHall,
            DisplayName = "union hall",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 9,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 1,
        };

        public static BuildingType statue { get; } = new BuildingType
        {
            Name = BuildingName.Statue,
            DisplayName = "statue",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 8,
            IsProduction = false,
            Slots = 0,
            size = 2,
            StartingQuantity = 1,
            Expansion = 1,
        };

        public static BuildingType cloister { get; } = new BuildingType
        {
            Name = BuildingName.Cloister,
            DisplayName = "cloister",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 2,
            StartingQuantity = 1,
            Expansion = 1,
        };


        //expansion 2 nobles

        public static BuildingType landOffice { get; } = new BuildingType
        {
            Name = BuildingName.LandOffice,
            DisplayName = "land office",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 2,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType chapel { get; } = new BuildingType
        {
            Name = BuildingName.Chapel,
            DisplayName = "chapel",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 3,
            VictoryScore = 1,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType huntingLodge { get; } = new BuildingType
        {
            Name = BuildingName.HuntingLodge,
            DisplayName = "hunting lodge",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 4,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType zoningOffice { get; } = new BuildingType
        {
            Name = BuildingName.ZoningOffice,
            DisplayName = "zoning office",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 5,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType royalSupplier { get; } = new BuildingType
        {
            Name = BuildingName.RoyalSupplier,
            DisplayName = "royal supplier",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 6,
            VictoryScore = 2,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType villa { get; } = new BuildingType
        {
            Name = BuildingName.Villa,
            DisplayName = "villa",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 7,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType jeweler { get; } = new BuildingType
        {
            Name = BuildingName.Jeweler,
            DisplayName = "jeweler",
            Good = GoodType.NoType,
            Color = ColorName.red,
            Price = 8,
            VictoryScore = 3,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
            Expansion = 2,
        };

        public static BuildingType royalGarden { get; } = new BuildingType
        {
            Name = BuildingName.RoyalGarden,
            DisplayName = "royal garden",
            Good = GoodType.NoType,
            Color = ColorName.violet,
            Price = 10,
            VictoryScore = 4,
            IsProduction = false,
            Slots = 1,
            size = 1,
            StartingQuantity = 2,
        };
    }
}
