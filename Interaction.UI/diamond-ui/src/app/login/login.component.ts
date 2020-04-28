import { Component, OnInit } from '@angular/core';
import { IAuthenticateModel } from '../models/i-authenticate-model';
import { StsService } from '../services/sts.service';
import { IdentityService } from '../services/identity.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  model: IAuthenticateModel = {
    email: null,
    password: null
  }

  constructor(private stsService: StsService, private identityService: IdentityService, private jwtHelper: JwtHelperService) { }

  ngOnInit(): void {
  }

  submit = () => this.stsService.postLogin(this.model).subscribe(res => {
    const jwt = this.jwtHelper.decodeToken(res);
    this.identityService.getUser(jwt.unique_name).subscribe();
  });
}
