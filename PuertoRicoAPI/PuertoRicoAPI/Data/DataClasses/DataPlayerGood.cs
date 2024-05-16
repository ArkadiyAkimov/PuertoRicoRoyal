using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataPlayerGood
    {
        public int Id { get; set; }

        public int DataPlayerId { get; set; }

        public int Quantity { get; set; } 

        public GoodType Type { get; set; }
    }
}
