import { UserMainService } from './../../services/user-main.service';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { User } from '../../models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerUser: User = new User();

constructor(private userMainService:UserMainService){}

ngOnInit():void {}  

    createUser(){
    this.userMainService
    .registerUser(this.registerUser)
    .subscribe((user:User) => {
      console.log('registered user:',user);
    });

    this.registerUser = new User();
    }  
}
