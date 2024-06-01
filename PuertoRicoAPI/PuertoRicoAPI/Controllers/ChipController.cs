using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Sockets;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Types;
using PuertoRicoAPI.Model.deployables;

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
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public ChipController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("colonist")]
        public async Task<ActionResult<DataGameState>> PostColonist(ChipInput chipInput)
        {

            Console.WriteLine("colonist posted");    

            DataGameState dataGameState = await DataFetcher
            .getDataGameState(_context, chipInput.DataGameId);

            GameState gs = await ModelFetcher
               .getGameState(_context, dataGameState.Id);

            Player player = gs.Players[chipInput.PlayerIndex];

            var currentRole = gs.getCurrentRole();

            switch (gs.CurrentRole)
            {
                case RoleName.Mayor:
                    if ((!gs.MayorTookPrivilige)
                        && player.CheckForPriviledge()
                        && (gs.ColonistsSupply > 0))
                    {
                        player.Colonists++;
                        gs.ColonistsSupply--;
                        gs.MayorTookPrivilige = true;

                        if (!player.hasActiveBuilding(BuildingName.Library))
                        {
                            (currentRole as Mayor).uselessTurnSkip(player);
                        }
                    }
                    else if (
                        gs.MayorTookPrivilige
                        && player.CheckForPriviledge()
                        && (gs.ColonistsSupply > 0)
                        && player.hasActiveBuilding(BuildingName.Library)
                        && player.getBuilding(BuildingName.Library).EffectAvailable
                        )
                    {
                        player.Colonists++;
                        gs.ColonistsSupply--;
                        player.getBuilding(BuildingName.Library).EffectAvailable = false;

                        (currentRole as Mayor).uselessTurnSkip(player);
                    }
                break;

                case RoleName.Settler:
                    if (player.hasActiveBuilding(BuildingName.Hospice)
                        && player.getBuilding(BuildingName.Hospice).EffectAvailable)
                        {
                            player.getBuilding(BuildingName.Hospice).EffectAvailable = false;
                            if (gs.ColonistsSupply > 0) gs.ColonistsSupply--;
                            else gs.ColonistsOnShip--;

                            Plantation targetplantation = player.Plantations.FirstOrDefault(plantation => plantation.BuildOrder == player.BuildOrder - 1);
                            if (targetplantation != null) targetplantation.IsOccupied = true;

                            if (player.TookTurn)
                            {
                                (currentRole as Settler).mainLoop();
                            }
                        }
                break;

                case RoleName.Builder:
                    if (player.hasActiveBuilding(BuildingName.Univercity)
                       && player.TookTurn)
                    {
                        if (gs.ColonistsSupply > 0) gs.ColonistsSupply--;
                        else gs.ColonistsOnShip--;


                        Building targetBuilding = player.Buildings.FirstOrDefault(building => building.BuildOrder == player.BuildOrder - 1);

                        Console.WriteLine("target building: {0}", targetBuilding);

                        if (targetBuilding != null) targetBuilding.Slots[0] = true;

                        (currentRole as Builder).mainLoop();
                    }
                    break;
            }

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("success");
        }
         
    }
}
