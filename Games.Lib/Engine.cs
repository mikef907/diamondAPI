using Common.Lib.DataAccess;
using Common.Lib.Models.EM;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Games.Lib
{
    public class Engine
    {
        private IGenericUnitOfWork _uow { get; set; }
        public Engine(IGenericUnitOfWork uow) => this._uow = uow;
        public bool CheckState(Guid matchId, out GameMove winner) {
            var state = _uow.Repo<GameState>().Where(m => m.Moves, m => m.MatchId == matchId).SingleOrDefault();
            
            winner = null;

            if (state.Moves.Count == 2)
            {
                var p1Move = state.Moves[0];
                var p2Move = state.Moves[1];

                if (p2Move.MoveData == p1Move.MoveData)
                {
                    return true;
                }
                else {
                    _uow.Repo<GameMove>().Reference(p1Move, p => p.Player);
                    _uow.Repo<GameMove>().Reference(p2Move, p => p.Player);

                    var p2MoveData = JsonConvert.DeserializeObject(p2Move.MoveData).ToString();

                    switch (JsonConvert.DeserializeObject(p1Move.MoveData)) {
                        case "rock":
                            if (p2MoveData == "scissors")
                                winner = p1Move;
                            else winner = p2Move;
                            return true;
                        case "paper":
                            if (p2MoveData == "rock")
                                winner = p1Move;
                            else winner = p2Move;
                            return true;
                        case "scissors":
                            if (p2MoveData == "paper")
                                winner = p1Move;
                            else winner = p2Move;
                            return true;

                    }
                }
                    
            }
            return false;
        }
    }
}
