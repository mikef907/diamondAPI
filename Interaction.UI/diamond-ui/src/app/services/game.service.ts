import { IGameState } from './../models/i-game-state';
import { IPlayerMatch } from './../models/i-player-match';
import { JwtHelperService } from '@auth0/angular-jwt';
import { IPlayer } from './../models/i-player';
import { BehaviorSubject, Subject, combineLatest, of, merge, forkJoin } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { tap, map, mergeMap, switchMap } from 'rxjs/operators';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { IMatch } from '../models/i-match';
import { IGameMove } from '../models/i-game-move';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private PlayerSubject = new BehaviorSubject<IPlayer>(null);
  Player = this.PlayerSubject.asObservable();

  private OpponentSubject = new BehaviorSubject<IPlayer>(null);
  Opponent = this.OpponentSubject.asObservable();

  private ActivePlayersSubject = new BehaviorSubject<IPlayer[]>(null);
  ActivePlayers = this.ActivePlayersSubject.asObservable();

  private ActiveMatchesSubject = new BehaviorSubject<IPlayerMatch[]>([]);
  ActiveMatches = this.ActiveMatchesSubject.asObservable();

  private IncomingChallengesSubject = new BehaviorSubject<IPlayer[]>([]);
  IncomingChallenges = this.IncomingChallengesSubject.asObservable();

  Reset = new Subject();

  hubConnection: HubConnection;

  constructor(private http: HttpClient, private jwtHelper: JwtHelperService) { }

  initHub() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.gamesRoot}games`, { accessTokenFactory: () => this.jwtHelper.tokenGetter() })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started!')
        this.Player.subscribe(player => {
          if (player) {
            this.hubConnection.invoke("JoinLobby");
          }
        })
      })
      .catch(err => console.log('Error while establishing connection :('));

    this.hubConnection.on("UserJoined", (player: IPlayer) => {
      console.log("Joined", player)
      this.ActivePlayers.subscribe(players => {
        const _p = players.find(p => p.id === player.id)
        if (_p) Object.assign(_p, player);
        else players.push(player);
      })
    })

    this.hubConnection.on("ActivePlayers", (players) => {
      console.log("active", players)
      this.ActivePlayersSubject.next(players);
    })

    this.hubConnection.on("ChallengeReceive", (player: IPlayer) => {
      console.log("incoming challenge from", player.username)

      this.IncomingChallengesSubject.pipe(map(challenges => {
        if (!challenges.some(c => c.id === player.id))
          challenges.push(player);
        return challenges;
      })).subscribe()
    });

    this.hubConnection.on("ChallengeResponse", (player: IPlayer, response: boolean) => {
      if (response) {
        alert(`${player.username} has accepted your challenge, ITS ON!`);
        this.getMatches().subscribe(() => alert('New match available in active challenges!'));
      } else {
        alert(`${player.username} has declined your challenge, they scared.`);
      }
    })

    this.hubConnection.on("DeclareWinner", (winner: IGameMove, matchId: string) => {
      if (winner) {
        alert(`${winner.player.username} won with ${winner.moveData}`)
      } else {
        alert(`It's a draw!`)
      }
      this.Reset.next();
    });
  }

  setOpponent(player: IPlayer) {
    this.Player.subscribe(p => {
      if (p.id !== player.id) {
        this.OpponentSubject.next(player);
      }
    })
  }

  getPlayer = () => this.http.get<IPlayer>(`${environment.gamesRoot}api/player`)
    .pipe(tap(player => this.PlayerSubject.next(player)));

  postPlayer = (model: IPlayer) =>
    this.http.post<IPlayer>(`${environment.gamesRoot}api/player`, model)
      .pipe(tap(player => this.PlayerSubject.next(player)));

  issueChallenge(connectionId: string) {
    this.hubConnection.invoke('IssueChallenge', connectionId);
  }

  respondToChallenge(response: boolean, opponent: IPlayer) {
    this.hubConnection.invoke("RespondToChallenge", opponent, response).then(() => {
      this.getMatches().subscribe(() => alert('New match available in active challenges!'));
    });
  }

  getMatches = () => this.http.get<IPlayerMatch[]>(`${environment.gamesRoot}api/player/matches`)
    .pipe(tap(matches => {
      this.ActiveMatchesSubject.next(matches);
    }));

  sendMove = (matchId: string, type: string) => this.hubConnection.invoke('SendMove', matchId, type)


  getGameMoves = (matchId: string) => this.http.get<IGameMove[]>(`${environment.gamesRoot}api/player/match-moves/${matchId}`)

  removeChallenge = (player: IPlayer) => this.IncomingChallengesSubject.pipe(map(challenges => {
    challenges.splice(challenges.findIndex(c => c.id === player.id), 1);
    return challenges;
  }));
}
