using PuertoRicoAPI.Controllers;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class PostCaptain : Role
    {
        public PostCaptain(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {

        }

        public override void mainLoop()
        {
            if (IsFirstIteration)
            {
                gs.CurrentPlayerIndex = gs.PrivilegeIndex;
            }

            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            var player = gs.getCurrPlayer();

            if (player.Goods.Sum(x => x.Quantity) == 0) this.mainLoop();
        }

        public override void endRole()
        {
            base.endRole();
        }

        public bool canEndTurn(EndTurnPostCaptainInput endTurnPostCaptainInput)
        {

            Player player = gs.getCurrPlayer();
            int playerTotalGoods = player.Goods.Sum(good => good.Quantity);

            if (playerTotalGoods == 0) return true;

            if (player.hasBuilding(BuildingName.LargeWarehouse, true))
            {
                if (endTurnPostCaptainInput.LargeWarehouseStoredTypes.Contains(GoodType.NoType)) return false;
            }
            if (player.hasBuilding(BuildingName.SmallWarehouse, true))
            {
                if (endTurnPostCaptainInput.SmallWarehouseStoredType == GoodType.NoType) return false;
            }
            if (player.hasBuilding(BuildingName.Storehouse, true))
            {
                if (endTurnPostCaptainInput.StorehouseStoredGoods.Contains(GoodType.NoType)) return false;
            }
            if (endTurnPostCaptainInput.WindroseStoredGood == GoodType.NoType) return false;

            return true;
        }

        public void KeepLegalGoods(EndTurnPostCaptainInput endTurnPostCaptainInput) 
        {
            Player player = gs.getCurrPlayer();

            foreach (Good good in player.Goods)
            {
                good.Quantity = 0;
                if (good.Type == endTurnPostCaptainInput.WindroseStoredGood) good.Quantity++;
                good.Quantity += endTurnPostCaptainInput.StorehouseStoredGoods.Count(x => x == good.Type);
                if (endTurnPostCaptainInput.SmallWarehouseStoredType == good.Type) good.Quantity = endTurnPostCaptainInput.SmallWarehouseStoredQuantity;
                if (endTurnPostCaptainInput.LargeWarehouseStoredTypes[0] == good.Type) good.Quantity = endTurnPostCaptainInput.LargeWarehouseStoredQuantities[0];
                if (endTurnPostCaptainInput.LargeWarehouseStoredTypes[1] == good.Type) good.Quantity = endTurnPostCaptainInput.LargeWarehouseStoredQuantities[1];
            }

        }
    }
}
