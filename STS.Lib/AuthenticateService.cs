using Common.Lib;
using Common.Lib.Models.DM;
using Common.Lib.Models.EM;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace STS.Lib
{
    public interface IAuthenticateService
    {
        Task<string> Authenticate(AuthenticateModel model);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private AppSettings _appSettings;
        private JwtSecurityTokenHandler _tokenHandler { get; set; } = new JwtSecurityTokenHandler();
        private static SecurityToken _stsToken { get; set; }
        public AuthenticateService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            if (_stsToken == null || _stsToken.ValidTo > DateTime.Now) 
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



        public async Task<string> Authenticate(AuthenticateModel model)
        {
           

            using (var http = new HttpClient())
            {
                //TODO: Optimize~
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenHandler.WriteToken(_stsToken));
                var result = await http.PostAsync($"https://localhost:44310/api/user/authenticate", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                Guid? isAuthenticated = JsonConvert.DeserializeObject<Guid?>(await result.Content.ReadAsStringAsync());
                if (isAuthenticated.HasValue)
                {
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, isAuthenticated.Value.ToString())
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
        }
    }
}
