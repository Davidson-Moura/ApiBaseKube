using ApiService.Domain.Entities.Users;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using ApiService.Domain.Security;
using ApiService.Models.Users;
using System.Security.Claims;
using System.Text;

namespace ApiService.Services.Security
{
    public class TokenService
    {
        private readonly ApiOptions _apiOption;
        public TokenService(IOptions<ApiOptions> apiOption)
        {
            _apiOption = apiOption.Value;
        }
        private string GetToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_apiOption.IssuerSigningKey!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                NotBefore = DateTime.Now,
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _apiOption.Issuer,
                Audience = _apiOption.Audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var textToken = tokenHandler.WriteToken(token);

            return textToken;
        }
        public UserLoginResponseModel GenerateToken(User user, HttpResponse response)
        {
            var model = new UserLoginResponseModel()
            {
                UserId = user.Id,
                UserName = user.Name,
                ExpiresAt = DateTime.Now.AddMinutes(15)
            };
            var expires = DateTime.Now.AddMinutes(15);

            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Hash, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name??string.Empty),
                    //new Claim(ClaimTypes.NameIdentifier, user.LoginName??string.Empty),
                    new Claim(ClaimTypes.Email, user.Email??string.Empty),
                    new Claim(ClaimTypeEnum.Expires.ToString(), expires.ToString()),
                };
            /*
            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypeEnum.IsSuperUser.ToString(), user.Super.StringDefaultByType()));
                claims.Add(new Claim(ClaimTypeEnum.EnterpriseId.ToString(), Guid.Empty.ToString()));
                claims.Add(new Claim(ClaimTypeEnum.Role.ToString(), AuthorizeRoles.UserAdmin.ToString()));
            }
            else
            {
                claims.Add(new Claim(ClaimTypeEnum.IsSuperUser.ToString(), false.StringDefaultByType()));
                claims.Add(new Claim(ClaimTypeEnum.EnterpriseId.ToString(), user.EnterpriseId));
                claims.Add(new Claim(ClaimTypeEnum.Role.ToString(), AuthorizeRoles.UserCustomer.ToString()));
            }

            group.Roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)) );
            */

            var textToken = GetToken(claims, expires);

            var imgClaims = new Claim[]
            {
                new Claim(ClaimTypes.Hash, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "ViewFiles"),
            };
            var textTokenFiles = GetToken(imgClaims, expires);

            SameSiteMode same = SameSiteMode.None;
#if !DEBUG
            //same = SameSiteMode.Strict;
#endif
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Impede acesso via JavaScript (protege contra XSS)
                Secure = true,   // Apenas HTTPS (não use em localhost sem HTTPS)
                SameSite = same, // Protege contra CSRF
                Expires = DateTime.UtcNow.AddHours(6) // Expiração do cookie
            };
            response.Cookies.Append("AuthToken", textTokenFiles, cookieOptions);

            model.Token = textToken;
            model.ExpiresAt = expires;
            return model;
        }
    }
}
