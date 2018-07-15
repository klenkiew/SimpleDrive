using System;
using System.Threading.Tasks;
using FileService.Dto;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace FileService
{
    public class FileContentChangedNotificationHub : Hub
    {
        public async Task Subscribe(string fileId)
        {
            Console.WriteLine("Subscribe: " + fileId);
            await Groups.AddToGroupAsync(Context.ConnectionId, fileId);
        }

        public async Task Unsubscribe(string fileId)
        {
            Console.WriteLine("Unsubscribe: " + fileId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, fileId);
        }
        
        public async Task Notify(FileContentChangedMessage message)
        {
            Console.WriteLine("Notify: " + JsonConvert.SerializeObject(message));
            await Clients.GroupExcept(message.FileId, Context.ConnectionId).SendAsync("OnContentChange", message);
        }
    }
}