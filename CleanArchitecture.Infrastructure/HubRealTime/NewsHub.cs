using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.HubRealTime
{
    public class NewsHub : Hub
    {
        public async Task JoinGroup(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, requestId);
        }
    }
}
