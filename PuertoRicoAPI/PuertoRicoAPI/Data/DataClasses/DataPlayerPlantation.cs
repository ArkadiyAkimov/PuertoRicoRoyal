using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataPlayerPlantation
    {
        public int Id { get; set; }

        public int DataPlayerId { get; set; }
        public DataSlot Slot { get; set; }

        public GoodType Good { get; set; }

        public int BuildOrder { get; set; }
    }
}
