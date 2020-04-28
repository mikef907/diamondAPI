using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Lib.Models.DM
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        public List<PlayerMatch> PlayerMatches { get; set; }
    }
}
