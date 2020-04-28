import { GameService } from './services/game.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Component } from '@angular/core';
import { IdentityService } from './services/identity.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  username: string;

  constructor(public identityService: IdentityService, private jwtHelper: JwtHelperService,
    public gameService: GameService) { }

  ngOnInit() {
    if (localStorage.getItem('token')
      && !this.jwtHelper.isTokenExpired(localStorage.getItem('token'))) {
      const guid = this.jwtHelper.decodeToken(localStorage.getItem('token')).unique_name;
      this.identityService.getUser(guid).subscribe();
    }

    this.gameService.Player.subscribe(player => {
      if (player) {
        this.gameService.initHub();
      }
    })

    // this.identityService.User.subscribe(user => {
    //   if (user) {
    //     this.gameService.initHub();
    //   }
    // });
  }

  createPlayer() {
    this.gameService.postPlayer({
      id: this.jwtHelper.decodeToken(localStorage.getItem('token')).unique_name,
      username: this.username,
      connectionId: null
    }).subscribe()
  }

  logout = () => {
    this.identityService.logout();
  }
}
