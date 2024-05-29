using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model;
using System.Data;
using PuertoRicoAPI.Types;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Sockets;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Models;

namespace PuertoRicoAPI.Controllers
{
    public class EndTurnInput
    {
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    public class EndTurnPostCaptainInput
    {
        public int DataGameId { get; set; }
        public GoodType WindroseStoredGood { get; set; }
        public GoodType[] StorehouseStoredGoods { get; set; } 
        public GoodType SmallWarehouseStoredType { get; set; }
        public int SmallWarehouseStoredQuantity { get; set; }
        public GoodType[] LargeWarehouseStoredTypes { get; set; }
        public int[] LargeWarehouseStoredQuantities { get; set; }
        public int PlayerIndex { get; set; }
    }

    public class EndTurnSmallWharf
    {
        public int DataGameId { get; set; }
        public GoodType[] goodsToShip { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class EndTurnController : ControllerBase
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public EndTurnController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<DataGameState>> PostEndTurn(EndTurnInput endTurnInput)
        {

            DataGameState dataGameState = await DataFetcher
            .getDataGameState(_context, endTurnInput.DataGameId);

            GameState gs = await ModelFetcher
             .getGameState(_context, endTurnInput.DataGameId);

            if (endTurnInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getCurrentRole();
            Player player = gs.getCurrPlayer();
            //if(gs.CurrentRole == RoleName.NoRole) { return Ok("No turn to end here"); }

            switch (gs.CurrentRole)
            {
                case RoleName.Mayor:
                    if((currentRole as Mayor).TryEndMayorTurn()) 
                      (currentRole as Mayor).mainLoop();
                    break;
                case RoleName.Builder:
                    (currentRole as Builder).mainLoop();
                    break;
                case RoleName.Settler:
                    if (player.hasBuilding(BuildingName.Hospice, true)) 
                        player.getBuilding(BuildingName.Hospice).EffectAvailable = false;
                    (currentRole as Settler).mainLoop();
                    break;
                case RoleName.Captain:
                    if(!(currentRole as Captain).checkIfCanShipAnyGoods())
                    {
                        gs.CaptainPlayableIndexes[player.Index] = false;
                        (currentRole as Captain).mainLoop();
                    }
                    break;
                case RoleName.Trader:
                    (currentRole as Trader).mainLoop(); 
                    break;
                case RoleName.GuestHouse:
                    (currentRole as GuestHouse).mainLoop();
                    break;
                default:
                    return Ok("No turn to end here");
            }
            
            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("postCaptain")]
        public async Task<ActionResult<DataGameState>> PostEndTurnPostCaptain(EndTurnPostCaptainInput endTurnPostCaptainInput)
        {

            DataGameState dataGameState = await DataFetcher
            .getDataGameState(_context, endTurnPostCaptainInput.DataGameId);

            GameState gs = await ModelFetcher
             .getGameState(_context, endTurnPostCaptainInput.DataGameId);

            if (endTurnPostCaptainInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getCurrentRole();
            //if(gs.CurrentRole == RoleName.NoRole) { return Ok("No turn to end here"); }

            if (gs.CurrentRole == RoleName.PostCaptain)
            {
                if (!(currentRole as PostCaptain).canEndTurn(endTurnPostCaptainInput)) return Ok("Must use all windrose and warehouse storage");
                (currentRole as PostCaptain).KeepLegalGoods(endTurnPostCaptainInput);
                (currentRole as PostCaptain).mainLoop();
            }

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("smallWharf")]
        public async Task<ActionResult<DataGameState>> PostEndTurnSmallWharf(EndTurnSmallWharf endTurnSmallWharf)
        {

            DataGameState dataGameState = await DataFetcher
            .getDataGameState(_context, endTurnSmallWharf.DataGameId);

            GameState gs = await ModelFetcher
             .getGameState(_context, endTurnSmallWharf.DataGameId);

            if (endTurnSmallWharf.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getCurrentRole();
            //if(gs.CurrentRole == RoleName.NoRole) { return Ok("No turn to end here"); }

            if (gs.CurrentRole == RoleName.Captain)
            {
                if((currentRole as Captain).canUseSmallWharf())
                {
                    (currentRole as Captain).shipGoodsSmallWharf(endTurnSmallWharf.goodsToShip);
                    (currentRole as Captain).mainLoop();
                }
            }

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
