using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataSlot
    {
        public int Id { get; set; }
        public SlotEnum State { get; set; } = SlotEnum.Vacant;

    }
}
