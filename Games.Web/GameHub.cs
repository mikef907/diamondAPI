using AutoMapper;
using Common.Lib.DataAccess;
using Common.Lib.Models.EM;
using DM = Common.Lib.Models.DM;
using Games.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games.Web
{
    public class GameHub : Hub
    {
        private static List<Player> _activePlayers { get; set; } = new List<Player>();
        private IGenericUnitOfWork _uow;
        private IMapper _mapper;
        public GameHub(IGenericUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [Authorize]
        public async Task JoinLobby() 
        {
            var player = _uow.Repo<Player>().Find(Guid.Parse(Context.User.Identity.Name));
            player.ConnectionId = Context.ConnectionId;
            // I don't thin this actually needs to be saved since it changes with every session
            //_uow.Commit();

            if (!_activePlayers.Any(p => p.Id == player.Id))
                _activePlayers.Add(player);
            else {
                _activePlayers.Find(p => p.Id == player.Id).ConnectionId = player.ConnectionId;
            }

            await Clients.Others.SendAsync("UserJoined", player);
            await Clients.Client(Context.ConnectionId).SendAsync("ActivePlayers", _activePlayers);
        }

        [Authorize]
        public async Task IssueChallenge(string connectionId) {
            var player = _activePlayers.Find(p => p.Id == Guid.Parse(Context.User.Identity.Name));

            await Clients.Client(connectionId).SendAsync("ChallengeReceive", player);
        }

        [Authorize]
        public async Task RespondToChallenge(Player opponent, bool response)
        {
            var player = _activePlayers.Find(p => p.Id == Guid.Parse(Context.User.Identity.Name));

            if (response)
            {
                var match = _uow.Repo<Match>().Create(GameFactory.CreateRockPaperScissors());
                _uow.Repo<PlayerMatch>().Create(GameFactory.CreatePlayerMatch(match.Id, player.Id));
                _uow.Repo<PlayerMatch>().Create(GameFactory.CreatePlayerMatch(match.Id, opponent.Id));
                _uow.Commit();
            }

            await Clients.Client(opponent.ConnectionId).SendAsync("ChallengeResponse", opponent, response);
        }

        [Authorize]
        public async Task SendMove(Guid matchId, string move) {
            var gameStateId = _uow.Repo<GameState>().SingleOrDefault(s => s.MatchId == matchId).Id;
            var gamemove = GameFactory.CreateGameMove(gameStateId, Guid.Parse(Context.User.Identity.Name));
            gamemove.MoveData = JsonConvert.SerializeObject(move);
            _uow.Repo<GameMove>().Create(gamemove);
            

            GameMove winner = null;
            if (new Engine(_uow).CheckState(matchId, out winner)) {
                var players = _uow.Repo<PlayerMatch>().Where(m => m.MatchId == matchId).Select(p => p.PlayerId);

                var match = _uow.Repo<Match>().Find(matchId);
                match.WinnerId = winner?.Player.Id;
                match.Finished = true;
                _uow.Repo<Match>().Update(match);

                players.ToList().ForEach(async id => {
                    await Clients.Client(_activePlayers.Single(x => x.Id == id).ConnectionId).SendAsync("DeclareWinner", _mapper.Map<DM.GameMove>(winner), matchId);
                });
            }
            _uow.Commit();
        }
    }
}
