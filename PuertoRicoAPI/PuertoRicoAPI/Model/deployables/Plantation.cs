using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.deployables
{
    public class Plantation
    {
        public Plantation(DataPlantation dataPlant)
        {
            this.IsOccupied = dataPlant.Slot.IsOccupied;

            this.IsExposed = dataPlant.IsExposed;

            this.IsDiscarded = dataPlant.IsDiscarded;
            
            this.Good = dataPlant.Good;
        }

        public Plantation()
        {
            this.IsOccupied = false;

            this.IsExposed = true;

            this.IsDiscarded = false;

            this.Good = GoodType.Quarry;
        }

        public Plantation(DataPlayerPlantation dataPlant)
        {
            this.IsOccupied = dataPlant.Slot.IsOccupied;

            this.IsExposed = true;

            this.IsDiscarded = false;

            this.Good = dataPlant.Good;
        }
        public bool IsOccupied { get; set; }
        public bool IsExposed { get; set; }

        public bool IsDiscarded { get; set; }
        public GoodType Good { get; set; }
    }
}
