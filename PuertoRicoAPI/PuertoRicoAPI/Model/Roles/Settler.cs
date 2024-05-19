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
            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            if(gs.getCurrPlayer().hasBuilding(BuildingName.Hacienda, true))
                 { 
                    gs.getCurrPlayer().CanUseHacienda = true;
                 }

            if (gs.countExposedPlantations() == 0) DrawPlantations();
        }

        public override void endRole()
        {
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

            
            foreach ( var plant in exposed )
            {
                plant.IsExposed = true;
            }
        }

        public bool CanTakeUpSideDown()
        {
            var hasActiveHacienda = this.gs
              .getCurrPlayer()
              .hasBuilding(Types.BuildingName.Hacienda, true);

            return (hasActiveHacienda
                && this.gs.getCurrPlayer().CanUseHacienda
                && CanTakePlantation());
        }

        public bool CanTakeQuarry()
        {
           var hasActiveConstructionHut = this.gs
                .getCurrPlayer()
                .hasBuilding(Types.BuildingName.ConstructionHut,true);

           return (hasActiveConstructionHut 
                || this.gs
                   .getCurrPlayer()
                   .CheckForPriviledge()) 
                && CanTakePlantation();
        }

        public bool CanTakePlantation()
        {
            return (gs.getCurrPlayer().freePlantationTiles() > 0);
        }

        public void TakePlantation(DataPlantation dataPlantation)
        {
            Plantation newPlantation;
            Player player = gs.getCurrPlayer();

            if (dataPlantation != null && dataPlantation.IsExposed)//exposed
            {
                newPlantation = new Plantation(dataPlantation);
                var removedPlantation = this.gs.Plantations
                    .First(plantation => plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                this.gs.Plantations.Remove(removedPlantation);

                player.HospiceTargetPlantation = dataPlantation.Good;
            }
            else if (dataPlantation != null) //upside down
            {
                newPlantation = new Plantation(dataPlantation);
                var removedPlantation = this.gs.Plantations
                    .First(plantation => !plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                this.gs.Plantations.Remove(removedPlantation);

                player.CanUseHacienda = false;
                player.HospiceTargetPlantation = dataPlantation.Good;
            }
            else  //quarry
            {
                this.gs.QuarryCount--;
                newPlantation = new Plantation();

                player.HospiceTargetPlantation = GoodType.Quarry;
            }

            player.Plantations.Add(newPlantation);



            //newPlantation.IsOccupied = true;
            //add a var to prevent player from taking more plantations
            //than he should.


            if (player.hasBuilding(BuildingName.Hospice, true)
                && !player.CanUseHospice
                && gs.ColonistsOnShip > 0)
            {
                if (player.hasBuilding(BuildingName.Hacienda, true)
                    && !player.CanUseHacienda
                    && dataPlantation.IsExposed) 
                    {
                    player.CanUseHospice = false;
                    this.mainLoop();
                    } 
                else player.CanUseHospice = true;
            }
            else if (dataPlantation == null || dataPlantation.IsExposed )
            {
                player.CanUseHospice = false;
                this.mainLoop();
            }
        }
    }
}
