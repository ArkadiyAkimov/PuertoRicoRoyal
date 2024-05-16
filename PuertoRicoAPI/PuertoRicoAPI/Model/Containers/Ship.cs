using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Containers
{
    public class Ship : GoodContainer
    {
        public Ship(DataShip dataShip)
         : base(capacity: dataShip.Capacity, goods: createDataShipGoods(dataShip))
        {
            Type = dataShip.Type;
        }

        public GoodType Type { get; set; }
        public static List<GoodType> createDataShipGoods(DataShip dataShip)
        {
            var goods = new List<GoodType>();

            for(int i=0; i<dataShip.Load; i++)
            {
                goods.Add(dataShip.Type);
            }

            return goods;
        }

        public void ResetShip()
        {
            this.Goods = new List<GoodType> { };
            this.Type = GoodType.NoType;
        }

        public int TryAddGoods(int amount, GoodType type)
        {
            if(amount == 0) return 0;
            if(this.Type != type && !this.IsEmpty()) return 0;
            this.Type = type;

            int availableCap = this.Capacity - this.Goods.Count;
            int transferedGoods = Math.Min(amount, availableCap);

            for(int i = 0; i<transferedGoods; i++)
            {
                this.Goods.Add(type);
            }

            return transferedGoods;
        }
    }
}
