using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Data.DataHandlers;

namespace PuertoRicoAPI.Model.ModelHandlers
{
    public static class ModelFetcher
    {
        public static async Task<GameState> getGameState(DataContext _context, int gameStateId)
        {
            DataGameState dataGameState = await DataFetcher.getDataGameState(_context, gameStateId);
            return new GameState( dataGameState);
        }
    }
}
