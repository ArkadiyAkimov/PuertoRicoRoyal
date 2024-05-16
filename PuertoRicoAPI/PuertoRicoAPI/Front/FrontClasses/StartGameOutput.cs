using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Front.FrontClasses
{
    public class StartGameOutput
    {
        public DataGameState gameState { get; set; }

        public List<BuildingType> BuildingTypes { get; set; }
    }
}
