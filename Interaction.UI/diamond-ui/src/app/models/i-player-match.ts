import { IMatch } from './i-match';
import { IPlayer } from './i-player';

export interface IPlayerMatch {
    playerId: string,
    player: IPlayer,
    matchId: string,
    match: IMatch
}