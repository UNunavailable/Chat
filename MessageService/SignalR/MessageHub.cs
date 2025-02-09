using Microsoft.AspNetCore.SignalR;

namespace MessageService.SignalR
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string content, DateTime timestamp, int clientNumber)
        {
            await Clients.All.SendAsync("ReceiveMessage", content, timestamp, clientNumber);
        }
    }

}
