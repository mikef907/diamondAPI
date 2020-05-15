using Common.Lib.Models.DM;
using Common.Lib.Service_Agents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Lib.ServiceAgent
{
    public class IdentityAgent : ServiceAgent, IIdentityAgent
    {
        public IdentityAgent(IOptions<AppSettings> appSettings, IHttpContextAccessor context, ServiceAgentFactory factory) 
            : base(appSettings, context, factory) { }

        /// <summary>
        /// This method will be used by the STS to check against Identity for user authentication, given that it makes sense that this will have
        /// special authentication that the STS can provide directly
        /// </summary>
        /// <param name="model">AuthenticateModel</param>
        /// <param name="">SecurityToken provided by the STS</param>
        /// <returns>User guid if authenticated</returns>
        public async Task<Guid?> Authenticate(AuthenticateModel model, SecurityToken stsToken)
        {
            using (var http = _saFactory.CreateHttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenHandler.WriteToken(stsToken));
                var result = await http.PostAsync($"{_appSettings.IdentityURL}token/authenticate", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                return JsonConvert.DeserializeObject<Guid?>(await result.Content.ReadAsStringAsync());
            }
        }

        public async Task CreateRefreshToken(RefreshToken model, SecurityToken stsToken)
        {
            using (var http = _saFactory.CreateHttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenHandler.WriteToken(stsToken));
                await http.PostAsync($"{_appSettings.IdentityURL}token/refresh", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            }
        }

        public async Task<RefreshToken> FetchRefreshToken(Guid userId, Guid jti, SecurityToken stsToken) 
        {
            using (var http = _saFactory.CreateHttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenHandler.WriteToken(stsToken));
                var result = await http.GetAsync($"{_appSettings.IdentityURL}token/refresh/{userId}/{jti}");
                return JsonConvert.DeserializeObject<RefreshToken>(await result.Content.ReadAsStringAsync());
            }
        }

        public async Task RemoveRefreshToken(string token, SecurityToken stsToken) {
            using (var http = _saFactory.CreateHttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenHandler.WriteToken(stsToken));
                await http.DeleteAsync($"{_appSettings.IdentityURL}token/refresh/{token}");
            }
        }
    }
}
