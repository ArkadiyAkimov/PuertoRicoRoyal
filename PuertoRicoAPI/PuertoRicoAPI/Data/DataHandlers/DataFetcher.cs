using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Model.Containers;
using PuertoRicoAPI.Model.deployables;
using System.Text.Json;

namespace PuertoRicoAPI.Data.DataHandlers
{
    public static class DataFetcher
    {
        public static async Task<DataGameState> getDataGameState(DataContext _context, int gameStateId)
        {
            return  await _context.Games
                .Include(game => game.Roles)
                .Include(game => game.Players).ThenInclude(player=> player.Buildings).ThenInclude(building=>building.Slots)
                .Include(game => game.Players).ThenInclude(player => player.Plantations).ThenInclude(plantation=>plantation.Slot)
                .Include(game => game.Players).ThenInclude(player => player.Goods)
                .Include(game => game.Buildings).ThenInclude(building=>building.Slots)
                .Include(game => game.Plantations).ThenInclude(plantation=>plantation.Slot)
                .Include(game => game.TradeHouse)
                .Include(game => game.Ships)
                .AsSplitQuery()
                .SingleOrDefaultAsync(game => game.Id == gameStateId);
        }

        public static async Task<DataRole> getDataRole(DataContext _context, int roleId)
        {
            return await _context.Roles
                .SingleOrDefaultAsync(role => role.Id == roleId);   
        }
        public static async Task<DataPlantation> getDataPlantation(DataContext _context, int plantationId)
        {
            return await _context.Plantations
                 .Include(plantation => plantation.Slot)
                .SingleOrDefaultAsync(plantation => plantation.Id == plantationId);
        }

        public static async Task<DataBuilding> getDataBuilding(DataContext _context, int buildingId)
        {
            return await _context.Buildings
                .Include(building => building.Slots)
                .SingleOrDefaultAsync(building => building.Id == buildingId);
        }

        public static async Task<DataSlot> getDataSlot(DataContext _context, int slotId)
        {
            return await _context.Slots
                .SingleOrDefaultAsync(slot => slot.Id == slotId);
        }

        public static async Task<DataPlayerGood> getDataGood(DataContext _context, int goodId)
        {
            return await _context.Goods
                .SingleOrDefaultAsync(good => good.Id == goodId);
        }

