using PuertoRicoAPI.Types;
using System.Collections.Generic;
using System.Text.Json;

namespace PuertoRicoAPI.Model.Containers
{
    public class GoodContainer
    {
        public GoodContainer(int capacity,string? goods = null) 
        {
            this.Capacity = capacity;

            if (goods == null) this.Goods = new List<GoodType>();

            else this.DeserializeGoodsFromJson(goods);
        }

        public GoodContainer(int capacity, List<GoodType> goods)
        {
            this.Capacity = capacity;
             this.Goods = goods;
        }

        public int Capacity { get; set; }
        public List<GoodType> Goods { get; set; } = new List<GoodType>();

        public string SerializeGoodsToJson()
        {
            return JsonSerializer.Serialize(Goods);
        }
        public void DeserializeGoodsFromJson(string json)
        {
            this.Goods = JsonSerializer.Deserialize<List<GoodType>>(json);
        }

        public bool IsFull()
        {
            return this.Goods.Count == this.Capacity;
        }

        public bool IsEmpty()
        {
            return this.Goods.Count == 0;
        }

        public bool HasGood(GoodType goodType)
        {
            foreach (GoodType good in this.Goods)
            {
                if(good == goodType) return true;
            }
            return false;
        }

        public void Empty()
        {
            this.Goods.Clear();
        }

    }
}
