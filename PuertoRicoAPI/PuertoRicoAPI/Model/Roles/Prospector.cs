using PuertoRicoAPI.Data.DataClasses;

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

            this.gs.getCurrPlayer().chargePlayer(-1);
            this.endRole();
        }

        public override void endRole()
        {
            base.endRole();
        }
    }
}
