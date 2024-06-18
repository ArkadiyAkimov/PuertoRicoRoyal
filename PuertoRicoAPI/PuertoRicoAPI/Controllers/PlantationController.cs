
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
using System.Xml.Linq;
using PuertoRicoAPI.Model.deployables;

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

    public class RemoveSellPlantationInput
    {   
        public int BuildOrder { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    public class BuyRandomPlantationInput
    {
        public bool isForest { get; set; }
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
                && !(player.hasActiveBuilding(BuildingName.Library)
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
                && !(player.hasActiveBuilding(BuildingName.Library)
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

        [HttpPost("buyRandomPlantation")]
        public async Task<ActionResult<DataGameState>> PostBuyRandomPlantation(BuyRandomPlantationInput plantationInput)
        {

            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, plantationInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, plantationInput.DataGameId);

            if (plantationInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("Not Your Turn.");

            Role currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            if (gs.CurrentRole == Types.RoleName.Trader)
            {
                if (player.hasActiveBuilding(BuildingName.LandOffice)
                    && player.getBuilding(BuildingName.LandOffice).EffectAvailable
                    && (player.getBuilding(BuildingName.LandOffice).Slots[0] == SlotEnum.Colonist))
                {
                    Random rnd = new Random();
                    var randomPlant = dataGameState.Plantations
                                               .Where(x => x.IsExposed == false && !x.IsDiscarded)
                                               .OrderBy(x => rnd.Next())
                                               .Take(1).ToList()[0];

                    (gs.getRole(RoleName.Settler) as Settler).TakePlantation(randomPlant, plantationInput.isForest);
                   
                    player.getBuilding(BuildingName.LandOffice).EffectAvailable = false;
                    player.Doubloons--;
                }
            }
            else return Ok("it's not trader phase");

            await DataFetcher.Update(dataGameState, gs);

            _context.SaveChanges();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("removeSell")]
        public async Task<ActionResult<DataGameState>> PostRemoveSell(RemoveSellPlantationInput plantationInput)
        {
            DataGameState dataGameState = await DataFetcher
               .getDataGameState(_context, plantationInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, plantationInput.DataGameId);

            if (plantationInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("Not Your Turn.");

            Role currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            if(gs.CurrentRole == RoleName.Trader) { 
            if ( player.hasActiveBuilding(BuildingName.LandOffice)
                && !player.getBuilding(BuildingName.LandOffice).EffectAvailable) return Ok("Can't sell another plantation");
            }
            if(gs.CurrentRole == RoleName.Settler)
            {
                if (player.hasActiveBuilding(BuildingName.HuntingLodge)
                && !player.getBuilding(BuildingName.HuntingLodge).EffectAvailable) return Ok("Can't remove another plantation");
            }

            Plantation targetPlantation = player.Plantations.First(x => x.BuildOrder == plantationInput.BuildOrder);
            if (targetPlantation == null) return Ok("Fatal Failure");

            DataPlayer dataPlayer = dataGameState.Players[plantationInput.PlayerIndex];
            DataPlayerPlantation targetDataPlantation = dataPlayer.Plantations.First(x => x.BuildOrder == plantationInput.BuildOrder);

            

            if (targetPlantation.Good != GoodType.Forest)
            {
                switch (targetPlantation.SlotState)
                {
                    case SlotEnum.Colonist:
                        player.Colonists++;
                        break;
                    case SlotEnum.Noble:
                        player.Nobles++;
                        break;
                    default:
                        break;
                }
                targetPlantation.SlotState = SlotEnum.Vacant;
                targetPlantation.IsExposed = false;
                targetPlantation.IsDiscarded = true;
                gs.Plantations.Add(targetPlantation);
            }
           
            player.Plantations.Remove(targetPlantation);
            dataPlayer.Plantations.Remove(targetDataPlantation);

            if (gs.CurrentRole == RoleName.Trader)
            {
                player.Doubloons++;
                player.getBuilding(BuildingName.LandOffice).EffectAvailable = false;
            }
            if (gs.CurrentRole == RoleName.Settler)
            {
                player.getBuilding(BuildingName.HuntingLodge).EffectAvailable = false;
            }

            await DataFetcher.Update(dataGameState, gs);

            _context.SaveChanges();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
