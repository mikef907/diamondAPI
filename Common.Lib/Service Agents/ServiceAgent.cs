using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Lib.ServiceAgent
{
    public abstract class ServiceAgent : IServiceAgent
    {
        protected static SecurityToken _stsToken;

        protected AppSettings _appSettings;
        protected JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public ServiceAgent(IOptions<AppSettings> appSettings) => _appSettings = appSettings.Value;

        public bool HasValidToken() => _stsToken != null && _stsToken.ValidTo > DateTime.Now;
    }
}
