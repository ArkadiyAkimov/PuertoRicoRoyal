using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataShip
    {
        public int Id { get; set; }
        public int DataGameStateId { get; set; }
        public int Capacity { get; set; }
        public int Load { get; set; }
        public GoodType Type { get; set; }
    }
}
