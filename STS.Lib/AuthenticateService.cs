using Common.Lib;
using Common.Lib.Models.DM;
using Common.Lib.ServiceAgent;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace STS.Lib
{
    public interface IAuthenticateService
    {
        Task<string> Authenticate(AuthenticateModel model);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private AppSettings _appSettings;

        private IIdentityAgent _identityAgent;
        private JwtSecurityTokenHandler _tokenHandler { get; set; } = new JwtSecurityTokenHandler();
        private static SecurityToken _stsToken { get; set; }
        public AuthenticateService(IOptions<AppSettings> appSettings, IIdentityAgent identityAgent)
        {
            _appSettings = appSettings.Value;
            _identityAgent = identityAgent;
            if (_stsToken == null || _stsToken.ValidTo > DateTime.Now)
            {
                CreateToken();
            }
        }

        public async Task<string> Authenticate(AuthenticateModel model)
        {
            Guid? userId = await _identityAgent.Authenticate(model, _stsToken);

            if (userId.HasValue)
            {
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, userId.Value.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = _tokenHandler.CreateToken(tokenDescriptor);
                return _tokenHandler.WriteToken(token);
            }
            else
            {
                return null;

            }
        }

        private void CreateToken()
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            _stsToken = _tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim("STS", "true")
                    }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

        }
    }
}
