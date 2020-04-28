import { IGameMove } from './i-game-move';
import { IPlayer } from './i-player';
export interface IGameState {
    id: string,
    currentTurn: IPlayer,
    matchId: string,
    moves: IGameMove[]
}