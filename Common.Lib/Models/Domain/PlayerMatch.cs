using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Lib.Models.DM
{
    public class PlayerMatch
    {
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public Guid MatchId { get; set; }
        public Match Match { get; set; }
    }
}
