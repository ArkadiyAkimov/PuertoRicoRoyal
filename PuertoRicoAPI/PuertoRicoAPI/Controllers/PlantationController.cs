
using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Sockets;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Models;

namespace PuertoRicoAPI.Controllers
{
    public class PlantationInput
    {
        public int PlantationId { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    public class QuarryInput
    {
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PlantationController : ControllerBase
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public PlantationController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<DataGameState>> PostPlantation(PlantationInput plantationInput)
        {

            DataPlantation dataPlantation = await DataFetcher.getDataPlantation(_context, plantationInput.PlantationId);

            GameState gs = await ModelFetcher
               .getGameState(_context, plantationInput.DataGameId);

            if (plantationInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var dataGameState = await DataFetcher
               .getDataGameState(_context, plantationInput.DataGameId);

            var currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();
            if (player.CanUseHospice) return Ok("already took plantation");

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakePlantation())
                {
                    (currentRole as Settler).TakePlantation(dataPlantation);
                }
            }
            else return Ok("it's not settler phase");

            await DataFetcher.Update(dataGameState, gs);

            _context.SaveChanges();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("quarry")]
        public async Task<ActionResult<DataGameState>> PostQuarry(QuarryInput quarryInput)
        {

            GameState gs = await ModelFetcher
               .getGameState(_context, quarryInput.DataGameId);

            if (quarryInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var dataGameState = await DataFetcher
               .getDataGameState(_context, quarryInput.DataGameId);

            var currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();
            if (player.CanUseHospice) return Ok("already took plantation");

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakeQuarry())
                {
                    (currentRole as Settler).TakePlantation(null);
                }
            }
            else return Ok("it's not settler phase");

            await DataFetcher.Update(dataGameState, gs);

            _context.SaveChanges();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("upSideDown")]
        public async Task<ActionResult<DataGameState>> PostUpSideDown(QuarryInput quarryInput)
        {

            GameState gs = await ModelFetcher
               .getGameState(_context, quarryInput.DataGameId);

            if (quarryInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var dataGameState = await DataFetcher
               .getDataGameState(_context, quarryInput.DataGameId);

            var currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();
            if (player.CanUseHospice) return Ok("already took plantation");

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakeUpSideDown())
                {
                    Random rnd = new Random();
                    var randomPlant = dataGameState.Plantations
                                               .Where(x => x.IsExposed == false && !x.IsDiscarded)
                                               .OrderBy(x => rnd.Next())
                                               .Take(1).ToList()[0];

                    gs.getCurrPlayer().CanUseHacienda = false;

                    (currentRole as Settler).TakePlantation(randomPlant);
                }
            }
            else return Ok("it's not settler phase");

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
