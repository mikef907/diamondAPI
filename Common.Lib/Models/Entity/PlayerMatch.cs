using System;

namespace Common.Lib.Models.EM
{
    public class PlayerMatch
    {
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public Guid MatchId { get; set; }
        public Match Match { get; set; }
    }
}
