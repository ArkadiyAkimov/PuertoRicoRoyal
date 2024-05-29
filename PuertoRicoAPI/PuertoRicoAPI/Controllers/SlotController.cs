using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Types;
using PuertoRicoAPI.Sockets;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Model.Roles;

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

            GameState gs = await ModelFetcher
             .getGameState(_context, slotInput.DataGameId);

            //if (slotInput.PlayerIndex != dataGameState.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            if ((gs.CurrentRole != RoleName.Mayor) 
             && (gs.CurrentPlayerIndex == slotInput.PlayerIndex))
            {
                if (dataSlot.IsOccupied)
                {
                    return Ok("Go home!");
                }
                else
                {

                    Player player = gs.getCurrPlayer();
                    Building guestHouseBuilding = player.getBuilding(BuildingName.GuestHouse);

                    if (player.hasBuilding(BuildingName.GuestHouse, true)) {

                        int playerId = dataGameState.Players[slotInput.PlayerIndex].Id;
                        int[] buildOrderAndIndex = await DataFetcher.getBuildOrderAndIndexOfDataSlot(_context, slotInput.SlotId, playerId);
                        GuestHouse guestHouseRole = (GuestHouse)gs.getRole(RoleName.GuestHouse);
                        var targetBuildingOrPlantation = guestHouseRole.getTargetFromBuildOrderAndIndex(buildOrderAndIndex);

                        for (int i = 0; i < 2; i++)
                    {
                        if (guestHouseBuilding.Slots[i] == true)
                        {
                            guestHouseBuilding.Slots[i] = false;
                            guestHouseRole.occupyTargetAndactivateBuildingEffect(targetBuildingOrPlantation, buildOrderAndIndex[1]);
                              if ((i == 1) && gs.CurrentRole == RoleName.GuestHouse)
                            {
                                    guestHouseRole.mainLoop();
                            }
                            break;
                        }
                    }

                        await DataFetcher.Update(dataGameState, gs);
                    }
                }
            }

            if (dataGameState.CurrentRole == Types.RoleName.Mayor) {
            
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
            }

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }
    }
}
