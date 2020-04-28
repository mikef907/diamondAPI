import { Component, OnInit } from '@angular/core';
import { IUserModel } from '../models/i-user-model';
import { IdentityService } from '../services/identity.service';
import { User } from '../models/user';
import { StsService } from '../services/sts.service';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.css']
})
export class CreateUserComponent implements OnInit {
  model: IUserModel =
    {
      id: null,
      firstName: null,
      lastName: null,
      password: null,
      email: null
    };

  constructor(private identityService: IdentityService, private stsService: StsService) { }

  ngOnInit(): void {
  }

  create = () => this.identityService.postCreate(this.model).subscribe(result =>
    this.stsService.postLogin({ email: this.model.email, password: this.model.password }).subscribe(token =>
      this.identityService.getUser(result.id).subscribe()));

}
