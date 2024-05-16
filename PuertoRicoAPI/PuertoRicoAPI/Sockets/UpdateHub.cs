namespace PuertoRicoAPI.Sockets
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using PuertoRicoAPI.Data.DataClasses;
    using PuertoRicoAPI.Data.DataHandlers;
    using System.Threading.Tasks;
        public class UpdateHub : Hub
        {
            public bool[] Indexes { get; set; } = new bool[5];
            public static async Task SendUpdate(DataGameState dataGameState, IHubContext<UpdateHub> context)
            {
                Console.WriteLine("Sending Update");
                for(int i = 0; i < dataGameState.Players.Count; i++) 
                {
                  await context.Clients
                        .Group("player"+i)
                        .SendAsync("ReceiveUpdate", DataFetcher.sanitizeData(dataGameState,i));
                }
            }
            
            public async Task SelectIndex(int index)
            {
                Console.WriteLine("Selected Index: " + index);
             
                await Groups.AddToGroupAsync(Context.ConnectionId, "player"+index);
            }

            public override async Task OnConnectedAsync()
            {
                Console.WriteLine("Connected with: {0}", Context.ConnectionId);
                await base.OnConnectedAsync();
            }

            public override async Task OnDisconnectedAsync(Exception exception)
            {
                Console.WriteLine("Disconnected with: {0}", Context.ConnectionId);
                await base.OnDisconnectedAsync(exception);
            }
    }
}
