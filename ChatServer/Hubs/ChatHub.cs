using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Log the received message
            LogMessage(user, message);

            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        private void LogMessage(string user, string message)
        {
            // In a real application, you might want to log messages to a database or file.
            // For simplicity, we'll just print the message to the console in this example.
            Console.WriteLine($"{user}: {message}");
        }
    }
}
