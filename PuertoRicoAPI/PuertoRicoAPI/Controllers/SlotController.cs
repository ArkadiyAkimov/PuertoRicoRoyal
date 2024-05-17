using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Types;

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
        private readonly DataContext _context;
        public SlotController(DataContext context)
        {
            _context = context;
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

            if (dataGameState.CurrentRole == Types.RoleName.Mayor)
            {
                if(dataSlot.IsOccupied)
                {
                    dataSlot.IsOccupied = false;
                    dataGameState.Players[dataGameState.CurrentPlayerIndex].Colonists++;
                } 
                else if(dataGameState.Players[dataGameState.CurrentPlayerIndex].Colonists > 0 )
                {
                    dataSlot.IsOccupied = true;
                    dataGameState.Players[dataGameState.CurrentPlayerIndex].Colonists--;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(DataFetcher.sanitizeData(dataGameState, slotInput.PlayerIndex));
        }
    }
}
