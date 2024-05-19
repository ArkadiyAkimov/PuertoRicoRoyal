using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataPlayerBuilding
    {
        public int Id { get; set; }

        public int DataPlayerId { get; set; }

        public BuildingName Name { get; set; }

        public List<DataSlot> Slots { get; set; }

        public int Quantity { get; set; }

        public int BuildOrder { get; set; }
    }
}
