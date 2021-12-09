using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TbsReact.Singleton;

namespace TbsReact.Hubs
{
    public class GroupHub : Hub
    {
        public async Task AddGroup(string groupkey)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupkey);
            await Clients.Caller.SendAsync("Message", $"{Context.ConnectionId} is now on account {groupkey}");
            AccountManager.ClientConnect(groupkey);
        }

        public async Task RemoveGroup(string groupkey)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupkey);
            await Clients.Caller.SendAsync("Message", $"{Context.ConnectionId} didn't watch account {groupkey} anymore");
            AccountManager.ClientDisconnect(groupkey);
        }
    }
}