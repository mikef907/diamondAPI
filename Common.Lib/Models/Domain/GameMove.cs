using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Lib.Models.DM
{
    public class GameMove
    {
        public int Id { get; set; }
        public Guid GameStateId { get; set; }
        public GameState GameState { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public string MoveData { get; set; }
    }
}
