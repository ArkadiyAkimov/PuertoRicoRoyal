
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

namespace PuertoRicoAPI.Controllers
{
    public class BuildingInput
    {
        public int BuildingId { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
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
                .getDataBuilding(_context,buildingInput.BuildingId);

            GameState gs = await ModelFetcher
                .getGameState(_context, buildingInput.DataGameId);

            if (buildingInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");
           
            if (gs.getCurrPlayer().TookTurn) return Ok("can't build twice dummy");

            Console.WriteLine(gs.Buildings.Count);

            Building building = gs.getBuilding(dataBuilding.Name);

            if (!Builder.tryBuyBuilding(building, gs)) return Ok("can't buy building"); 

            var dataGameState = await DataFetcher
                .getDataGameState(_context, dataBuilding.DataGameStateId);
            
            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

      
    }
}
