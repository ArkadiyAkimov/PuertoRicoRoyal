using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Controllers;
using PuertoRicoAPI.Data.DataClasses;
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
                Console.WriteLine(this.Name + ": First Iteration");
                this.gs.IsRoleInProgress = true;
                this.gs.CurrentRole = this.Name;
                IsFirstIteration = false;
                IsPlayable = false;
                var currentPlayer = gs.getCurrPlayer();
                currentPlayer.chargePlayer(-this.Bounty);
                this.Bounty = 0;
                Console.WriteLine("Current Player: " + gs.getCurrPlayer().Index);
                return;
            }

            if(this.Name != RoleName.Craftsman) gs.nextPlayer();


            if (gs.getCurrPlayer().CheckForPriviledge() && this.Name != RoleName.Captain)
            {
                endRole();
                return;
            }
            Console.WriteLine("Current Player: " + gs.getCurrPlayer().Index);
        }
        public virtual void endRole()
        {
            Console.WriteLine(this.Name + ": End Role");
            if(this.Name != RoleName.Captain) gs.nextPrivilege();
            gs.IsRoleInProgress = false;
            gs.CurrentRole = RoleName.NoRole;
            this.IsFirstIteration = true;
        }

    }
}