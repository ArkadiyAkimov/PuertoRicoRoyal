using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model;

namespace PuertoRicoAPI.Controllers
{
    public class ChipInput
    {
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ChipController : ControllerBase
    {
        private readonly DataContext _context;
        public ChipController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("colonist")]
        public async Task<ActionResult<DataGameState>> PostColonist(ChipInput chipInput)
        {

            Console.WriteLine("colonist posted");    

            DataGameState dataGameState = await DataFetcher
            .getDataGameState(_context, chipInput.DataGameId);

            //if (slotInput.PlayerIndex != dataGameState.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            if ((dataGameState.CurrentRole == Types.RoleName.Mayor) 
                && (!dataGameState.MayorTookPrivilige) 
                && (chipInput.PlayerIndex == dataGameState.PrivilegeIndex)
                && (dataGameState.ColonistsSupply > 0))
            {
                dataGameState.Players[chipInput.PlayerIndex].Colonists++;
                dataGameState.ColonistsSupply--;
                dataGameState.MayorTookPrivilige = true;
            }

            await _context.SaveChangesAsync();

            return Ok(DataFetcher.sanitizeData(dataGameState, chipInput.PlayerIndex));
        }
    }
}
