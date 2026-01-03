using ApiService.Domain.Security;
using System.Security.Claims;

namespace ApiService.Infra.Security
{
    public class ClaimContext : IClaimContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsIdentity _identity;
        public ClaimContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        public string RemoteIpAddress => _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        public string CurrentHttpConnectionId
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Connection.Id;
            }
        }

        public string UserId
        {
            get
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
                    return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Hash)?.Value;
                return "System";
            }
        }
        public string UserName
        {
            get
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
                    return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                return "None";
            }
        }
        public string LoginName
        {
            get
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                    return userName;
                }

                return "System";
            }
        }
        public string Email => _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        public string GetMachineIdentifier()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "unknown";

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "no-ip";
            var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "no-ua";

            var raw = $"{ip}-{userAgent}";
            var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw)));

            return hash;
        }
    }
}
