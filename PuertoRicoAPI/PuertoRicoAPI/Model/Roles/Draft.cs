using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class Draft : Role
    {
        public Draft(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {

            base.mainLoop();
            if (gs.CurrentRole != Name) return;
        }

        public override void endRole()
        {
            gs.CurrentPlayerIndex = 0;
            base.endRole();
        }

        public bool isDraftOver()
        {
            foreach(Building building in gs.Buildings)
            {
                if (!building.isDrafted && !building.isBlocked) return false;
            }
            return true;
        }

        public void draftRandom()
        {
            List<Building> availableBuildings = gs.Buildings.Where(building => !building.isDrafted && !building.isBlocked).ToList();
            if (gs.IsBuildingsExpansion == false)
            {
                foreach (Building building in availableBuildings)
                {
                    if (building.Type.Expansion == 1) building.isBlocked = true;
                }
                availableBuildings = availableBuildings.Where(building => building.Type.Expansion != 1).ToList();
            }
            if (gs.IsNoblesExpansion == false)
            {
                foreach (Building building in availableBuildings)
                {
                    if (building.Type.Expansion == 2) building.isBlocked = true;
                }
                availableBuildings = availableBuildings.Where(building => building.Type.Expansion != 2).ToList();
            }

            Random rnd = new Random();

            int chosenBuildingIndex = rnd.Next(availableBuildings.Count);

            draftBuilding(availableBuildings[chosenBuildingIndex]);

            if (isDraftOver()) this.endRole();
            else this.draftRandom();
        }

        public void draftBuilding(Building building)
        {
            if (IsFirstIteration) mainLoop();

            if (building.isBlocked) return;
            Console.WriteLine("drafting building {0}",building.Type.DisplayName);
            building.isDrafted = true;

            blockBuildings(building);

            if (isDraftOver()) this.endRole();
            else this.mainLoop();
        }

        public void blockBuildings(Building draftedBuilding)
        {
            int draftedBuildingCount = countDraftedBuildings(draftedBuilding);
           
           switch(draftedBuilding.Type.Price)
            {
                case 1:
                case 3:
                case 4:
                case 6:
                case 7:
                case 9:
                    blockBuildingsAtCost(1, draftedBuildingCount, draftedBuilding);
                    break;
                case 2:
                case 5:
                case 8:
                    blockBuildingsAtCost(2, draftedBuildingCount,draftedBuilding);
                    break;
                case 10:
                    blockBuildingsAtCost(5,draftedBuildingCount, draftedBuilding);
                    break;
            }

           if(draftedBuilding.Type.Name == BuildingName.Hacienda)
            {
                gs.Buildings.FirstOrDefault(building => building.Type.Name == BuildingName.ForestHouse).isBlocked = true;
            }
            if (draftedBuilding.Type.Name == BuildingName.ForestHouse)
            {
                gs.Buildings.FirstOrDefault(building => building.Type.Name == BuildingName.Hacienda).isBlocked = true;
            }

        }

        public void blockBuildingsAtCost(int count,int draftedCount,Building draftedBuilding)
        {
            if (draftedCount == count)
            {
                foreach (Building building in gs.Buildings)
                {
                    if (building.Type.Price == draftedBuilding.Type.Price
                                && building.isDrafted == false
                                && building.Type.Expansion != 2
                                && building.Type.IsProduction == false) building.isBlocked = true;
                }
            }
        }

        public int countDraftedBuildings(Building draftedBuilding)
        {
           return gs.Buildings.Count(building => building.isDrafted
                                              && building.Type.Price == draftedBuilding.Type.Price
                                              && building.Type.Expansion != 2
                                              && building.Type.IsProduction == false);
        }

    }
}
