using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataPlantation
    {
        public int Id { get; set; }

        public int DataGameStateId { get; set; }

        public DataSlot Slot { get; set; }

        public bool IsExposed { get; set; }

        public bool IsDiscarded { get; set; }

        public GoodType Good { get; set; }
    }
}
