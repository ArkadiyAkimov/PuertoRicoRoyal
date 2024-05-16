using PuertoRicoAPI.Data.DataClasses;
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

            for(int i = 0; i < buildingType.Slots; i++) 
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

            for (int i = 0; i < buildingType.Slots; i++)
            {
                this.Slots[i] = dataBuilding.Slots[i].IsOccupied;
            }
        }

        public Building(Building building)
        {
            this.gs = building.gs;
            this.Type = building.Type;
            this.Slots = new bool[building.Type.Slots];
            this.Quantity = 1;

            for(int i = 0; i < Slots.Length; i++)
            {
                this.Slots[i] = false;
            }
        }

        public GameState gs { get; set; }
        public BuildingType Type { get; set; }
        public bool[] Slots { get; set; }
        public int Quantity { get; set; }

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
            int basePrice = this.Type.Price;
            int quarryDiscountLimit = this.Type.VictoryScore;
            int activeQuarries = this.gs.getCurrPlayer().countActiveQuarries();
            int quarryDiscount = Math.Min(quarryDiscountLimit, activeQuarries);
            basePrice -= quarryDiscount;

            if(this.gs.getCurrPlayer().CheckForPriviledge()) basePrice-- ;

            return basePrice;
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
