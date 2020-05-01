using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Lib.Models.EM
{
    public class GameState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int TurnNumber { get; set; }
        public Player CurrentTurn { get; set; }
        public Guid? CurrentTurnId { get; set; }
        public List<GameMove> Moves { get; set; }
        public Guid MatchId { get; set;}
        public Match Match { get; set; }
    }
}