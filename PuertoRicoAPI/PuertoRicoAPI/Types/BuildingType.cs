using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
    }

    public enum BuildingName
    {
        SmallIndigoPlant,
        SmallSugarMill,
        SmallMarket,
        Hacienda,
        ConstructionHut,
        SmallWarehouse, 
        LargeIndigoPlant,
        LargeSugarMill,
        Hospice,
        Office,
        LargeMarket,
        LargeWarehouse,
        TobaccoStorage,
        CoffeeRoaster,
        Univercity,
        Factory,
        Harbor,
        Wharf,
        GuildHall,
        Residence,
        Fortress,
        CustomsHouse,
        CityHall,
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
                SmallIndigoPlant,
                smallSugarMill,
                smallMarket,
                hacienda,
                constructionHut,
                smallWarehouse,
                largeIndigoPlant,
                largeSugarMill,
                hospice,
                office,
                largeMarket,
                largeWarehouse,
                tobaccoStorage,
                coffeeRoaster,
                univercity,
                factory,
                harbor,
                wharf,
                guildHall,
                residence,
                fortress,
                customsHouse,
                cityHall
            };
        }

        public static BuildingType getBuildingType(BuildingName buildingName)
        {
            return getAll()[(int)buildingName];
        }

        public static BuildingType SmallIndigoPlant { get; } = new BuildingType
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
        };
    }
}
