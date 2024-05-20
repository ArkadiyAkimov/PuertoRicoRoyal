﻿using PuertoRicoAPI.Data.DataClasses;
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
                Console.WriteLine("first iteration settler");
                foreach (Player player in gs.Players)
                {
                    player.TookTurn = false;
                    Console.WriteLine("player {0} turn available", player.Index);

                    if (player.hasBuilding(BuildingName.Hacienda, true))
                    {
                        Console.WriteLine("player {0} hacienda enabled.", player.Index);
                        player.getBuilding(BuildingName.Hacienda).EffectAvailable = true;
                    }

                    if (player.hasBuilding(BuildingName.Hospice, true))
                    {
                        Console.WriteLine("player {0} hospice disabled.", player.Index);
                        player.getBuilding(BuildingName.Hospice).EffectAvailable = true;
                    }
                }
            }

            base.mainLoop();
            if (gs.CurrentRole != Name) return;

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


            foreach (var plant in exposed)
            {
                plant.IsExposed = true;
            }
        }

        public bool CanTakeUpSideDown()
        {
            Player player = gs.getCurrPlayer();

            return player.hasBuilding(BuildingName.Hacienda,true)
                && player.getBuilding(BuildingName.Hacienda).EffectAvailable
                && CanTakePlantation();
        }

        public bool CanTakeQuarry()
        {
            Player player = gs.getCurrPlayer();

            return (player.hasBuilding(BuildingName.ConstructionHut,true)
                 || player.CheckForPriviledge())
                 && CanTakePlantation();
        }

        public bool CanTakePlantation()
        {
            return (gs.getCurrPlayer().freePlantationTiles() > 0);
        }

        public bool CheckUsedHacienda()
        {
            Player player = this.gs.getCurrPlayer();
            return player.hasBuilding(BuildingName.Hacienda, true)
                    && !player.CanUseHacienda;
        }

        public bool CheckUsedHospice()
        {
            Player player = this.gs.getCurrPlayer();
            return player.hasBuilding(BuildingName.Hospice, true)
                   && !player.CanUseHospice;
        }

        public void TakePlantation(DataPlantation dataPlantation)
        {
            Plantation newPlantation;
            Player player = gs.getCurrPlayer();

            if (dataPlantation != null && dataPlantation.IsExposed)//exposed
            {
                newPlantation = new Plantation(dataPlantation);
                Plantation removedPlantation = this.gs.Plantations
                    .FirstOrDefault(plantation => plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                this.gs.Plantations.Remove(removedPlantation);
                player.Plantations.Add(newPlantation);

                player.TookTurn = true;

                if (player.hasBuilding(BuildingName.Hospice, true))
                {
                    if (player.hasBuilding(BuildingName.Hacienda, true)
                        && !player.getBuilding(BuildingName.Hacienda).EffectAvailable) this.mainLoop();
                    Console.WriteLine("player {0} hospice enabled.", player.Index);
                    player.getBuilding(BuildingName.Hospice).EffectAvailable = true;
                    return;
                }
                else this.mainLoop();
            }
            else if (dataPlantation != null) //upside down
            {
                newPlantation = new Plantation(dataPlantation);
                Plantation removedPlantation = this.gs.Plantations
                    .FirstOrDefault(plantation => !plantation.IsExposed
                    && plantation.Good == dataPlantation.Good);

                this.gs.Plantations.Remove(removedPlantation);
                player.Plantations.Add(newPlantation);


                player.getBuilding(BuildingName.Hacienda).EffectAvailable = false;

                if (player.hasBuilding(BuildingName.Hospice, true))
                {
                    Console.WriteLine("player {0} hospice enabled.", player.Index);
                    player.getBuilding(BuildingName.Hospice).EffectAvailable = true;
                }
                return;
            }
            else  //quarry
            {
                this.gs.QuarryCount--;
                newPlantation = new Plantation();
                player.Plantations.Add(newPlantation);

                player.TookTurn = true;
                
                if (player.hasBuilding(BuildingName.Hospice, true))
                {
                    if (player.hasBuilding(BuildingName.Hacienda, true)
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
