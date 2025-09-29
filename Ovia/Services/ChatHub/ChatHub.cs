using Microsoft.AspNetCore.SignalR;

namespace Ovia.Services.ChatHub
{
    public class ChatHub : Hub
    {
        public async void refresh()
        {
            await Clients.All.SendAsync("refresh");
        }
        public async void count()
        {
            await Clients.All.SendAsync("count");
        }
        public async void refreshChat()
        {
            await Clients.All.SendAsync("refreshChat");
        }
        public async void countChat()
        {
            await Clients.All.SendAsync("countChat");
        }
        public async void refreshRooms()
        {
            await Clients.All.SendAsync("refreshRooms");
        }

        public async Task SendMessage(string fromUser , string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", fromUser, message);
        }



        //public async Task SendMessage(string roomName, string user, string message)
        //{
        //    await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        //}





    }
}
