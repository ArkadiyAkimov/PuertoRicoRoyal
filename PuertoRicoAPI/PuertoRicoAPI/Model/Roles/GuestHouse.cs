using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
using System.Numerics;

namespace PuertoRicoAPI.Model.Roles
{
    public class GuestHouse : Role
    {

        public GuestHouse(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {
           
        }

        public override void mainLoop()
        {

            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            Player player = this.gs.getCurrPlayer();


            if (player.hasBuilding(BuildingName.GuestHouse, true) && isGuestHouseUseful()) return;
            else mainLoop();
        }

        public override void endRole()
        {
            gs.CurrentPlayerIndex = gs.PrivilegeIndex;
            base.endRole();
            gs.getRole(gs.GuestHouseNextRole).mainLoop();
        }

        public bool isGuestHouseUseful()
        {
            Player player = this.gs.getCurrPlayer();

            BuildingName[] buildingNames = new BuildingName[] { };

            switch (gs.GuestHouseNextRole)
            {
                case RoleName.Mayor:
                    buildingNames.Append(BuildingName.Villa);
                    break;
                case RoleName.Captain:
                    buildingNames.Append(BuildingName.UnionHall);
                    break;
                case RoleName.Craftsman:
                    if (checkIfHasUnoccupiedPlantationsOfType(GoodType.Indigo))
                    {
                        buildingNames.Append(BuildingName.SmallIndigoPlant);
                        buildingNames.Append(BuildingName.LargeIndigoPlant);
                    }
                    if (checkIfHasUnoccupiedPlantationsOfType(GoodType.Sugar))
                    {
                        buildingNames.Append(BuildingName.SmallSugarMill);
                        buildingNames.Append(BuildingName.LargeSugarMill);
                    }
                    if (checkIfHasUnoccupiedPlantationsOfType(GoodType.Tobacco))
                    {
                        buildingNames.Append(BuildingName.TobaccoStorage);
                    }
                    if (checkIfHasUnoccupiedPlantationsOfType(GoodType.Coffee))
                    {
                        buildingNames.Append(BuildingName.CoffeeRoaster);
                    }
                    buildingNames.Append(BuildingName.Aqueduct);
                    buildingNames.Append(BuildingName.Factory);
                    buildingNames.Append(BuildingName.SpecialtyFactory);
                    break;
            }

            return checkForUnoccupiedBuildingFromArray(buildingNames);
        }

        public bool checkIfHasUnoccupiedPlantationsOfType(GoodType goodType)
        {
            Player player = gs.getCurrPlayer();

            foreach(Plantation plantation in player.Plantations)
            {
                if (plantation.Good == goodType) return true;
            }

            return false;
        }

        public bool checkForUnoccupiedBuildingFromArray(BuildingName[] buildingNames)
        {
            Player player = this.gs.getCurrPlayer();

            foreach (BuildingName buildingName in buildingNames)
            {
                if (player.hasBuilding(BuildingName.UnionHall, false)) return true;
            }

            return false;
        }

        public dynamic getTargetFromBuildOrderAndIndex(int[] buildOrderAndIndex)
        {
            Player player = gs.getCurrPlayer();

            if (buildOrderAndIndex != null)
            {
                foreach (Building building in player.Buildings)
                {
                    if (building.BuildOrder == buildOrderAndIndex[0])
                    {
                        return building;
                    }
                }

                foreach (Plantation plantation in player.Plantations)
                {
                    if (plantation.BuildOrder == buildOrderAndIndex[0])
                    {
                        return plantation;
                    }
                }
            }

            return null;
        }

        public void occupyTargetAndactivateBuildingEffect(dynamic target,int index)
        {
            Player player = gs.getCurrPlayer();

            if (target == null) return;

            if(target is Building)
            {
                target.Slots[index] = true;
                player.getBuilding(target.Type.Name).EffectAvailable = true;
                Console.WriteLine("guesthouse activated {0}", target.Type.DisplayName);
            }
            if(target is Plantation)
            {
                target.IsOccupied = true;
                Console.WriteLine("guesthouse occupied {0} plantation", target.Good);
            }
           
        }

    }
}
