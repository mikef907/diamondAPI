using ElmahCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Lib.ServiceAgent
{
    public abstract class ServiceAgent : IServiceAgent
    {
        protected static SecurityToken _stsToken;
        protected HttpContext _context;

        protected AppSettings _appSettings;
        protected JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public ServiceAgent(IOptions<AppSettings> appSettings, IHttpContextAccessor context)
        {
            _appSettings = appSettings.Value;
            _context = context.HttpContext;
        }

        public bool HasValidToken() => _stsToken != null && _stsToken.ValidTo > DateTime.Now;
    }
}
