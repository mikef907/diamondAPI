using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.Lib.Models.EM
{
    public class GameMove
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid GameStateId { get; set; }
        public GameState GameState { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public string MoveData { get; set; }
    }
}
