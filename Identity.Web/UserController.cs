using AutoMapper;
using Common.Lib.DataAccess;
using DM = Common.Lib.Models.DM;
using EM = Common.Lib.Models.EM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.Web
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private IGenericUnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }
        public UserController(IGenericUnitOfWork uow, IMapper mapper) {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<DM.User> Get() => _mapper.Map<IEnumerable<DM.User>>(_uow.Repo<EM.User>().Get());

        [HttpGet("protected")]
        [Authorize]
        public void GetProtected()
        {

        }

        // GET api/<controller>/5
        [HttpGet("{id:guid}")]
        public DM.User Get(Guid id) => _mapper.Map<DM.User>(_uow.Repo<EM.User>().Find(id));

        // GET api/<controller>/5
        [HttpPost("authenticate")]
        [Authorize(Policy = "STS")]
        public Guid? PostAuthenticateUser([FromBody]DM.AuthenticateModel model) => _uow.Repo<EM.User>().Where(u => u.Email == model.Email && u.Password == model.Password).SingleOrDefault()?.Id;

        // POST api/<controller>
        [HttpPost]
        public DM.User Post([FromBody]DM.User user) {
            var entry = _uow.Repo<EM.User>().Create(_mapper.Map<EM.User>(user));
            _uow.Commit();
            return _mapper.Map<DM.User>(entry);
         }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]DM.User user) {
            _uow.Repo<EM.User>().Update(_mapper.Map<EM.User>(user));
            _uow.Commit();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(Guid id) => _uow.Repo<EM.User>().Delete(id);


    }
}
