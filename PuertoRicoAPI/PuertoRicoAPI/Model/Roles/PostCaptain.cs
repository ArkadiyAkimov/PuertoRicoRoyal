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

        public bool canEndTurn(GoodType[] storageGoods)
        {
            Player player = gs.getCurrPlayer();
            int playerGoodTypes = 0;
            int playerAvailableStorageSpace = 1;
            int playerStoredGoodTypes = 0;
            bool canEndTurn = false;

            if (player.hasBuilding(BuildingName.SmallWarehouse, true)) playerAvailableStorageSpace += 1;
            if (player.hasBuilding(BuildingName.LargeWarehouse, true)) playerAvailableStorageSpace += 2;

            foreach (Good good in player.Goods)
            {
                if(good.Quantity > 0) playerGoodTypes++;
            }

            foreach(GoodType goodType in storageGoods)
            {
                if(goodType != GoodType.NoType) playerStoredGoodTypes++;
            }

            int totalGoodTypesMustStore = Math.Min(playerGoodTypes, playerAvailableStorageSpace);

            if(totalGoodTypesMustStore == playerStoredGoodTypes) canEndTurn = true;

            return canEndTurn;
        }

        public void KeepLegalGoods(GoodType[] storageGoods) 
        {
            Player player = gs.getCurrPlayer();
            if (storageGoods[0] != GoodType.NoType)
            {
                var singleGoodToStore = player.Goods.FirstOrDefault(x => x.Type == storageGoods[0]);
                if (singleGoodToStore.Quantity > 1) singleGoodToStore.Quantity = 1;
            }

            foreach (Good good in player.Goods.Where(x => !storageGoods.Contains(x.Type)).ToList())
            {
                gs.GetGoodCount(good.Type, -good.Quantity);
                good.Quantity = 0;
            }

            if (storageGoods[1] != GoodType.NoType)
            {
                var smallWarehouseGood = player.Goods.FirstOrDefault(x => x.Type == storageGoods[1]);
                if (!player.hasBuilding(BuildingName.SmallWarehouse, true))
                {
                    gs.GetGoodCount(smallWarehouseGood.Type, -smallWarehouseGood.Quantity);
                    smallWarehouseGood.Quantity = 0;
                }
            }

            if (storageGoods[2] != GoodType.NoType)
            {
                var largeWarehouseGood1 = player.Goods.FirstOrDefault(x => x.Type == storageGoods[2]);
                if (!player.hasBuilding(BuildingName.LargeWarehouse, true))
                {
                    gs.GetGoodCount(largeWarehouseGood1.Type, -largeWarehouseGood1.Quantity);
                    largeWarehouseGood1.Quantity = 0;
                }
            }

            if (storageGoods[3] != GoodType.NoType)
            {
                var largeWarehouseGood2 = player.Goods.First(x => x.Type == storageGoods[3]);
                if (!player.hasBuilding(BuildingName.LargeWarehouse, true))
                {
                    gs.GetGoodCount(largeWarehouseGood2.Type, -largeWarehouseGood2.Quantity);
                    largeWarehouseGood2.Quantity = 0;
                }
            }
        }
    }
}
