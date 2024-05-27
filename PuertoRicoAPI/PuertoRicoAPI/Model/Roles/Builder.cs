using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
using System.Numerics;

namespace PuertoRicoAPI.Model.Roles
{
    public class Builder : Role
    {
        public Builder(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {

            base.mainLoop();
            if (gs.CurrentRole != Name) return;


            checkBuilderSkip();
        }

        public override void endRole()
        {
            base.endRole();
        }

        public void checkBuilderSkip()
        {
          
            foreach (Building building in gs.Buildings)
            {
                if (canBuyBuilding(building)) return;
            }

            Console.WriteLine("player {0} can't build anything skipping turn", gs.getCurrPlayer().Index);
            this.mainLoop();
        }

      

        public bool tryBuyBuilding(Building building)
        {
            int buildingPrice = building.getBuildingPrice();

            if (!canBuyBuilding(building)) return false;
            building.Quantity--;
            gs.getCurrPlayer().chargePlayer(buildingPrice);


            if (building is ProdBuilding) gs.getCurrPlayer().Buildings.Add(new ProdBuilding(building as ProdBuilding));
            else gs.getCurrPlayer().Buildings.Add(new Building(building));

            if (gs.getCurrPlayer().freeBuildingTiles() == 0) gs.LastGovernor = true;

            if (gs.getCurrPlayer().hasBuilding(BuildingName.Univercity, true))
            {
                gs.getCurrPlayer().TookTurn = true;
                return true;
            }

            if (gs.getCurrPlayer().hasBuilding(BuildingName.Church, true))
            {
                if (building.Type.VictoryScore == 2 || building.Type.VictoryScore == 3) gs.getCurrPlayer().VictoryPoints++;
                if (building.Type.VictoryScore > 3) gs.getCurrPlayer().VictoryPoints += 2;
            }

            gs.getCurrentRole().mainLoop();

            return true;
        }

        public bool canBuyBuilding(Building building)
        {
            Player player = gs.getCurrPlayer();

            if (gs.CurrentRole != RoleName.Builder) return false;

            if (player.hasBuilding(building.Type.Name)) return false;

            if (player.freeBuildingTiles() < building.Type.size) return false;

            int buildingPrice = building.getBuildingPrice();

            buildingPrice += calculateBlackMarketDiscount();

            if (buildingPrice > player.Doubloons) return false;



            return true;
        }

        public int calculateBlackMarketDiscount()
        {
            Player player = gs.getCurrPlayer();

            if (player.hasBuilding(BuildingName.BlackMarket, true))
            {
                int discount = 0;

                discount += player.VictoryPoints > 0 ? 1 : 0;
                discount += colonistForBlackMarket();
                discount += goodForBlackMarket();

                Console.WriteLine("black market available discount: {0}", discount);

                return discount;
            }
            else return 0;
        }

        public int colonistForBlackMarket()
        {
            Player player = gs.getCurrPlayer();

            foreach (Plantation plantation in player.Plantations)
            {
                if (plantation.IsOccupied && plantation.Good != GoodType.Forest) return 1;
            }

            foreach (Building building in player.Buildings)
            {
                if (building.Type.Name != BuildingName.BlackMarket)
                {
                    foreach (bool slot in building.Slots) if (slot) return 1;
                }
            }
            
            return 0;
        }

        public int goodForBlackMarket()
        {
            Player player = gs.getCurrPlayer();
            
            foreach(Good good in player.Goods)
            {
                if(good.Quantity > 0) return 1;
            }

            return 0;

        }
    }
}
