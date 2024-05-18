using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

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

            int budget = 0;
            bool canBuild = false;

            Player player = gs.getCurrPlayer();

            budget += player.countActiveQuarries();
            budget += player.Doubloons;
            if (player.CheckForPriviledge()) budget++;

            foreach (Building building in gs.Buildings)
            {
                if (!player.hasBuilding(building.Type.Name)
                    && building.Quantity > 0
                    && budget >= building.Type.Price) canBuild = true;
            }

            if (!canBuild)
            {
                Console.WriteLine("player {0} can't build anything skipping turn", player.Index);
                this.mainLoop();
            }

        }

        public override void endRole()
        {
            base.endRole();
        }

        public static bool tryBuyBuilding(Building building, GameState gs)
        {
            int buildingPrice = building.getBuildingPrice();

            if (!canBuyBuilding(building, buildingPrice, gs)) return false;

            building.Quantity--;

            gs.getCurrPlayer().chargePlayer(buildingPrice);
            if (building is ProdBuilding) gs.getCurrPlayer().Buildings.Add(new ProdBuilding(building as ProdBuilding));
            else gs.getCurrPlayer().Buildings.Add(new Building(building));

            if (gs.getCurrPlayer().hasBuilding(BuildingName.Univercity, true))
            {
                gs.getCurrPlayer().Buildings.Last().Slots[0] = true;
            }

            if (gs.getCurrPlayer().freeBuildingTiles() == 0) gs.LastGovernor = true;

            gs.getCurrentRole().mainLoop();

            return true;
        }

        public static bool canBuyBuilding(Building building, int buildingPrice, GameState gs)
        {
            Console.WriteLine("info: currentRole {0}, Doubloons {1}, HasBuilding {2}, freeBuildingSlots {3}."
                , gs.CurrentRole, gs.getCurrPlayer().Doubloons, gs.getCurrPlayer().hasBuilding(building.Type.Name)
                , gs.getCurrPlayer().freeBuildingTiles());

            if (gs.CurrentRole != RoleName.Builder) return false;
            if (gs.getCurrPlayer().Doubloons < buildingPrice) return false;
            if (gs.getCurrPlayer().hasBuilding(building.Type.Name)) return false;
            if (gs.getCurrPlayer().freeBuildingTiles() < building.Type.size) return false;
            return true;
        }
    }
}
