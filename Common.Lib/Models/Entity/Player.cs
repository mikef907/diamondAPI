using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Lib.Models.EM
{
    public class Player
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        public List<PlayerMatch> PlayerMatches { get; set; }
    }
}
