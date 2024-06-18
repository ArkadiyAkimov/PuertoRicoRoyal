using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class Mayor : Role
    {
        public Mayor(DataRole dataRole,GameState gs) : base(dataRole,gs) 
        {

        }

        public override void mainLoop()
        {
            if(IsFirstIteration)
            {
                gs.Players.ForEach(x => x.Colonists += (int)Math.Ceiling(((double)(gs.ColonistsOnShip + gs.NoblesOnShip) / gs.Players.Count) - ((double)Utility.Mod((x.Index - gs.getCurrPlayer().Index), gs.Players.Count) / gs.Players.Count)));
                if (gs.IsNoblesExpansion && (gs.NoblesOnShip > 0))
                {
                    gs.getCurrPlayer().Nobles++;
                    gs.NoblesOnShip--;
                    gs.getCurrPlayer().Colonists--;

                    foreach (var player in gs.Players) 
                    {
                        if (player.hasActiveBuilding(BuildingName.Villa))
                        {
                            if(gs.NoblesSupply > 0)
                            {
                                player.Nobles++;
                                gs.NoblesSupply--;

                            }else if(gs.ColonistsSupply > 0)
                            {
                                player.Colonists++;
                                gs.ColonistsSupply--;
                            }
                        }
                    }
                }
                gs.ColonistsOnShip = 0;
                initializeBuildingEffects(BuildingName.Library, true);
            }

            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            Player currentPlayer = gs.getCurrPlayer();

            if (currentPlayer.CheckForPriviledge()) return;
            uselessTurnSkip(currentPlayer);
        }

        public override void endRole()
        {
            int freeSlots = countFreeSlots();

            gs.ColonistsOnShip = Math.Max(freeSlots, gs.Players.Count);
            gs.ColonistsSupply -= gs.ColonistsOnShip;

            if (gs.IsNoblesExpansion && gs.NoblesSupply > 0) {
                gs.NoblesOnShip++;
                gs.NoblesSupply--;
                gs.ColonistsOnShip--;
                gs.ColonistsSupply++;
            }

            if(gs.ColonistsSupply < 0 && !gs.IsNoblesExpansion) 
            {
                gs.LastGovernor = true;
                gs.ColonistsOnShip = 0;
            }

            gs.MayorTookPrivilige = false;

            base.endRole();
        }

        public void uselessTurnSkip(Player currentPlayer)
        {
            if(currentPlayer.Buildings.Count == 0 && currentPlayer.Plantations.Count == 0)
            {
                Console.WriteLine("player {0} has no slots to fill",currentPlayer);
                mainLoop();
            }
            else if (!isPlayerInputNecessary(currentPlayer))
            {
                Console.WriteLine("No player input necessary player " + currentPlayer.Index);
                fillInEmptySlots(currentPlayer);
                mainLoop();
            }
            
        }

        bool isPlayerInputNecessary(Player player)
        {
            if (player.Nobles > 0) return true;

            foreach(Plantation plant in player.Plantations)
            {
                if(plant.SlotState == SlotEnum.Noble) return true;
            }
            foreach(Building building in player.Buildings)
            {
                foreach(SlotEnum slot in building.Slots)
                {
                    if(slot == SlotEnum.Noble) return true;
                }
            }
            // make complex auto completion for players with nobles in the future.

            int freeSlots = 0;

            player.Buildings.ForEach(building =>
            {
                freeSlots += building.freeSlots();
            });

            player.Plantations.ForEach(plantation =>
            {
                if (plantation.SlotState == SlotEnum.Vacant) freeSlots++;
            });

            if(player.Colonists >= freeSlots) return false;
            return true;
        }

        void fillInEmptySlots(Player player)
        {

            player.Buildings.ForEach(building =>
            {
                for(int i = 0; i<building.Slots.Length; i++)
                {
                    if (building.Slots[i] == SlotEnum.Vacant)
                    {
                        player.Colonists--;
                        building.Slots[i] = SlotEnum.Colonist;
                    }
                }
            });

            player.Plantations.ForEach(plantation =>
            {
                if (plantation.SlotState == SlotEnum.Vacant)
                {
                    player.Colonists--;
                    plantation.SlotState = SlotEnum.Colonist;
                }
            });

        }

        int countFreeSlots()
        {
            int freeSlots = 0;
            gs.Players.ForEach(player =>
            {
                player.Buildings.ForEach(building =>
                {
                    freeSlots += building.freeSlots();
                });
            });

            return freeSlots;
        }

        public bool TryEndMayorTurn()
        {
            var currentPlayer = this.gs.getCurrPlayer();

            int residents = currentPlayer.Colonists + currentPlayer.Nobles;

            if(residents == 0) return true;

            bool cantEndTurn = false;

            currentPlayer.Buildings.ForEach(building =>
            {
                if(building.freeSlots() > 0) cantEndTurn = true;
            });

            currentPlayer.Plantations.ForEach(plantation =>  //fixed
            {
                if (plantation.SlotState == SlotEnum.Vacant) cantEndTurn = true;
            });

            return !cantEndTurn;
        }
    }
}
