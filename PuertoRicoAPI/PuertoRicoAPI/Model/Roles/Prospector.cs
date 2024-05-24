using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class Prospector : Role
    {
        public Prospector(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {
            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            Player player = this.gs.getCurrPlayer();

            player.chargePlayer(-1);
            if(player.hasBuilding(BuildingName.Library,true)) player.chargePlayer(-1);
            this.endRole();
        }

        public override void endRole()
        {
            base.endRole();
        }
    }
}
