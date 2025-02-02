﻿using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
using System.Numerics;

namespace PuertoRicoAPI.Model.Roles
{
    public class Craftsman : Role
    {
        public Craftsman(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {
            if (IsFirstIteration)
            {
                initializeBuildingEffects(BuildingName.Library, true);
            }

            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            int privilidgeUniqueGoods = 0;

            for (int i = 0; i < gs.Players.Count; i++)
            {
                Player player = gs.getCurrPlayer();
                int[] prodQueue = this.getProductionArray(player);

                Console.WriteLine("player {0} produced goods", player.Index);

                if (player.hasActiveBuilding(BuildingName.Jeweler))
                {
                    player.chargePlayer(-player.CountNobles());
                }

                if (player.hasActiveBuilding(BuildingName.Chapel))
                {
                    if (player.getBuilding(BuildingName.Chapel).isNobleOccupied())
                    {
                        player.VictoryPoints++;
                        gs.VictoryPointSupply--;
                    }
                    else
                    {
                        player.Doubloons++;
                    }
                }

                if (player.CheckForPriviledge())
                {
                    privilidgeUniqueGoods = this.AddGoodsToPlayer(prodQueue);
                }
                else
                {
                    this.AddGoodsToPlayer(prodQueue);
                }

                gs.nextPlayer();
            }

            if (privilidgeUniqueGoods > 0)
            {
                int[] productionArrayGoods = getProductionArray(gs.getCurrPlayer());

                privilidgeUniqueGoods =
                    (CheckSupplyGood(GoodType.Corn) ? (productionArrayGoods[0] > 0 ? 1 : 0) : 0) +
                    (CheckSupplyGood(GoodType.Indigo) ? (productionArrayGoods[1] > 0 ? 1 : 0) : 0) +
                    (CheckSupplyGood(GoodType.Sugar) ? (productionArrayGoods[2] > 0 ? 1 : 0) : 0) +
                    (CheckSupplyGood(GoodType.Tobacco) ? (productionArrayGoods[3] > 0 ? 1 : 0) : 0) +
                    (CheckSupplyGood(GoodType.Coffee) ? (productionArrayGoods[4] > 0 ? 1 : 0) : 0);

                if (privilidgeUniqueGoods > 0) return;
            }

            this.endRole();
        }

        public override void endRole()
        {
            base.endRole();
        }

        public int[] getProductionArray(Player player)
        {
            int cornCt = 0;
            int[] indigoCt = { 0, 0 };
            int[] sugarCt = { 0, 0 };
            int[] tobaccoCt = { 0, 0 };
            int[] coffeeCt = { 0, 0 };


            foreach(Plantation plantation in player.Plantations)
            {
                if (plantation.SlotState != SlotEnum.Vacant)
                switch(plantation.Good)
                {
                    case Types.GoodType.Corn: 
                         cornCt++; 
                        break;
                    case Types.GoodType.Indigo:
                         indigoCt[0]++;
                        break;
                    case Types.GoodType.Sugar:
                         sugarCt[0]++;
                        break;
                    case Types.GoodType.Tobacco:
                         tobaccoCt[0]++;
                        break;
                    case Types.GoodType.Coffee:
                         coffeeCt[0]++;
                        break;
                    default:
                        break;
                }
            }

            foreach(Building building in player.Buildings)
            {
                foreach(SlotEnum slot in building.Slots)
                {
                    if((slot != SlotEnum.Vacant) && BuildingTypes.ProdBuildings
                                            .Contains(building.Type.Name))
                    { 
                        switch(building.Type.Good)
                        {
                            case Types.GoodType.Indigo:
                                indigoCt[1]++;
                                break;
                            case Types.GoodType.Sugar:
                                sugarCt[1]++;
                                break;
                            case Types.GoodType.Tobacco:
                                tobaccoCt[1]++;
                                break;
                            case Types.GoodType.Coffee:
                                coffeeCt[1]++;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }


            int totalCorn = cornCt;
            int totalIndigo = Math.Min(indigoCt[0], indigoCt[1]);
            int totalSugar = Math.Min(sugarCt[0], sugarCt[1]);
            int totalTobacco = Math.Min(tobaccoCt[0], tobaccoCt[1]);
            int totalCoffee = Math.Min(coffeeCt[0], coffeeCt[1]);

            if (totalIndigo > 0 && player.hasActiveBuilding(BuildingName.Aqueduct) && player.hasActiveBuilding(BuildingName.LargeIndigoPlant)) totalIndigo++;
            if (totalSugar > 0 && player.hasActiveBuilding(BuildingName.Aqueduct) && player.hasActiveBuilding(BuildingName.LargeSugarMill)) totalSugar++;

            int[] productionArray = { totalCorn, totalIndigo, totalSugar, totalTobacco, totalCoffee };

            return productionArray;
        }

        public int AddGoodsToPlayer(int[] productionArray)
        {
            Player player = gs.getCurrPlayer();

            int uniqueGoodsProduced =
            this.GiveGoodsOfType(GoodType.Corn, productionArray[0]) +
            this.GiveGoodsOfType(GoodType.Indigo, productionArray[1]) +
            this.GiveGoodsOfType(GoodType.Sugar, productionArray[2]) +
            this.GiveGoodsOfType(GoodType.Tobacco, productionArray[3]) +
            this.GiveGoodsOfType(GoodType.Coffee, productionArray[4]);

            if (player.hasActiveBuilding(BuildingName.Factory))
            {
                if (uniqueGoodsProduced == 5) player.chargePlayer(-5);
                else if (uniqueGoodsProduced > 0) player.chargePlayer(uniqueGoodsProduced - 1);
            }

            if (player.hasActiveBuilding(BuildingName.SpecialtyFactory))
            {
                player.chargePlayer(-(productionArray.Skip(1).Max() - 1));
            }

            Console.WriteLine("unique goods produced {0} exported", uniqueGoodsProduced);

            return uniqueGoodsProduced;
        }

        public int GiveGoodsOfType(GoodType goodType, int goodCt)
        {
            bool didProduce = false;
            for (int i = 0; i < goodCt; i++)
            {
                if (this.CheckSupplyGood(goodType))
                {
                    this.GiveSupplyGood(goodType);
                    didProduce = true;
                }
            }
            if (didProduce) return 1;
            return 0;
        }

        public bool CheckSupplyGood(GoodType goodType)
        {
            int desiredGoodSupply = this.gs.GetGoodCount(goodType);
            return desiredGoodSupply > 0;
        }

        public void GiveSupplyGood(GoodType goodType)
        {
            this.gs.GetGoodCount(goodType, 1); //used to remove one ;)
            this.gs.getCurrPlayer().Goods
                .Where(x => x.Type == goodType)
                .ToList()[0].Quantity++;
        }


    }

}
