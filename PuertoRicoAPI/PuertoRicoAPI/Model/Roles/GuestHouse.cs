using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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


            if (player.hasActiveBuilding(BuildingName.GuestHouse) && isGuestHouseUseful()) return;
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

            int guests = 0;

            if (player.hasActiveBuilding(BuildingName.GuestHouse)){                // count guests
                guests = player.getBuilding(BuildingName.GuestHouse).Slots.Count(x => x);
            }
            
            BuildingName[] checkUnoccupiedBuildingNames = new BuildingName[] { };
            BuildingName[] checkPossesionBuildingNames = new BuildingName[] { };

            switch (gs.GuestHouseNextRole)
            {
                case RoleName.Mayor:
                    checkUnoccupiedBuildingNames.Append(BuildingName.Villa);
                    break;
                case RoleName.Captain:
                    checkUnoccupiedBuildingNames.Append(BuildingName.UnionHall);
                    break;
                case RoleName.Craftsman:
                    if (checkIfHasUnoccupiedPlantationsOfType(GoodType.Corn)) return true; // if has unoccupied corn its game


                    if (hasPlantationsOfType(GoodType.Indigo) && (inPossesionOfBuildingFromArray(new BuildingName[] { BuildingName.LargeIndigoPlant, BuildingName.SmallIndigoPlant })))
                    {
                            if(canUpGoodProductionUsingGuesthouse(GoodType.Indigo,guests)) return true;
                            checkUnoccupiedBuildingNames.Append(BuildingName.Aqueduct);
                    }
                    if (hasPlantationsOfType(GoodType.Sugar) && (inPossesionOfBuildingFromArray(new BuildingName[] { BuildingName.LargeSugarMill, BuildingName.SmallSugarMill })))
                    {
                            if (canUpGoodProductionUsingGuesthouse(GoodType.Sugar, guests)) return true;
                            checkUnoccupiedBuildingNames.Append(BuildingName.Aqueduct);
                    }
                    if (hasPlantationsOfType(GoodType.Tobacco) && (inPossesionOfBuildingFromArray(new BuildingName[] { BuildingName.TobaccoStorage })))
                    {
                            if (canUpGoodProductionUsingGuesthouse(GoodType.Tobacco, guests)) return true;
                    }
                    if (hasPlantationsOfType(GoodType.Coffee) && (inPossesionOfBuildingFromArray(new BuildingName[] { BuildingName.CoffeeRoaster })))
                    {
                            if (canUpGoodProductionUsingGuesthouse(GoodType.Coffee, guests)) return true;
                    }
                    
                    checkUnoccupiedBuildingNames.Append(BuildingName.Factory);
                    checkUnoccupiedBuildingNames.Append(BuildingName.SpecialtyFactory);
                    break;
            }

            return checkForAtLeastOneUnoccupiedBuildingFromArray(checkUnoccupiedBuildingNames);
        }

        public bool canUpGoodProductionUsingGuesthouse(GoodType goodType,int guests)
        {
            Player player = this.gs.getCurrPlayer();

            int totalPlantationSlots = 0;
            int totalBuildingSlots = 0;
            int vacantPlantationSlots = 0;
            int vacantBuildingSlots = 0;

            foreach (Building building in player.Buildings)
            {
                if (building.Type.Good == goodType)
                {
                    totalBuildingSlots += building.Slots.Count();
                    vacantBuildingSlots += building.Slots.Count(slot => slot == false);
                }
            }

            foreach (Plantation plantation in player.Plantations)
            {
                if (plantation.Good == goodType)
                {
                    totalPlantationSlots++;
                    if (plantation.IsOccupied == false) vacantPlantationSlots++;
                }
            }

            int occupiedBuildingSlots = totalBuildingSlots - vacantBuildingSlots; //used manually entered prodLimit first but realized total building slots are same and even better..
            int occupiedPlantationSlots = Math.Min(totalPlantationSlots - vacantPlantationSlots, totalBuildingSlots); //ignore any above totalbuildingSlots == production limit
            int differenceInOccupiedSlots = Math.Max(occupiedBuildingSlots, occupiedPlantationSlots) - Math.Min(occupiedBuildingSlots, occupiedPlantationSlots); //find how many colonists can make a difference

            bool isVacantPair = ((vacantPlantationSlots > 0) && (vacantBuildingSlots > 0));
            bool isNotAtMaxProduction = (Math.Max(occupiedBuildingSlots, occupiedPlantationSlots) < totalBuildingSlots);

            Console.WriteLine("good: {0}, isVacantPair: {1}, isAtMaxProduction: {2}, differenceInOccupiedSlots: {3} player: {4}.", goodType, isVacantPair, !isNotAtMaxProduction, differenceInOccupiedSlots, player.Index + 1);

            if (differenceInOccupiedSlots > 0) return true; // if 1 colonist can make a difference.
            else if (differenceInOccupiedSlots == 0 && isNotAtMaxProduction && isVacantPair && (guests == 2)) return true; //if no differecne but not max production, and theres 2 guests, and 2 vacant corresponding slots, can occupy both a building and a plant.
                                                                                                                          

            return false;
        }

        public bool hasPlantationsOfType(GoodType goodType, bool andIsOccupied= false)
        {
            Player player = gs.getCurrPlayer();

            foreach(Plantation plantation in player.Plantations)
            {
                if (plantation.Good == goodType)
                {
                    if (andIsOccupied) return plantation.IsOccupied;
                    else return true;
                }
            }

            return false;
        }

        public bool inPossesionOfBuildingFromArray(BuildingName[] buildingNames)
        {
            Player player = this.gs.getCurrPlayer();

            foreach (BuildingName buildingName in buildingNames)
            {
                if (player.hasVacancyAtBuilding(buildingName) || player.hasActiveBuilding(buildingName)) return true;
            }

            return false;
        }

        public bool checkIfHasUnoccupiedPlantationsOfType(GoodType goodType)
        {
            Player player = gs.getCurrPlayer();

            foreach (Plantation plantation in player.Plantations)
            {
                if (plantation.Good == goodType) return (plantation.IsOccupied == false);
            }

            return false;
        }

        public bool checkForAtLeastOneUnoccupiedBuildingFromArray(BuildingName[] buildingNames)
        {
            Player player = this.gs.getCurrPlayer();

            foreach (BuildingName buildingName in buildingNames)
            {
                if (player.hasVacancyAtBuilding(buildingName)) return true;
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
