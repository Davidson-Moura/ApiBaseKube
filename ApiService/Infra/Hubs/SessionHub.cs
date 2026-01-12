using ApiService.Domain.Services.Caches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ApiService.Domain.Security;
using ApiService.Domain.Hubs;
using Common.Messages;

namespace ApiService.Infra.Hubs
{
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class SessionHub : Hub<ISessionHub>
    {
        private readonly IClaimContext _claimContext;
        private readonly IUserMappingCacheService _userMappingCacheService;
        public SessionHub(
            IClaimContext claimContext,
            IUserMappingCacheService userMappingCacheService)
        {
            _claimContext = claimContext;
            _userMappingCacheService = userMappingCacheService;
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async override Task OnConnectedAsync()
        {
            var machineId = _claimContext.GetMachineIdentifier();
            var existOther = _userMappingCacheService.AddConnectionAsync(
                Context.ConnectionId, 
                _claimContext.UserId, 
                _claimContext.TenantId.ToString(), 
                machineId);

            var removeds = await _userMappingCacheService.RemoveAndGetOtherConnectionId(_claimContext.UserId, machineId);

            ForceDisconnectConnectionId(removeds, SMessage.Message(Messages.YourUserWasAccessedFromAnotherLocation));

            await Clients.Others.UserSigned(_claimContext.UserId, _claimContext.LoginName);
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var infos = await _userMappingCacheService.GetConnectionInfoAsync(Context.ConnectionId);
            var countConns = await _userMappingCacheService.RemoveConnectionAsync(Context.ConnectionId);

            if (countConns <= 0) await Clients.All.UserUnsigned(infos?.UserId, "");

            await base.OnDisconnectedAsync(exception);
        }
        private async Task ForceDisconnectConnectionId(IEnumerable<string> connectionIds, string msg)
        {
            var clients = Clients.Clients(connectionIds);
            await clients.ForcedDisconnect(msg);
        }
    }
}
