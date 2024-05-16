using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataRole
    {
        public int Id { get; set; }
        public int DataGameStateId { get; set; }
        public RoleName Name { get; set; }
        public bool IsPlayable { get; set; }
        public int Bounty { get; set; }
        public bool IsFirstIteration { get; set; }
    }
}
