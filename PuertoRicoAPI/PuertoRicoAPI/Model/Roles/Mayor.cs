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
                gs.Players.ForEach(x => x.Colonists += (int)Math.Ceiling(((double)gs.ColonistsOnShip / gs.Players.Count) - ((double)Utility.Mod((x.Index - gs.getCurrPlayer().Index), gs.Players.Count) / gs.Players.Count)));
                gs.ColonistsOnShip = 0;
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

            if(gs.ColonistsSupply < 0) 
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
            int freeSlots = 0;

            player.Buildings.ForEach(building =>
            {
                freeSlots += building.freeSlots();
            });

            player.Plantations.ForEach(plantation =>
            {
                if (!plantation.IsOccupied) freeSlots++;
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
                    if (!building.Slots[i])
                    {
                        player.Colonists--;
                        building.Slots[i] = true;
                    }
                }
            });

            player.Plantations.ForEach(plantation =>
            {
                if (!plantation.IsOccupied)
                {
                    player.Colonists--;
                    plantation.IsOccupied = true;
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

            if(currentPlayer.Colonists == 0) return true;
            bool cantEndTurn = false;

            currentPlayer.Buildings.ForEach(building =>
            {
                if(building.freeSlots() > 0) { cantEndTurn = true; }
            });

            return !cantEndTurn;
        }
    }
}
