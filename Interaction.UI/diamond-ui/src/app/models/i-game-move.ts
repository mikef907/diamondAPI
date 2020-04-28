import { IPlayer } from './i-player';

export interface IGameMove {
    id: number;
    player: IPlayer,
    moveData: string
}