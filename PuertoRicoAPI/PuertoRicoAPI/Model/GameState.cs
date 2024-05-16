using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Types;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Model.Containers;
using System.Text.Json;

namespace PuertoRicoAPI.Model
{
    public class GameState
    {
        public GameState(DataGameState dataGameState)
        {
            this.IsRoleInProgress = dataGameState.IsRoleInProgress;
            this.CurrentPlayerIndex = dataGameState.CurrentPlayerIndex;
            this.PrivilegeIndex = dataGameState.PrivilegeIndex;
            this.GovernorIndex = dataGameState.GovernorIndex;
            this.VictoryPointSupply = dataGameState.VictoryPointSupply;
            this.ColonistsSupply = dataGameState.ColonistsSupply;
            this.ColonistsOnShip = dataGameState.ColonistsOnShip;
            this.QuarryCount = dataGameState.QuarryCount;
            this.CornSupply = dataGameState.CornSupply;
            this.IndigoSupply = dataGameState.IndigoSupply;
            this.SugarSupply = dataGameState.SugarSupply;
            this.TobaccoSupply = dataGameState.TobaccoSupply;
            this.CoffeeSupply = dataGameState.CoffeeSupply;
            this.CurrentRole = dataGameState.CurrentRole;
            this.TradeHouse = new TradeHouse(dataGameState.TradeHouse);
            this.CaptainPlayableIndexes = JsonSerializer
                         .Deserialize<List<bool>>(dataGameState.CaptainPlayableIndexes);
            this.CaptainFirstShipment = dataGameState.CaptainFirstShipment;
            this.LastGovernor = dataGameState.LastGovernor;
            this.GameOver = dataGameState.GameOver;

            this.Roles = new List<Role>();
            dataGameState.Roles.ForEach(dataRole =>
            {
                this.Roles.Add(RoleInit.getRoleClass(dataRole, this));
            });

            this.Players = new List<Player>();
            dataGameState.Players.ForEach(dataPlayer =>
            {
                this.Players.Add(new Player(dataPlayer, this));
            });

            this.Buildings = new List<Building>();
            dataGameState.Buildings.ForEach(dataBuilding =>
            {
                this.Buildings.Add(new Building(dataBuilding, this));
            });

            this.Plantations = new List<Plantation>();
            dataGameState.Plantations.ForEach(dataPlantation =>
            {
                this.Plantations.Add(new Plantation(dataPlantation));
            });

            this.Ships = new List<Ship>();

            dataGameState.Ships.ForEach(dataShip =>
            {
                this.Ships.Add(new Ship(dataShip));
            });
        }

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
        public List<Role> Roles { get; set; }
        public List<Player> Players { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Plantation> Plantations { get; set; }
        public List<Ship> Ships { get; set; }
        public TradeHouse TradeHouse { get; set; }
        public List<bool> CaptainPlayableIndexes { get; set; }
        public bool CaptainFirstShipment { get; set; }
        public bool LastGovernor { get; set; }
        public bool GameOver { get; set; }
        public Player getCurrPlayer()
        {
            return this.Players[this.CurrentPlayerIndex];
        }

        public Role getRole(RoleName name)
        {

            foreach (Role role in this.Roles)
            {
                if(role.Name == name) return role;
            }
            return Roles[0];
        }

        public Role getCurrentRole()
        {
            foreach(Role role in this.Roles)
            {
                if(role.Name == this.CurrentRole)
                {
                    return role;
                }
            }
            return null;
        }

        public Building getBuilding(BuildingName name)
        {
            foreach (Building building in this.Buildings)
            {
                if (building.Type.Name == name) return building;
            }
            return null;
        }

        public void nextPlayer()
        {
            this.CurrentPlayerIndex = Utility.Mod(this.CurrentPlayerIndex + 1 , this.Players.Count); 
        }

        public void nextPrivilege()
        {
            if (this.PrivilegeIndex == Utility.Mod(this.GovernorIndex - 1, this.Players.Count))
            {
                nextGovernor();
            }
            else
            {
                this.PrivilegeIndex = Utility.Mod(this.PrivilegeIndex + 1 , this.Players.Count);
                this.CurrentPlayerIndex = this.PrivilegeIndex;
            }
            Console.WriteLine("Current Privilege: " + this.PrivilegeIndex);
        }

        public void nextGovernor()
        {
            if (LastGovernor)
            {
                GameOver = true;
                foreach(Player player in this.Players)
                {
                    player.CalculateScore();
                }
            }

            this.GovernorIndex = Utility.Mod(this.GovernorIndex + 1 , this.Players.Count);
            this.PrivilegeIndex = this.GovernorIndex;
            this.CurrentPlayerIndex = this.GovernorIndex;

            this.Roles.ForEach(role =>
            {
                if (role.IsPlayable && role.Name != RoleName.PostCaptain) role.Bounty++;

                role.IsPlayable = true;
            });
            Console.WriteLine("Current Governor: " + this.GovernorIndex);
        }

       public int countExposedPlantations()
        {
            int count = 0;

            foreach (var plantation in this.Plantations)
            {
                if(plantation.IsExposed) count++;
            }

            return count;   
        }

        public int GetGoodCount(GoodType goodType,int removeQuantity = 0)
        {
            switch(goodType)
            {
                case GoodType.Corn:
                    this.CornSupply -= removeQuantity;
                    return this.CornSupply;
                case GoodType.Indigo:
                    this.IndigoSupply -= removeQuantity;
                    return this.IndigoSupply;
                case GoodType.Sugar:
                    this.SugarSupply -= removeQuantity;
                    return this.SugarSupply;
                case GoodType.Tobacco:
                    this.TobaccoSupply -= removeQuantity;
                    return this.TobaccoSupply;
                case GoodType.Coffee:
                    this.CoffeeSupply -= removeQuantity;
                    return this.CoffeeSupply;
                default:
                    return 0;
            }
        }
    }
}
