using Common.Lib.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Lib.Models.DM
{
    public class Match
    {
        public Guid Id { get; set; }
        public GameType Type { get; set; }
        public GameState GameState { get; set; }
        public List<PlayerMatch> PlayerMatches { get; set; }
        public Guid? WinnerId { get; set; }
        public Player Winner { get; set; }
        public bool Finished { get; set; }
    }
}