        public static async Task<DataGameState> Update(DataGameState dataGameState, GameState gs)
        {

            dataGameState.IsRoleInProgress = gs.IsRoleInProgress;
            dataGameState.CurrentPlayerIndex = gs.CurrentPlayerIndex;
            dataGameState.PrivilegeIndex = gs.PrivilegeIndex;
            dataGameState.GovernorIndex = gs.GovernorIndex;
            dataGameState.VictoryPointSupply = gs.VictoryPointSupply;
            dataGameState.ColonistsSupply = gs.ColonistsSupply;
            dataGameState.ColonistsOnShip = gs.ColonistsOnShip;
            dataGameState.QuarryCount = gs.QuarryCount;
            dataGameState.CornSupply = gs.CornSupply;
            dataGameState.IndigoSupply = gs.IndigoSupply;
            dataGameState.SugarSupply = gs.SugarSupply;
            dataGameState.TobaccoSupply = gs.TobaccoSupply;
            dataGameState.CoffeeSupply = gs.CoffeeSupply;
            dataGameState.CurrentRole = gs.CurrentRole;
            dataGameState.TradeHouse.Goods = gs.TradeHouse.SerializeGoodsToJson();
            dataGameState.CaptainPlayableIndexes = JsonSerializer.Serialize(gs.CaptainPlayableIndexes);
            dataGameState.CaptainFirstShipment = gs.CaptainFirstShipment;
            dataGameState.MayorTookPrivilige = gs.MayorTookPrivilige;
            dataGameState.LastGovernor = gs.LastGovernor;
            dataGameState.GameOver = gs.GameOver;

            foreach (var (dataRole,role) in dataGameState.Roles.Zip(gs.Roles)) //roles
            {
                dataRole.IsPlayable = role.IsPlayable;
                dataRole.Bounty = role.Bounty;
                dataRole.IsFirstIteration = role.IsFirstIteration;
            }

            foreach (var (dataPlayer, player) in dataGameState.Players.Zip(gs.Players)) //players
            {
                dataPlayer.Doubloons = player.Doubloons;
                dataPlayer.Colonists = player.Colonists;
                dataPlayer.VictoryPoints = player.VictoryPoints;
                dataPlayer.CanUseHacienda = player.CanUseHacienda;
                dataPlayer.CanUseHospice = player.CanUseHospice;
                dataPlayer.HospiceTargetSlotId = player.HospiceTargetPlantation;
                dataPlayer.CanUseWharf = player.CanUseWharf;
                dataPlayer.CanUseSmallWarehouse = player.CanUseSmallWarehouse;
                dataPlayer.CanUseLargeWarehouse = player.CanUseLargeWarehouse;
                dataPlayer.Score = player.Score;

                foreach(var(dataGood,good) in dataPlayer.Goods.Zip(player.Goods)) // player-goods
                {
                    dataGood.Quantity = good.Quantity;
                }

                foreach (var (dataBuilding, building) in dataPlayer.Buildings.Zip(player.Buildings))
                {
                    dataBuilding.Quantity = building.Quantity;
                    foreach(var (dataSlot,slot) in dataBuilding.Slots.Zip(building.Slots))
                    {
                        dataSlot.IsOccupied = slot;
                    }
                }

                for(int i = dataPlayer.Buildings.Count; i < player.Buildings.Count; i++)
                {
                    DataPlayerBuilding dataPlayerBuilding = new DataPlayerBuilding();
                    Building building = player.Buildings[i];
                    dataPlayerBuilding.Name = building.Type.Name;
                    dataPlayerBuilding.Quantity = building.Quantity;
                    dataPlayerBuilding.Slots = new List<DataSlot>();

                    for (int j = 0; j < building.Slots.Length; j++)
                    {
                        dataPlayerBuilding.Slots.Add(new DataSlot());
                    }

                    foreach (var (dataSlot, slot) in dataPlayerBuilding.Slots.Zip(building.Slots))
                    {
                        dataSlot.IsOccupied = slot;
                    }
                    dataPlayer.Buildings.Add(dataPlayerBuilding);
                }

                foreach (var (dataPlantation, plantation) in dataPlayer.Plantations.Zip(player.Plantations))
                {
                    dataPlantation.Slot.IsOccupied = plantation.IsOccupied;
                }

                for (int i = dataPlayer.Plantations.Count; i < player.Plantations.Count; i++)
                {
                    DataPlayerPlantation dataPlayerPlantation = new DataPlayerPlantation();
                    Plantation plantation = player.Plantations[i];
                    dataPlayerPlantation.Good = plantation.Good;
                    dataPlayerPlantation.Slot = new DataSlot();
                    dataPlayerPlantation.Slot.IsOccupied = plantation.IsOccupied;

                    dataPlayer.Plantations.Add(dataPlayerPlantation);
                }

            }

            List<DataBuilding> newDataBuildings = new List<DataBuilding>();        // buildings 
            for (int i = 0 ; i < gs.Buildings.Count; i++)
            {
                DataBuilding dataBuilding = dataGameState.Buildings[i];
                Building building = gs.Buildings[i];

                dataBuilding.Name = building.Type.Name;
                dataBuilding.Quantity = building.Quantity;
                dataBuilding.Slots = new List<DataSlot>();

                for (int j = 0; j < building.Slots.Length; j++)
                {
                    dataBuilding.Slots.Add(new DataSlot());
                }

                foreach (var (dataSlot, slot) in dataBuilding.Slots.Zip(building.Slots))
                {
                    dataSlot.IsOccupied = slot;
                }

               // if(dataBuilding.Quantity > 0)
                newDataBuildings.Add(dataBuilding);
            }

            dataGameState.Buildings = newDataBuildings;

            List<DataPlantation> newDataPlantation = new List<DataPlantation>();

            for(int i = 0; i < gs.Plantations.Count; i++)  // plantations
            {
                DataPlantation dataPlantation = new DataPlantation();
                Plantation plantation = gs.Plantations[i];

                dataPlantation.Good = plantation.Good;
                dataPlantation.Slot = new DataSlot();
                dataPlantation.Slot.IsOccupied = plantation.IsOccupied;
                dataPlantation.IsExposed = plantation.IsExposed;
                dataPlantation.IsDiscarded = plantation.IsDiscarded;

                newDataPlantation.Add(dataPlantation);
            }

            dataGameState.Plantations = newDataPlantation;

            List<DataShip> newDataShips = new List<DataShip>();

            for (int i = 0; i < gs.Ships.Count; i++)  // ships
            {
                DataShip dataShip = dataGameState.Ships[i];
                Ship ship = gs.Ships[i];

                dataShip.Load = ship.Goods.Count;
                dataShip.Type = ship.Type;

                newDataShips.Add(dataShip);
            }

            dataGameState.Ships = newDataShips;

            return dataGameState;
        }

        public static DataGameState sanitizeData(DataGameState dataGameState, int playerIndex)
        {
            var dataGameStateCopy = Utility.CloneJson(dataGameState);

            foreach(DataPlayer player in dataGameStateCopy.Players)
            {
                if(player.Index != playerIndex)
                {
                    player.VictoryPoints = 0;
                }
            }
            
            return dataGameState; //change to dataGameStateCopy to disable debug mode
        }
    }
}
