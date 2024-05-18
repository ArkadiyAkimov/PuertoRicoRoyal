using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Types;
using PuertoRicoAPI.Sockets;
using Microsoft.AspNetCore.SignalR;

namespace PuertoRicoAPI.Controllers
{
    public class SlotInput
    {
        public int SlotId { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public SlotController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<DataGameState>> PostSlot(SlotInput slotInput)
        {

            Console.WriteLine("SlotId: " +  slotInput.SlotId);

            DataSlot dataSlot = await DataFetcher
                .getDataSlot(_context, slotInput.SlotId);

            DataGameState dataGameState = await DataFetcher
            .getDataGameState(_context, slotInput.DataGameId);


            //if (slotInput.PlayerIndex != dataGameState.CurrentPlayerIndex) return Ok("wait your turn, bitch");


            if (dataGameState.CurrentRole != Types.RoleName.Mayor) return Ok("Succes");
            
            if(dataSlot.IsOccupied)
            {
                dataSlot.IsOccupied = false;
                dataGameState.Players[slotInput.PlayerIndex].Colonists++;
            } 
            else if(dataGameState.Players[slotInput.PlayerIndex].Colonists > 0 )
            {
                dataSlot.IsOccupied = true;
                dataGameState.Players[slotInput.PlayerIndex].Colonists--;
            }

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
