import { GameService } from './game.service';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IUserModel } from '../models/i-user-model';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../models/user';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  private UserSubject = new BehaviorSubject(null);
  User: Observable<User> = this.UserSubject.asObservable();

  constructor(private http: HttpClient, private gameService: GameService) {

  }

  postCreate = (model: IUserModel) => this.http.post<IUserModel>(`${environment.identityRoot}api/user`, model);

  getUser = (id: string) => this.http.get<IUserModel>(`${environment.identityRoot}api/user/${id}`)
    .pipe(tap(result => {
      if (result) {
        this.UserSubject.next(new User(result));
        this.gameService.getPlayer().subscribe();
      } else {
        this.logout();
      }
    }));

  logout() {
    localStorage.clear();
    console.log('LocalStorage cleared!')
    this.UserSubject.next(null);
  }
}
