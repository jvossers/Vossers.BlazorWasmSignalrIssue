using Microsoft.AspNetCore.SignalR;
using System.Collections.Immutable;
using Vossers.BlazorWasmSignalrIssue.Shared;

namespace Vossers.BlazorWasmSignalrIssue.SignalrServer.Hubs
{
    public class DemoHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var immutableList = ImmutableList<string>.Empty.Add("One").Add("Two").Add("Three");

            await Clients.All.SendAsync("ImmutableViewModelUpdated", new ImmutableViewModel(immutableList));
            await Clients.All.SendAsync("MutableViewModelUpdated", new MutableViewModel(immutableList.ToList()));

            await base.OnConnectedAsync();
        }
    }
}