using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.Containers;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class Trader : Role
    {
        public Trader(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {
            base.mainLoop();
            if (gs.CurrentRole != Name) return;
            if (!this.CanSellAnything()) this.mainLoop();
        }

        public override void endRole()
        {
            TradeHouse tradeHouse = this.gs.TradeHouse;

            if (tradeHouse.IsFull())
            {
                for (int i = 0; i < tradeHouse.Capacity; i++)
                {
                    gs.GetGoodCount(tradeHouse.Goods[i], -1); // returning good to main supply
                }
                tradeHouse.Empty();
            }

            base.endRole();
        }

        public bool CanSellAnything()
        {
            bool canSellAnything = false;
            Player player = gs.getCurrPlayer();

            foreach(Good good in player.Goods)
            {
                if (gs.TradeHouse.CanSellGood(good.Type, player))
                {
                    canSellAnything = true;
                }
            }
            return canSellAnything;
        }

        public bool TrySellGood(Good good)
        {
            Player player = gs.getCurrPlayer();

            if (gs.TradeHouse.CanSellGood(good.Type, player))
            {
                Console.WriteLine("player selling good " + good.Type);
                gs.TradeHouse.SellGood(good, player);
                if (player.CheckForPriviledge()) player.chargePlayer(-1);
                return true;
            }
            return false;
        }
    }
}
