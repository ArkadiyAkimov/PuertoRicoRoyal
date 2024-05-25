
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
using PuertoRicoAPI.Types;

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

            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, plantationInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, plantationInput.DataGameId);

            if (plantationInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("Not Your Turn.");

            Role currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            if (player.TookTurn
                && !(player.hasBuilding(BuildingName.Library,true)
                && !player.getBuilding(BuildingName.Library).EffectAvailable)) return Ok("Can't take another plantation");

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakePlantation())
                {
                    (currentRole as Settler).TakePlantation(dataPlantation,false);
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

            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, quarryInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, quarryInput.DataGameId);

            if (quarryInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("Not Your Turn.");

            Role currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            if (player.TookTurn) return Ok("Can't take another plantation");
            Console.WriteLine("nigger {0}",player.Index);

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakeQuarry())
                {
                    (currentRole as Settler).TakePlantation(null,false);
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

            DataGameState dataGameState = await DataFetcher
              .getDataGameState(_context, quarryInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, quarryInput.DataGameId);

            if (quarryInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("Not Your Turn.");

            Role currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            if (player.TookTurn) return Ok("Can't take an upside down plantation after taking turn");

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakeUpSideDown())
                {
                    Random rnd = new Random();
                    var randomPlant = dataGameState.Plantations
                                               .Where(x => x.IsExposed == false && !x.IsDiscarded)
                                               .OrderBy(x => rnd.Next())
                                               .Take(1).ToList()[0];

                   

                    (currentRole as Settler).TakePlantation(randomPlant, false);
                }
            }
            else return Ok("it's not settler phase");

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("forest")]
        public async Task<ActionResult<DataGameState>> PostForest(PlantationInput plantationInput)
        {

            DataPlantation dataPlantation = await DataFetcher.getDataPlantation(_context, plantationInput.PlantationId);

            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, plantationInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, plantationInput.DataGameId);

            if (plantationInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("Not Your Turn.");

            Role currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            if (player.TookTurn
                && !(player.hasBuilding(BuildingName.Library, true)
                && !player.getBuilding(BuildingName.Library).EffectAvailable)) return Ok("Can't take another plantation");

            if (gs.CurrentRole == Types.RoleName.Settler)
            {
                if ((currentRole as Settler).CanTakeForest())
                {
                    (currentRole as Settler).TakePlantation(dataPlantation,true);
                }
            }
            else return Ok("it's not settler phase");

            await DataFetcher.Update(dataGameState, gs);

            _context.SaveChanges();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
