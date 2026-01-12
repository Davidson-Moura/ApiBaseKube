using ApiService.Domain.Security;
using ApiService.Definitions;
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
        public Guid TenantId
        {
            get
            {
                if (_identity is not null) return Guid.Parse(_identity.FindFirst(ClaimTypeEnum.TenantId.ToString())?.Value);
                if (IsAdmin) return DefaultValues.AdminTenantId;
                return Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypeEnum.TenantId.ToString())?.Value, out Guid result) ? result : Guid.Empty;
            }
        }
        public Guid? AuthorizationGroupId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypeEnum.AuthorizationGroupId.ToString())?.Value;
                if (Guid.TryParse(value, out Guid result)) return result;
                return null;
            }
        }
        public bool IsAdmin => bool.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.System)?.Value, out bool v) ? v : false;
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
        public void SetClaim()
        {
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Hash, "0"),
                    new Claim(ClaimTypes.Name, string.Empty),
                    new Claim(ClaimTypes.System, true.ToString()),
                    new Claim(ClaimTypes.Email, string.Empty),
                    new Claim(ClaimTypeEnum.Expires.ToString(), DateTime.Now.ToString()),
                    new Claim(ClaimTypeEnum.TenantId.ToString(), DefaultValues.AdminTenantId.ToString()),
                };

            _identity = new ClaimsIdentity(claims);
        }
    }
}
