using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.deployables
{
    public class Plantation
    {
        public Plantation(DataPlantation dataPlant)
        {
            this.SlotState = dataPlant.Slot.State;

            this.IsExposed = dataPlant.IsExposed;

            this.IsDiscarded = dataPlant.IsDiscarded;
            
            this.Good = dataPlant.Good;

        }

        public Plantation()
        {
            this.SlotState = SlotEnum.Vacant;

            this.IsExposed = true;

            this.IsDiscarded = false;

            this.Good = GoodType.Quarry;

            this.BuildOrder = 0;
        }

        public Plantation(DataPlayerPlantation dataPlant)
        {
            this.SlotState = dataPlant.Slot.State;

            this.IsExposed = true;

            this.IsDiscarded = false;

            this.Good = dataPlant.Good;

            this.BuildOrder = dataPlant.BuildOrder;
        }
        public SlotEnum SlotState { get; set; }
        public bool IsExposed { get; set; }
        public bool IsDiscarded { get; set; }
        public GoodType Good { get; set; }
        public int BuildOrder { get; set; }
    }
}
