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
            this.Slots = new SlotEnum[buildingType.Slots];
            this.Quantity = dataBuilding.Quantity;
            this.isDrafted = dataBuilding.isDrafted;
            this.isBlocked = dataBuilding.isBlocked;

            for (int i = 0; i < buildingType.Slots; i++) 
            {
                this.Slots[i] = dataBuilding.Slots[i].State;
            }
        }

        public Building(DataPlayerBuilding dataBuilding, GameState gs)
        {
            this.gs = gs;
            BuildingType buildingType = BuildingTypes.getBuildingType(dataBuilding.Name);
            this.Type = buildingType;
            this.Slots = new SlotEnum[buildingType.Slots];
            this.Quantity = dataBuilding.Quantity;
            this.BuildOrder = dataBuilding.BuildOrder;
            this.EffectAvailable = dataBuilding.EffectAvailable;

            for (int i = 0; i < buildingType.Slots; i++)
            {
                this.Slots[i] = dataBuilding.Slots[i].State;
            }
        }

        public Building(Building building)   //this is where player buildings are initialized
        {
            this.gs = building.gs;
            this.Type = building.Type;
            this.Slots = new SlotEnum[building.Type.Slots];
            this.Quantity = 1;
            this.BuildOrder = 0; //updated when added to player
            this.EffectAvailable = false; 

            for(int i = 0; i < Slots.Length; i++)
            {
                this.Slots[i] = SlotEnum.Vacant;
            }
        }

        public GameState gs { get; set; }
        public BuildingType Type { get; set; }
        public SlotEnum[] Slots { get; set; }
        public int Quantity { get; set; }
        public int BuildOrder { get; set; }
        public bool EffectAvailable { get; set; }
        public bool isDrafted { get; set; }
        public bool isBlocked { get; set; }

        public int freeSlots()
        {
            int freeSlots = 0;
            foreach(SlotEnum slot in Slots)
            {
                if(slot == SlotEnum.Vacant) freeSlots++;
            }
            return freeSlots;
        }

        public int getBuildingPrice(int guestHouseBonusQuarries = 0)
        {
            Player player = gs.getCurrPlayer();
            int basePrice = this.Type.Price;
            int quarryDiscountLimit = Math.Min(this.Type.VictoryScore, 4);
            int activeQuarries = player.countActiveQuarries() + guestHouseBonusQuarries;  //guesthouse bonus is 0 by default but that one time we need to calculate skip builder we need to consider guesthouse quarry activation 
            int forests = player.countForests();
            int quarryDiscount = Math.Min(quarryDiscountLimit, activeQuarries);
            basePrice -= quarryDiscount;
            basePrice -= (int)Math.Floor((double)forests / 2);

            if (player.hasActiveBuilding(BuildingName.ZoningOffice))
            {
                if (player.getBuilding(BuildingName.ZoningOffice).isNobleOccupied())
                {
                    if(this.Type.size == 2) basePrice -= 2;
                } else
                {
                    if (this.Type.size == 1) basePrice--;
                }
            }

            if (player.CheckForPriviledge())
            {   
                basePrice--;
                if (player.hasActiveBuilding(BuildingName.Library)) basePrice--;
            }

            return Math.Max(basePrice,0);
        }

        public bool isOccupied()
        {
            return this.Slots[0] != SlotEnum.Vacant;
        }

        public bool isNobleOccupied()
        {
            return this.Slots[0] == SlotEnum.Noble;
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
