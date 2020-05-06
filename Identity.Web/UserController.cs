using AutoMapper;
using Common.Lib.DataAccess;
using Common.Lib.Models.DM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using EM = Common.Lib.Models.EM;

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
        public IEnumerable<User> Get() => _mapper.Map<IEnumerable<User>>(_uow.Repo<EM.User>().Get());

        [HttpGet("protected")]
        [Authorize]
        public void GetProtected()
        {

        }

        // GET api/<controller>/5
        [HttpGet("{id:guid}")]
        public User Get(Guid id) => _mapper.Map<User>(_uow.Repo<EM.User>().Find(id));


        // POST api/<controller>
        [HttpPost]
        public User Post([FromBody]User user) {
            var entry = _uow.Repo<EM.User>().Create(_mapper.Map<EM.User>(user));
            _uow.Commit();
            return _mapper.Map<User>(entry);
         }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]User user) {
            _uow.Repo<EM.User>().Update(_mapper.Map<EM.User>(user));
            _uow.Commit();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(Guid id) => _uow.Repo<EM.User>().Delete(id);


    }
}
