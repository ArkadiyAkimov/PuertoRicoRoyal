﻿using Microsoft.AspNetCore.Mvc;
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
        public bool IsNoble { get; set; }
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
                if (dataSlot.State != SlotEnum.Vacant)
                {
                    return BadRequest("Can't use guesthouse on Occupied slot.");
                }
                else
                {

                    Player player = gs.getCurrPlayer();
                    Building guestHouseBuilding = player.getBuilding(BuildingName.GuestHouse);

                    if (player.hasActiveBuilding(BuildingName.GuestHouse)) {

                        int playerId = dataGameState.Players[slotInput.PlayerIndex].Id;
                        int[] buildOrderAndIndex = await DataFetcher.getBuildOrderAndIndexOfDataSlot(_context, slotInput.SlotId, playerId);
                        GuestHouse guestHouseRole = (GuestHouse)gs.getRole(RoleName.GuestHouse);
                        var targetBuildingOrPlantation = guestHouseRole.getTargetFromBuildOrderAndIndex(buildOrderAndIndex);

                        for (int i = 0; i < 2; i++)
                    {
                        if (guestHouseBuilding.Slots[i] != SlotEnum.Vacant)
                        {
                            
                            guestHouseRole.occupyTargetAndactivateBuildingEffect(targetBuildingOrPlantation, buildOrderAndIndex[1], guestHouseBuilding.Slots[i]);
                            guestHouseBuilding.Slots[i] = SlotEnum.Vacant;
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
            
            if(dataSlot.State != SlotEnum.Vacant)
            {
                if(dataSlot.State == SlotEnum.Colonist) dataGameState.Players[slotInput.PlayerIndex].Colonists++;
                else dataGameState.Players[slotInput.PlayerIndex].Nobles++;

                dataSlot.State = SlotEnum.Vacant;
            } 
            else
            {
                if (!slotInput.IsNoble && dataGameState.Players[slotInput.PlayerIndex].Colonists > 0)
                    {
                        dataSlot.State = SlotEnum.Colonist;
                        dataGameState.Players[slotInput.PlayerIndex].Colonists--;
                    }
                else if (slotInput.IsNoble && dataGameState.Players[slotInput.PlayerIndex].Nobles > 0)
                    {
                        dataSlot.State = SlotEnum.Noble;
                        dataGameState.Players[slotInput.PlayerIndex].Nobles--;
                    }
                }
            }

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok(dataGameState);
        }
    }
}
