using Common.Lib.Models.DM;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Lib.ServiceAgent
{
    public class IdentityAgent : IServiceAgent
    {
        private static SecurityToken _stsToken;

        private AppSettings _appSettings;
        private JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public IdentityAgent(IOptions<AppSettings> appSettings) => _appSettings = appSettings.Value;
        
        public bool HasValidToken() => _stsToken != null && _stsToken.ValidTo > DateTime.Now;

        /// <summary>
        /// This method will be used by the STS to check against Identity for user authentication, given that it makes sense that this will have
        /// special authentication that the STS can provide directly
        /// </summary>
        /// <param name="model">AuthenticateModel</param>
        /// <param name="">SecurityToken provided by the STS</param>
        /// <returns>User guid if authenticated</returns>
        public async Task<Guid?> Authenticate(AuthenticateModel model, SecurityToken stsToken)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenHandler.WriteToken(stsToken));
                var result = await http.PostAsync($"{_appSettings.IdentityURL}user/authenticate", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                return JsonConvert.DeserializeObject<Guid?>(await result.Content.ReadAsStringAsync());
            }
        }
    }
}
