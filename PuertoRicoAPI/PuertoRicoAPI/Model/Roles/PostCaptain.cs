using PuertoRicoAPI.Controllers;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
using System.Numerics;

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

        public bool isPlayerTotalGoodsZero(List<Good> playerGoods)
        {
            int playerTotalGoods = playerGoods.Sum(good => good.Quantity);
            if (playerTotalGoods == 0) return true;
            return false;
        }
        public bool canEndTurn(EndTurnPostCaptainInput endTurnPostCaptainInput)
        {

            Player player = gs.getCurrPlayer();
            if(isPlayerTotalGoodsZero(player.Goods)) return true;

            List<Good> playerGoodsCopy = player.Goods.ToList();


            if (player.hasActiveBuilding(BuildingName.LargeWarehouse))
            {
                for (int i = 0; i < endTurnPostCaptainInput.LargeWarehouseStoredTypes.Count(); i++)
                {
                    if (endTurnPostCaptainInput.LargeWarehouseStoredTypes[i] == (GoodType.NoType)) return false;
                    playerGoodsCopy[(int)endTurnPostCaptainInput.LargeWarehouseStoredTypes[i]].Quantity = 0;
                    if (isPlayerTotalGoodsZero(playerGoodsCopy)) return true;
                }
            }
            if (player.hasActiveBuilding(BuildingName.SmallWarehouse))
            {
                if (endTurnPostCaptainInput.SmallWarehouseStoredType == GoodType.NoType) return false;
                playerGoodsCopy[(int)endTurnPostCaptainInput.SmallWarehouseStoredType].Quantity = 0;
                if (isPlayerTotalGoodsZero(playerGoodsCopy)) return true;
            }
            if (player.hasActiveBuilding(BuildingName.Storehouse))
            {
                for (int i = 0; i < endTurnPostCaptainInput.StorehouseStoredGoods.Count(); i++)
                {
                    if (endTurnPostCaptainInput.StorehouseStoredGoods[i] == GoodType.NoType) return false;
                    playerGoodsCopy[(int)endTurnPostCaptainInput.StorehouseStoredGoods[0]].Quantity--;
                    if (isPlayerTotalGoodsZero(playerGoodsCopy)) return true;
                }
            }
            if (endTurnPostCaptainInput.WindroseStoredGood == GoodType.NoType) return false;

            return true;
        }

        public void KeepLegalGoods(EndTurnPostCaptainInput endTurnPostCaptainInput) 
        {
            Player player = gs.getCurrPlayer();

            foreach (Good good in player.Goods)
            {
                int originalQuantity = good.Quantity;
                good.Quantity = 0;
                if (good.Type == endTurnPostCaptainInput.WindroseStoredGood) good.Quantity++;
                good.Quantity += endTurnPostCaptainInput.StorehouseStoredGoods.Count(x => x == good.Type);

                if (endTurnPostCaptainInput.SmallWarehouseStoredType == good.Type) good.Quantity = endTurnPostCaptainInput.SmallWarehouseStoredQuantity;
                if (endTurnPostCaptainInput.LargeWarehouseStoredTypes[0] == good.Type) good.Quantity = endTurnPostCaptainInput.LargeWarehouseStoredQuantities[0];
                if (endTurnPostCaptainInput.LargeWarehouseStoredTypes[1] == good.Type) good.Quantity = endTurnPostCaptainInput.LargeWarehouseStoredQuantities[1];

                gs.GetGoodCount(good.Type, - (originalQuantity -  good.Quantity));
            }

        }
    }
}
