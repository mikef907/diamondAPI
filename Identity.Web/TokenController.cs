using AutoMapper;
using Common.Lib.DataAccess;
using Common.Lib.Models.DM;
using ElmahCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using EM = Common.Lib.Models.EM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.Web
{
    [Route("[controller]")]
    [Authorize(Policy = "STS")]
    public class TokenController : Controller
    {
        private IGenericUnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }
        public TokenController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public Guid? PostAuthenticateUser([FromBody]AuthenticateModel model) 
        {
            HttpContext.RiseError(new Exception(JsonConvert.SerializeObject(model)));
            HttpContext.RiseError(new Exception(JsonConvert.SerializeObject(_uow.Repo<EM.User>().Get(u => u.Email == model.Email && u.Password == model.Password).SingleOrDefault())));
            return _uow.Repo<EM.User>().Get(u => u.Email == model.Email && u.Password == model.Password).SingleOrDefault()?.Id;
        
        }

        [HttpPost("refresh")]
        public void PostRefreshToken([FromBody]RefreshToken model)
        {
            _uow.Repo<EM.RefreshToken>().Create(_mapper.Map<EM.RefreshToken>(model));
            _uow.Commit();
        }

        [HttpGet("refresh/{userId:guid}/{jti:guid}")]
        public RefreshToken GetRefreshToken(Guid userId, Guid jti) => _mapper.Map<RefreshToken>(_uow.Repo<EM.RefreshToken>().Get(r => r.UserId == userId && r.JwtId == jti).SingleOrDefault());

        [HttpDelete("refresh/{token}")]
        public void DeleteRefreshToken(string token)
        {
            _uow.Repo<EM.RefreshToken>().Delete(token);
            _uow.Commit();
        }
    }
}
