import { IPlayer } from './i-player';
import { IGameState } from './i-game-state';

export interface IMatch {
    id: string,
    type: string,
    state: IGameState,
    players: IPlayer[]
}