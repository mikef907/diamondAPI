using Common.Lib.Models.DM;
using Microsoft.AspNetCore.Mvc;
using STS.Lib;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace STS.Web
{
    [Route("authenticate")]
    public class AuthenticationController : Controller
    {
        private IAuthenticateService _service { get; set; }
        public AuthenticationController(IAuthenticateService service) => _service = service;
        public async Task<IActionResult> PostAuthentication([FromBody]AuthenticateModel model) {
            string token = await _service.Authenticate(model);
            if (string.IsNullOrEmpty(token)) return BadRequest();
            else return Ok(token);
        } 
    }
}
