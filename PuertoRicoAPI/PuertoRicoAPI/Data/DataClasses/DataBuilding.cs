using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataBuilding
    {
        public int Id { get; set; }

        public int DataGameStateId { get; set; }
     
        public BuildingName Name { get; set; }

        public List<DataSlot> Slots { get; set; }
        public int Quantity { get; set; }
        public bool isDrafted { get; set; }
        public bool isBlocked { get; set; }
    }
}  