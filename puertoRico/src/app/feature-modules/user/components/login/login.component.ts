import { User } from './../../models/user';
import { UserMainService } from './../../services/user-main.service';
import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent { 
  username:string = '';
  password:string = '';

  constructor(private userMainService:UserMainService){}

  loginWithUser(){
    this.userMainService.getUserByUsernameAndPassword(this.username, this.password)
    .subscribe(
      (user:User) => {
        this.userMainService.userLoggedIn?.next(user);
        console.log('login:',this.userMainService.userLoggedIn?.value);
      }
    );
    this.username = '';
    this.password = '';
    };
}
