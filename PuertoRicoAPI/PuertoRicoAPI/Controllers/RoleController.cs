using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Sockets;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Controllers
{
    public class RoleInput
    {
        public int RoleId { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public RoleController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<DataGameState>> PostRole(RoleInput roleInput) {

            DataRole dataRole = await DataFetcher
                .getDataRole(_context,roleInput.RoleId);

            GameState gs = await ModelFetcher
                .getGameState(_context, roleInput.DataGameId);

            if (roleInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getRole(dataRole.Name);

            var dataGameState = await DataFetcher
                .getDataGameState(_context, dataRole.DataGameStateId);

            if (!currentRole.IsPlayable || gs.IsRoleInProgress) return Ok("either role in progress or this role unavailable");

            switch (currentRole.Name)
            {
                case RoleName.Captain:
                case RoleName.Mayor:
                case RoleName.Craftsman:
                    GuestHouse guestHouseRole = (GuestHouse)gs.getRole(RoleName.GuestHouse);
                    gs.GuestHouseNextRole = currentRole.Name;
                    guestHouseRole.mainLoop();
                    break;
                default:
                    currentRole.mainLoop();
                    break;
            }

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }

}
