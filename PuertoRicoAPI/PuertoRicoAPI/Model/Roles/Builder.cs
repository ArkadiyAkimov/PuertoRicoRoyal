using PuertoRicoAPI.Controllers;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
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
            Player player = gs.getCurrPlayer();

            int guestHouseQuarryBonus = 0;

            if ((player.hasActiveBuilding(BuildingName.GuestHouse) && hasUnoccupiedPlantationsOfType(GoodType.Quarry)))      // occupied guesthouse and vacant quarries (possible discount)
            {
                int unoccupiedQuarries = 0;
                int guests = player.getBuilding(BuildingName.GuestHouse).Slots.Count(slot => slot != SlotEnum.Vacant);

                foreach(Plantation plantation in player.Plantations)
                {
                    if(plantation.Good == GoodType.Quarry && (plantation.SlotState == SlotEnum.Vacant)) unoccupiedQuarries++;
                }

                guestHouseQuarryBonus += Math.Min(guests, unoccupiedQuarries);   //added max number of quarries that can be occupied
            } 

            foreach (Building building in gs.Buildings)
            {
                if (canBuyBuilding(building, calculateMaxBlackMarketDiscount(), guestHouseQuarryBonus) && !inPossesionOfBuilding(building)) return; // can buy building
            }

            

            Console.WriteLine("player {0} can't build anything skipping turn", player.Index);
            this.mainLoop();
        }

        public bool inPossesionOfBuilding(Building building)
        {
            Player player = this.gs.getCurrPlayer();

            if (player.hasVacancyAtBuilding(building.Type.Name) || player.hasActiveBuilding(building.Type.Name)) return true;
           
            return false;
        }

        public bool hasUnoccupiedPlantationsOfType(GoodType goodType)
        {
            Player player = gs.getCurrPlayer();

            foreach (Plantation plantation in player.Plantations)
            {
                if (plantation.Good == goodType) return (plantation.SlotState == SlotEnum.Vacant);
            }

            return false;
        }


        public bool tryBuyBuilding(Building building,int blackMarketDiscount=0)
        {
            int buildingPrice = building.getBuildingPrice();

            if (!canBuyBuilding(building,blackMarketDiscount)) return false;
            building.Quantity--;
            gs.getCurrPlayer().chargePlayer(buildingPrice - blackMarketDiscount);


            if (building is ProdBuilding) gs.getCurrPlayer().Buildings.Add(new ProdBuilding(building as ProdBuilding));
            else gs.getCurrPlayer().Buildings.Add(new Building(building));

            if (gs.getCurrPlayer().freeBuildingTiles() == 0) gs.LastGovernor = true;

            if (gs.getCurrPlayer().hasActiveBuilding(BuildingName.Univercity))
            {
                gs.getCurrPlayer().TookTurn = true;
                return true;
            }

            if (gs.getCurrPlayer().hasActiveBuilding(BuildingName.Church))
            {
                if (building.Type.VictoryScore == 2 || building.Type.VictoryScore == 3) gs.getCurrPlayer().VictoryPoints++;
                if (building.Type.VictoryScore > 3) gs.getCurrPlayer().VictoryPoints += 2;
            }

            if(blackMarketDiscount == 0) gs.getCurrentRole().mainLoop();

            return true;
        }

        public bool canBuyBuilding(Building building,int blackMarketDiscount = 0, int guestHouseQuarryBonus = 0)
        {
            Player player = gs.getCurrPlayer();

            if (gs.CurrentRole != RoleName.Builder) return false;

            foreach(Building ownedBuilding in player.Buildings)
            {
                if (ownedBuilding.Type.Name == building.Type.Name) return false;
            }

            if (player.freeBuildingTiles() < building.Type.size) return false;

            int buildingPrice = building.getBuildingPrice(guestHouseQuarryBonus);  // defaults to 0 but on one occation is used

            buildingPrice -= blackMarketDiscount;

            if (blackMarketDiscount == 0)
            {
                if (buildingPrice > player.Doubloons) return false; //without black market you need more money than the discount
            }
            else {
            if (buildingPrice != player.Doubloons) return false; //with black market using the black market discount you must have the exact amount of money or else can't use that much discount
            }

            return true;
        }

        public int calculateMaxBlackMarketDiscount()
        {
            Player player = gs.getCurrPlayer();

            if (player.hasActiveBuilding(BuildingName.BlackMarket))
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

        public int calculateTotalBlackMarketDiscount(BuildingBlackMarketInput blackMarketInput, int[] buildOrderAndIndex)
        {
            Player player = gs.getCurrPlayer();

            int discount = 0;

            if (blackMarketInput.SellColonist)
            {
                if(blackMarketInput.SlotId == 0)
                {
                    if (player.Colonists > 0) discount++;
                }
                else if(buildOrderAndIndex != null)
                {
                    foreach (Building building in player.Buildings)
                    {
                        if(building.BuildOrder == buildOrderAndIndex[0])
                        {
                            if (building.Slots[buildOrderAndIndex[1]] != SlotEnum.Vacant) discount++;
                        }
                    }

                    foreach (Plantation plantation in player.Plantations)
                    {
                        if (plantation.BuildOrder == buildOrderAndIndex[0])
                        {
                            if (plantation.SlotState != SlotEnum.Vacant) discount++;
                        }
                    }
                }
            }
            if (blackMarketInput.SellVictoryPoint)
            {
                if (player.VictoryPoints > 0) discount++;
            }
            if (blackMarketInput.SellGood)
            {
                if (player.Goods[blackMarketInput.GoodType].Quantity > 0) discount++;
            }
            
            return discount;
        }

        public void ChargeForBlackMarketUse(BuildingBlackMarketInput blackMarketInput, int[] buildOrderAndIndex)
        {
            Player player = gs.getCurrPlayer();


            if (blackMarketInput.SellColonist)
            {
                if (blackMarketInput.SlotId == 0)
                {
                    if (player.Colonists > 0)
                    {
                        player.Colonists--;
                        gs.ColonistsSupply++;
                    }
                }
                else if (buildOrderAndIndex != null)
                {
                    foreach (Building building in player.Buildings)
                    {
                        if (building.BuildOrder == buildOrderAndIndex[0])
                        {
                            if (building.Slots[buildOrderAndIndex[1]] != SlotEnum.Vacant)
                            {
                                if(building.Slots[buildOrderAndIndex[1]] == SlotEnum.Colonist) gs.ColonistsSupply++;
                                else gs.NoblesSupply++;
                                building.Slots[buildOrderAndIndex[1]] = SlotEnum.Vacant;
                               
                            }
                        }
                    }

                    foreach (Plantation plantation in player.Plantations)
                    {
                        if (plantation.BuildOrder == buildOrderAndIndex[0])
                        {
                            if (plantation.SlotState != SlotEnum.Vacant)
                            {
                                if (plantation.SlotState == SlotEnum.Colonist) gs.ColonistsSupply++;
                                else gs.NoblesSupply++;
                                plantation.SlotState = SlotEnum.Vacant;
                            }
                        }
                    }
                }
            }
            if (blackMarketInput.SellVictoryPoint)
            {
                if (player.VictoryPoints > 0)
                {
                    player.VictoryPoints--;
                    gs.VictoryPointSupply++;
                }
            }
            if (blackMarketInput.SellGood)
            {
                if (player.Goods[blackMarketInput.GoodType].Quantity > 0)
                {
                    player.Goods[blackMarketInput.GoodType].Quantity--;
                    gs.GetGoodCount(player.Goods[blackMarketInput.GoodType].Type, -1);
                }
            }

        }

        public int colonistForBlackMarket()
        {
            Player player = gs.getCurrPlayer();

            foreach (Plantation plantation in player.Plantations)
            {
                if ((plantation.SlotState != SlotEnum.Vacant) && plantation.Good != GoodType.Forest) return 1;
            }

            foreach (Building building in player.Buildings)
            {
                    foreach (SlotEnum slot in building.Slots) if (slot != SlotEnum.Vacant) return 1;
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
