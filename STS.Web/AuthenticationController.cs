using Common.Lib.Models.DM;
using Microsoft.AspNetCore.Mvc;
using STS.Lib;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace STS.Web
{
    
    public class AuthenticationController : Controller
    {
        private IAuthenticateService _service { get; set; }
        public AuthenticationController(IAuthenticateService service) => _service = service;

        [HttpPost("authenticate")]
        public async Task<IActionResult> PostAuthentication([FromBody]AuthenticateModel model) {
            var authentication = await _service.Authenticate(model);
            if (authentication == null) return BadRequest();
            else return Ok(authentication);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> PostRefreshToken([FromBody]AuthenticationModel model) {
            var authentication = await _service.ConsumeRefreshToken(model);
            if (authentication == null) return BadRequest();
            else return Ok(authentication);
        }
    }
}
