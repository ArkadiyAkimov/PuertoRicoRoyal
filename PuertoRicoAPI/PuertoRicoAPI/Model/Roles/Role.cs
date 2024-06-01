using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Controllers;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class Role
    {
        public Role(DataRole dataRole, GameState gs)
        {
            this.gs = gs;
            this.Name = dataRole.Name;
            this.IsPlayable = dataRole.IsPlayable;
            this.Bounty = dataRole.Bounty;
            this.IsFirstIteration = dataRole.IsFirstIteration;
        }

        public GameState gs { get; set; }
        public RoleName Name { get; set; }

        public bool IsPlayable { get; set; }

        public int Bounty { get; set; }

        public bool IsFirstIteration { get; set; }

        public virtual void mainLoop()
        {
            
            if (IsFirstIteration)
            {
                gs.getCurrPlayer().TookTurn = false;
                Console.WriteLine(this.Name + ": First Iteration");
                this.gs.IsRoleInProgress = true;
                this.gs.CurrentRole = this.Name;
                IsFirstIteration = false;
                IsPlayable = false;
                gs.getCurrPlayer().chargePlayer(-this.Bounty);
                this.Bounty = 0;
                Console.WriteLine("Current Player: " + gs.getCurrPlayer().Index);
                return;
            }


            if (this.Name != RoleName.Craftsman) gs.nextPlayer();

            if(!(gs.getCurrPlayer().hasActiveBuilding(BuildingName.Library)
                && gs.getCurrPlayer().CheckForPriviledge()))
            {
                gs.getCurrPlayer().TookTurn = false;
            }


            if (gs.getCurrPlayer().CheckForPriviledge()
                && this.Name != RoleName.Captain
                && this.Name != RoleName.Draft )
            {
                endRole();
                return;
            }
            Console.WriteLine("Current Player: " + gs.getCurrPlayer().Index);
        }
        public virtual void endRole()
        {
            Console.WriteLine(this.Name + ": End Role");
            if((this.Name != RoleName.Captain) 
                && (this.Name != RoleName.Draft)
                && (this.Name != RoleName.GuestHouse)) gs.nextPrivilege();
            gs.IsRoleInProgress = false;
            gs.CurrentRole = RoleName.NoRole;
            this.IsFirstIteration = true;
        }

        public void initializeBuildingEffects(BuildingName buildingName, bool isTrue)
        {
            foreach (Player player in gs.Players)
            {
                if (player.hasActiveBuilding(buildingName))
                {
                    Console.WriteLine("player {0} {1} enabled.", player.Index, buildingName);
                    player.getBuilding(buildingName).EffectAvailable = isTrue;
                }
            }
        }

    }
}