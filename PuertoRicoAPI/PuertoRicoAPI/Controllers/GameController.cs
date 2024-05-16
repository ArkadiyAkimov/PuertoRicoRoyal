using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Front.FrontClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Sockets;
using PuertoRicoAPI.Types;
using System.Data;

namespace PuertoRicoAPI.Controllers
{
    public class GameStartInput
    {
        public int GameId { get; set; }
        public int NumOfPlayers { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly DataContext _context;
        public GameController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<StartGameOutput>> postNewGame(GameStartInput gameStartInput)
        {
            var gameToJoin = await DataFetcher.getDataGameState(_context, gameStartInput.GameId);

            Console.WriteLine("tried id: {0}",gameStartInput.GameId);

            var output = new StartGameOutput();

            if (gameToJoin == null)
            {
                output = await DataInitializer.Initialize(_context, gameStartInput.NumOfPlayers);
                Console.WriteLine("Initialized Game: {0} NumOfPlayers: {1}",
                    output.gameState.Id,
                    gameStartInput.NumOfPlayers
                    );
            }
            else
            {
                output.gameState = gameToJoin;
                output.BuildingTypes = BuildingTypes.getAll();
                Console.WriteLine("Joined Game: {0} NumOfPlayers: {1}",
                output.gameState.Id,
                gameStartInput.NumOfPlayers
                );
            }

            output.gameState = DataFetcher.sanitizeData(output.gameState, gameStartInput.PlayerIndex);

            return Ok(output);
        }
    }
}
