using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
using System.Numerics;

namespace PuertoRicoAPI.Model.Roles
{
    public class Settler : Role
    {
        public Settler(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {
            if (this.IsFirstIteration)
            {
                this.initializeBuildingEffects(BuildingName.Hacienda,true);
                this.initializeBuildingEffects(BuildingName.Hospice, false);
                this.initializeBuildingEffects(BuildingName.Library, true);
                this.initializeBuildingEffects(BuildingName.HuntingLodge, true);
            }


            base.mainLoop();
            if (gs.CurrentRole != Name) return;


            if (gs.countExposedPlantations() == 0) DrawPlantations();
        }

        public override void endRole()
        {
            Player player = gs.getCurrPlayer();

            if (player.hasActiveBuilding(BuildingName.Library)
                && player.getBuilding(BuildingName.Library).EffectAvailable)
            {
                player.getBuilding(BuildingName.Library).EffectAvailable = false;
                return;
            }

            foreach(Player x in gs.Players)
            {
                RewardForHuntingLodge(x);
            }

            gs.Plantations.Where(x => x.IsExposed).ToList().ForEach(plantation =>
            {
                plantation.IsExposed = false;
                plantation.IsDiscarded = true;
            });

            DrawPlantations();
            base.endRole();
            
        }

        public void DrawPlantations()
        {
            Random rnd = new Random();
            var totalPlantCount = gs.Plantations.Sum(x => !x.IsExposed && !x.IsDiscarded ? 1 : 0);
            var legalPlantCount = Math.Min(totalPlantCount, gs.Players.Count + 1);


            var exposed = this.gs.Plantations
                .Where(x => x.IsDiscarded == false)
                .OrderBy(x => rnd.Next()).Take(legalPlantCount).ToList();

            if (legalPlantCount < gs.Players.Count + 1)
            {
                gs.Plantations.Where(x => x.IsDiscarded).ToList().ForEach(plantation =>
                {
                    plantation.IsDiscarded = false;
                });

                exposed.AddRange(this.gs.Plantations
                .Where(x => x.IsDiscarded == false)
                .OrderBy(x => rnd.Next()).Take(this.gs.Players.Count + 1 - legalPlantCount));
            }


            foreach (var plant in exposed)
            {
                plant.IsExposed = true;
            }
        }

        public bool CanTakeUpSideDown()
        {
            Player player = gs.getCurrPlayer();

            return player.hasActiveBuilding(BuildingName.Hacienda)
                && player.getBuilding(BuildingName.Hacienda).EffectAvailable
                && CanTakePlantation();
        }

        public bool CanTakeQuarry()
        {
            Player player = gs.getCurrPlayer();

            return (player.hasActiveBuilding(BuildingName.ConstructionHut)
                 || player.CheckForPriviledge())
                 && CanTakePlantation();
        }

        public bool CanTakeForest()
        {
            Player player = gs.getCurrPlayer();

            return player.hasActiveBuilding(BuildingName.ForestHouse)
                && CanTakePlantation();
        }

        public bool CanTakePlantation()
        {
            return (gs.getCurrPlayer().freePlantationTiles() > 0);
        }

      

        public bool CheckUsedHacienda()
        {
            Player player = this.gs.getCurrPlayer();
            return player.hasActiveBuilding(BuildingName.Hacienda)
                    && !player.getBuilding(BuildingName.Hacienda).EffectAvailable;
        }

        public bool CheckUsedHospice()
        {
            Player player = this.gs.getCurrPlayer();
            return player.hasActiveBuilding(BuildingName.Hospice)
                   && !player.getBuilding(BuildingName.Hospice).EffectAvailable;
        }

        public void RewardForHuntingLodge(Player player)
        {

            if (player.hasActiveBuilding(BuildingName.HuntingLodge)
                && player.getBuilding(BuildingName.HuntingLodge).Slots[0] == SlotEnum.Noble)
            {
                bool leastOccupiedSpaces = true;

                foreach (Player opponent in gs.Players)
                {
                    if ((opponent.Index != player.Index)
                        && (opponent.Plantations.Count() <= player.Plantations.Count()))
                    {
                        leastOccupiedSpaces = false;
                    }
                }

                if (leastOccupiedSpaces)
                {
                    player.VictoryPoints += 2;
                    gs.VictoryPointSupply -= 2;
                }
            }
        }

        public void TakePlantation(DataPlantation dataPlantation,bool tookForest)
        {
            Plantation newPlantation;
            Player player = gs.getCurrPlayer();


            if (dataPlantation != null && dataPlantation.IsExposed && !tookForest)//exposed
            {

                newPlantation = new Plantation(dataPlantation);
                Plantation removedPlantation = this.gs.Plantations
                    .FirstOrDefault(plantation => plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                this.gs.Plantations.Remove(removedPlantation);
                player.Plantations.Add(newPlantation);

                player.TookTurn = true;

                if (player.hasActiveBuilding(BuildingName.Library)
                  && !player.getBuilding(BuildingName.Library).EffectAvailable)
                {
                    this.endRole();
                    return;
                }

                if (player.hasActiveBuilding(BuildingName.Hospice))
                {
                    if (player.hasActiveBuilding(BuildingName.Hacienda)
                        && !player.getBuilding(BuildingName.Hacienda).EffectAvailable) this.mainLoop();
                    Console.WriteLine("player {0} hospice enabled.", player.Index);
                    player.getBuilding(BuildingName.Hospice).EffectAvailable = true;
                    return;
                }
                else this.mainLoop();
            }
            else if (dataPlantation != null && dataPlantation.IsExposed && tookForest)//forest
            {

                newPlantation = new Plantation();
                newPlantation.Good = GoodType.Forest;
                newPlantation.SlotState = SlotEnum.Colonist;

                Plantation removedPlantation = this.gs.Plantations
                    .FirstOrDefault(plantation => plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                //this.gs.Plantations.Remove(removedPlantation); 
                removedPlantation.IsDiscarded = true;
                removedPlantation.IsExposed = false;
                player.Plantations.Add(newPlantation);

                player.TookTurn = true;

                if (player.hasActiveBuilding(BuildingName.Library)
                  && !player.getBuilding(BuildingName.Library).EffectAvailable)
                {
                    this.endRole();
                    return;
                }

                this.mainLoop();
            }
            else if (dataPlantation != null && !dataPlantation.IsExposed && tookForest)//buying forest
            {

                newPlantation = new Plantation();
                newPlantation.Good = GoodType.Forest;
                newPlantation.SlotState = SlotEnum.Colonist;

                Plantation removedPlantation = this.gs.Plantations
                    .FirstOrDefault(plantation => !plantation.IsExposed
                    && (plantation.Good == dataPlantation.Good));

                //this.gs.Plantations.Remove(removedPlantation); 
                removedPlantation.IsDiscarded = true;
                player.Plantations.Add(newPlantation);

                player.TookTurn = true;
                return;
            }
            else if (dataPlantation != null) //upside down
            {

                newPlantation = new Plantation(dataPlantation);
                Plantation removedPlantation = this.gs.Plantations
                    .FirstOrDefault(plantation => !plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                this.gs.Plantations.Remove(removedPlantation);
                player.Plantations.Add(newPlantation);


                if (gs.CurrentRole == RoleName.Settler)
                {
                    player.getBuilding(BuildingName.Hacienda).EffectAvailable = false;

                    if (player.hasActiveBuilding(BuildingName.Hospice))
                    {
                        Console.WriteLine("player {0} hospice enabled.", player.Index);
                        player.getBuilding(BuildingName.Hospice).EffectAvailable = true;
                    }
                    return;
                }
            }
            else  //quarry
            {
                this.gs.QuarryCount--;
                newPlantation = new Plantation();
                player.Plantations.Add(newPlantation);

                player.TookTurn = true;
                
                if (player.hasActiveBuilding(BuildingName.Hospice))
                {
                    if (player.hasActiveBuilding(BuildingName.Hacienda)
                        && !player.getBuilding(BuildingName.Hacienda).EffectAvailable) this.mainLoop();
                    Console.WriteLine("player {0} hospice enabled.", player.Index);
                    player.getBuilding(BuildingName.Hospice).EffectAvailable = true;
                    return;
                }
                else this.mainLoop();
            }
        }
    }
}
