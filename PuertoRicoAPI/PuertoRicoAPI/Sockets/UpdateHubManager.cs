using System.Collections.Concurrent;

namespace PuertoRicoAPI.Sockets
{
    public class UpdateHubManager
    {
        private readonly ConcurrentDictionary<int, UpdateHub> _gameHubs = new ConcurrentDictionary<int, UpdateHub>();

        public UpdateHub GetHubForGame(int gameId)
        {
            return _gameHubs.GetOrAdd(gameId, id => new UpdateHub());
        }

        public void RemoveHubForGame(int gameId)
        {
            _gameHubs.TryRemove(gameId, out _);
        }
    }

}
