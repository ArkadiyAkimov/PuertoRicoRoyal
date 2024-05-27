using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.deployables
{
    public class Building
    {
        public Building(DataBuilding dataBuilding, GameState gs)
        {
            this.gs = gs;
            BuildingType buildingType = BuildingTypes.getBuildingType(dataBuilding.Name);
            this.Type = buildingType;
            this.Slots = new bool[buildingType.Slots];
            this.Quantity = dataBuilding.Quantity;
            this.isDrafted = dataBuilding.isDrafted;
            this.isBlocked = dataBuilding.isBlocked;

            for (int i = 0; i < buildingType.Slots; i++) 
            {
                this.Slots[i] = dataBuilding.Slots[i].IsOccupied;
            }
        }

        public Building(DataPlayerBuilding dataBuilding, GameState gs)
        {
            this.gs = gs;
            BuildingType buildingType = BuildingTypes.getBuildingType(dataBuilding.Name);
            this.Type = buildingType;
            this.Slots = new bool[buildingType.Slots];
            this.Quantity = dataBuilding.Quantity;
            this.BuildOrder = dataBuilding.BuildOrder;
            this.EffectAvailable = dataBuilding.EffectAvailable;

            for (int i = 0; i < buildingType.Slots; i++)
            {
                this.Slots[i] = dataBuilding.Slots[i].IsOccupied;
            }
        }

        public Building(Building building)   //this is where player buildings are initialized
        {
            this.gs = building.gs;
            this.Type = building.Type;
            this.Slots = new bool[building.Type.Slots];
            this.Quantity = 1;
            this.BuildOrder = 0; //updated when added to player
            this.EffectAvailable = false; 

            for(int i = 0; i < Slots.Length; i++)
            {
                this.Slots[i] = false;
            }
        }

        public GameState gs { get; set; }
        public BuildingType Type { get; set; }
        public bool[] Slots { get; set; }
        public int Quantity { get; set; }
        public int BuildOrder { get; set; }
        public bool EffectAvailable { get; set; }
        public bool isDrafted { get; set; }
        public bool isBlocked { get; set; }

        public int freeSlots()
        {
            int freeSlots = 0;
            foreach(bool slot in Slots)
            {
                if(!slot) freeSlots++;
            }
            return freeSlots;
        }

        public int getBuildingPrice()
        {
            Player player = gs.getCurrPlayer();
            int basePrice = this.Type.Price;
            int quarryDiscountLimit = Math.Min(this.Type.VictoryScore,4);
            int activeQuarries = player.countActiveQuarries();
            int forests = player.countForests();
            int quarryDiscount = Math.Min(quarryDiscountLimit, activeQuarries);
            basePrice -= quarryDiscount;
            basePrice -= (int)Math.Floor((double)forests/2);

            if (player.CheckForPriviledge())
            {   
                basePrice--;
                if (player.hasBuilding(BuildingName.Library, true)) basePrice--;
            }

            return basePrice;
        }

        public bool isOccupied()
        {
            return this.Slots[0];
        }
    }

    public class ProdBuilding : Building
    {
        public ProdBuilding(DataBuilding dataBuilding,GameState gs) : base(dataBuilding,gs)
        {
            this.GoodType = BuildingTypes.getBuildingType(dataBuilding.Name).Good;
        }

        public ProdBuilding(ProdBuilding prodBuilding) : base(prodBuilding)
        {
            this.GoodType = prodBuilding.GoodType;
        }
        public GoodType GoodType { get; set; }
    }
}
