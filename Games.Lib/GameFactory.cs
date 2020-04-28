using Common.Lib.Models.EM;
using Common.Lib.Models.Primitives;
using System;

namespace Games.Lib
{
    public static class GameFactory
    {
        public static Match CreateRockPaperScissors() => new Match() { Type = GameType.RockPaperScissors, GameState = new GameState() };
        public static PlayerMatch CreatePlayerMatch(Guid matchId, Guid playerId) =>
            new PlayerMatch() { MatchId = matchId, PlayerId = playerId };

        public static GameMove CreateGameMove(Guid gameStateId, Guid playerId) =>
            new GameMove() { GameStateId = gameStateId, PlayerId = playerId };
    }
}
