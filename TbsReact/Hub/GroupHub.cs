using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TbsReact.Singleton;

namespace TbsReact.Hubs
{
    public class GroupHub : Hub
    {
        public async Task AddGroup(int index)
        {
            var acc = AccountData.GetAccount(index);
            await Groups.AddToGroupAsync(Context.ConnectionId, acc.Name);
            await Clients.Caller.SendAsync("message", $"{Context.ConnectionId} is now on account {acc.Name}");

            AccountManager.ClientConnect(acc.Name);
        }

        public async Task RemoveGroup(int index)
        {
            var acc = AccountData.GetAccount(index);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, acc.Name);
            await Clients.Caller.SendAsync("message", $"{Context.ConnectionId} didn't watch account {acc.Name} anymore");
            AccountManager.ClientDisconnect(acc.Name);
        }
    }
}