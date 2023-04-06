using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace register_app.Hubs
{
    [Authorize(Roles = "Security")]
    public class SecurityHub : Hub
    {
        public async Task SendSecurityInfo(string securityInfo)
        {
            await Clients.All.SendAsync("ReceiveSecurityInfo", securityInfo);
        }
    }
}
