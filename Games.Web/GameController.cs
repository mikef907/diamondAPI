using AutoMapper;
using Common.Lib.DataAccess;
using Common.Lib.Models.EM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using DM = Common.Lib.Models.DM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Games.Web
{
    [Authorize]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private IGenericUnitOfWork _uow;
        private IMapper _mapper;
        public GameController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
    }
}
