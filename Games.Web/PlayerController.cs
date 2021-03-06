﻿using AutoMapper;
using Common.Lib.DataAccess;
using Common.Lib.Models.EM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using DM = Common.Lib.Models.DM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Games.Web
{
    [Route("[controller]")]
    [Authorize]
    public class PlayerController : Controller
    {
        private IGenericUnitOfWork _uow;
        private IMapper _mapper;
        public PlayerController(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public DM.Player GetPlayer() => _mapper.Map<DM.Player>(_uow.Repo<Player>().Find(Guid.Parse(User.Identity.Name)));

        [HttpPost]
        public DM.Player PostPlayer([FromBody]DM.Player model)
        {
            var player = _uow.Repo<Player>().Create(_mapper.Map<Player>(model));
            _uow.Commit();
            return _mapper.Map<DM.Player>(player);
        }

        [HttpGet("matches")]
        public IEnumerable<DM.PlayerMatch> GetPlayerMatches() {
            var userId = Guid.Parse(User.Identity.Name);
            var matches = _uow.Repo<PlayerMatch>()
               .Get(p => p.Include(p => p.Player), p => !p.Match.Finished 
                    && p.Match.PlayerMatches.Any(p => p.PlayerId == userId) && p.PlayerId != userId);
            return _mapper.Map<IEnumerable<DM.PlayerMatch>>(matches);
        }

        [HttpGet("match-moves/{matchId:guid}")]
        public IEnumerable<DM.GameMove> GetMatchMoves(Guid matchId) =>
            _mapper.Map<IEnumerable<DM.GameMove>>(
                _uow.Repo<GameMove>()
                .Get(gm => gm.Include(gm => gm.GameState),
                    gm => gm.GameState.MatchId == matchId
                    && gm.PlayerId == Guid.Parse(User.Identity.Name)));
    }
}
