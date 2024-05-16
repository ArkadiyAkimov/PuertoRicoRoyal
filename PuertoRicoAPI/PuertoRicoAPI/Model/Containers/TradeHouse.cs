using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Containers
{
    public class TradeHouse : GoodContainer
    {
        public TradeHouse(DataTradeHouse dataTradeHouse) 
            : base(capacity: 4, dataTradeHouse.Goods) { }

        public bool CanSellGood(GoodType goodType, Player player)
        {
           if(player.GetGoodCount(goodType) == 0) return false;
           if(this.IsFull()) return false;
           if(player.hasBuilding(BuildingName.Office, true)) return true;
           
           return !this.HasGood(goodType);
        }

        public void SellGood(Good good,Player player)
        {
            this.Goods.Add(good.Type);

            good.Quantity--;

            player.chargePlayer(-good.GetPrice());
            if (player.hasBuilding(BuildingName.SmallMarket, true)) player.chargePlayer(-1);
            if (player.hasBuilding(BuildingName.LargeMarket, true)) player.chargePlayer(-2);
        }
    }
}
