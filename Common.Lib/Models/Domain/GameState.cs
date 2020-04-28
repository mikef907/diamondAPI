using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Lib.Models.DM
{
    public class GameState
    {
        public Guid Id { get; set; }
        public int TurnNumber { get; set; }
        public Player CurrentTurn { get; set; }
        public Guid CurrentTurnId { get; set; }
        public List<GameMove> Moves { get; set; }
        public Guid MatchId { get; set; }
        public Match Match { get; set; }
    }
}
