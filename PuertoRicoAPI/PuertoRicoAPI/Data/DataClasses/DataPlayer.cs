using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataPlayer
    {
        public int Id { get; set; }
        public int DataGameStateId { get; set; }
        public int Index { get; set; }
        public int Doubloons { get; set; }
        public int Colonists { get; set; }
        public int VictoryPoints { get; set; }
        public bool TookTurn {  get; set; } 
        public List<DataPlayerBuilding> Buildings { get; set; }
        public List<DataPlayerPlantation> Plantations { get; set; }
        public int BuildOrder { get; set; }
        public List<DataPlayerGood> Goods { get; set; }
        public double Score { get; set; }

    }
}
