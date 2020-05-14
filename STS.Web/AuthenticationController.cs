using Common.Lib.Models.DM;
using Microsoft.AspNetCore.Mvc;
using STS.Lib;
using System.Net;
using System.Threading.Tasks;

namespace STS.Web
{

    public class AuthenticationController : Controller
    {
        private IAuthenticateService _service { get; set; }
        public AuthenticationController(IAuthenticateService service)
        {
            _service = service;
        }

        [HttpPost("access-token")]
        public async Task<IActionResult> PostAuthentication([FromForm]AccessTokenRequest model)
        {
            model.Password = WebUtility.UrlDecode(model.Password);
            var accessToken = await _service.GrantAccessToken(model);
            if (accessToken == null) return BadRequest("Credentials invalid");
            else return Ok(accessToken);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> PostRefreshToken([FromForm]RefreshTokenRequest model) 
        {
            model.Refresh_Token = WebUtility.UrlDecode(model.Refresh_Token);
            var accessToken = await _service.ConsumeRefreshToken(model);
            if (accessToken == null) return BadRequest("Credentials invalid");
            else return Ok(accessToken);
        }
    }
}
