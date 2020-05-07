using Common.Lib.Models.DM;
using Microsoft.AspNetCore.Mvc;
using STS.Lib;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace STS.Web
{

    public class AuthenticationController : Controller
    {
        private IAuthenticateService _service { get; set; }
        public AuthenticationController(IAuthenticateService service) => _service = service;

        [HttpPost("access-token")]
        [Consumes("application/x-www-form-urlencoded;charset=utf-8")]
        public async Task<IActionResult> PostAuthentication([FromForm]AccessTokenRequest model)
        {
            var accessToken = await _service.GrantAccessToken(model);
            if (accessToken == null) return BadRequest("Credentials invalid");
            else return Ok(accessToken);
        }

        [HttpPost("refresh-token")]
        [Consumes("application/x-www-form-urlencoded;charset=utf-8")]
        public async Task<IActionResult> PostRefreshToken([FromForm]RefreshTokenRequest model) 
        {
            model.Refresh_Token = WebUtility.UrlDecode(model.Refresh_Token);
            var accessToken = await _service.ConsumeRefreshToken(model);
            if (accessToken == null) return BadRequest("Credentials invalid");
            else return Ok(accessToken);
        }
    }
}
