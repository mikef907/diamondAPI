using Common.Lib.Service_Agents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Lib.ServiceAgent
{
    public abstract class ServiceAgent : IServiceAgent
    {
        protected static SecurityToken _stsToken;
        protected HttpContext _context;
        protected ServiceAgentFactory _saFactory;
        protected AppSettings _appSettings;
        protected JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public ServiceAgent(IOptions<AppSettings> appSettings, IHttpContextAccessor context, ServiceAgentFactory factory)
        {
            _appSettings = appSettings.Value;
            _context = context.HttpContext;
            _saFactory = factory;
        }

        public bool HasValidToken() => _stsToken != null && _stsToken.ValidTo > DateTime.Now;
    }
}
