using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Data.DataClasses
{
    public class DataGameState
    {
        public int Id { get; set; }
        public bool IsRoleInProgress { get; set; }
        public int CurrentPlayerIndex { get; set; }
        public int PrivilegeIndex { get; set; }
        public int GovernorIndex { get; set; }
        public int VictoryPointSupply { get; set; }
        public int ColonistsSupply { get; set; }
        public int ColonistsOnShip { get; set; }
        public int QuarryCount { get; set; }
        public int CornSupply { get; set; }
        public int IndigoSupply { get; set; }
        public int SugarSupply { get; set; }
        public int TobaccoSupply { get; set; }
        public int CoffeeSupply { get; set; }
        public RoleName CurrentRole { get; set; }
        public List<DataRole> Roles { get; set; }
        public List<User> Users { get; set; }
        public List<DataPlayer> Players { get; set; }
        public List<DataBuilding> Buildings { get; set; }
        public List<DataPlantation> Plantations { get; set; }
        public List<DataShip> Ships { get; set; }
        public DataTradeHouse TradeHouse { get; set; }
        public string CaptainPlayableIndexes { get; set; }
        public bool CaptainFirstShipment { get; set; }
        public bool MayorTookPrivilige { get; set; }
        public bool LastGovernor { get; set; }
        public bool GameOver { get; set; }
    }
}
