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
        public GoodType[] StorageGoods { get; set; }
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
                    Player player = gs.getCurrPlayer();
                    if (player.hasBuilding(BuildingName.Hospice, true)) 
                        player.getBuilding(BuildingName.Hospice).EffectAvailable = false;
                    (currentRole as Settler).mainLoop();
                    break;
                case RoleName.Trader:
                    (currentRole as Trader).mainLoop(); 
                    break;
                case RoleName.PostCaptain:
                    (currentRole as PostCaptain).KeepLegalGoods(endTurnInput.StorageGoods);
                    (currentRole as PostCaptain).mainLoop();
                    break;
                default:
                    return Ok("No turn to end here");
            }
            
            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
