import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IUserModel } from '../models/i-user-model';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import { Subject, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { IAuthenticateModel } from '../models/i-authenticate-model';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class StsService {
  constructor(private http: HttpClient, private jwtHelper: JwtHelperService) { }

  postLogin = (model: IAuthenticateModel) =>
    this.http.post(`${environment.stsRoot}authenticate`, model, { responseType: 'text' })
      .pipe(tap(result => localStorage.setItem('token', result)));
}


