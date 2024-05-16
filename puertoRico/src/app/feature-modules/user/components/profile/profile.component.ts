import { User } from './../../models/user';
import { Component, Input, OnInit } from '@angular/core';
import { UserMainService } from '../../services/user-main.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit{
  userLoggedIn?:User;

  constructor(private userMainService:UserMainService){
  }

  ngOnInit(): void {
    this.userMainService.userLoggedIn?.subscribe(
      (user:User|null)=>{
      if(user) this.userLoggedIn = user;
    })
  }

  logout(){
    console.log(this.userLoggedIn);
    this.userLoggedIn = undefined;
  }

  deleteUser(user:User){
    this.userMainService.deleteUser(user.id!)
    .subscribe((user:User) => {
      console.log('user deleted:',user);
    });

    this.logout();
  }
}
