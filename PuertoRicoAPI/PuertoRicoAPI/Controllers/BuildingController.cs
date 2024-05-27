
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Sockets;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Controllers
{
    public class BuildingInput
    {
        public int BuildingId { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    public class BuildingBlackMarketInput
    {
        public int BuildingId { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
        public bool SellColonist { get; set; }
        public int SlotId { get; set; }
        public bool SellGood { get; set; }
        public int GoodType { get; set; }
        public bool SellVictoryPoint { get; set; }

    }


    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public BuildingController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<DataGameState>> PostBuilding(BuildingInput buildingInput)
        {



            DataBuilding dataBuilding = await DataFetcher
                .getDataBuilding(_context, buildingInput.BuildingId);

            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, dataBuilding.DataGameStateId);

            GameState gs = await ModelFetcher
                .getGameState(_context, dataGameState.Id);

            if (buildingInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getCurrentRole();

            Building building = gs.getBuilding(dataBuilding.Name);

            if (gs.CurrentRole == RoleName.Draft && !building.isDrafted && !building.isBlocked)
            {
                (currentRole as Draft).draftBuilding(building);
            }
            else
            {
                if (gs.getCurrPlayer().TookTurn) return Ok("can't build twice dummy");
                Console.WriteLine(gs.Buildings.Count);
                if (!(currentRole as Builder).tryBuyBuilding(building)) return Ok("can't buy building");
            }

            
            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("blackMarket")]
        public async Task<ActionResult<DataGameState>> PostBlackMarketBuilding(BuildingBlackMarketInput buildingBlackMarketInput)
        {



            DataBuilding dataBuilding = await DataFetcher
                .getDataBuilding(_context, buildingBlackMarketInput.BuildingId);

            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, dataBuilding.DataGameStateId);

            GameState gs = await ModelFetcher
                .getGameState(_context, dataGameState.Id);

            if (buildingBlackMarketInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getCurrentRole();

            Building building = gs.getBuilding(dataBuilding.Name);

            
            {
                if (gs.getCurrPlayer().TookTurn) return Ok("can't build twice dummy");
                Console.WriteLine(gs.Buildings.Count);
                if (!(currentRole as Builder).tryBuyBuilding(building)) return Ok("can't buy building");
            }


            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }


    }
}
