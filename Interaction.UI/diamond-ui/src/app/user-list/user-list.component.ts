import { GameService } from './../services/game.service';
import { IPlayer } from './../models/i-player';
import { Component } from '@angular/core';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent {
  constructor(public gameService: GameService) { }

  selectOpponent = (player: IPlayer) => this.gameService.setOpponent(player);

}
