import { User } from './../../feature-modules/user/models/user';
import { BehaviorSubject, flatMap } from 'rxjs';
import { UserMainService } from './../../feature-modules/user/services/user-main.service';
import { Component, OnInit} from '@angular/core';
import { GameService } from 'src/app/feature-modules/game/services/game.service';
import { GameStateJson } from 'src/app/feature-modules/game/services/game-start-http.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit{
  userLoggedIn?:User;
  isRegistering:boolean = false;

  constructor(
    private userMainService:UserMainService,
    public gameService:GameService,
    private router:Router
    ){}

  ngOnInit(): void {
    this.userMainService.userLoggedIn?.subscribe(
      (user:User|null)=>{
      if(user) this.userLoggedIn = user;
    }) 
  }

  chooseIndex(index:number){
    this.gameService.playerIndex = index;
    localStorage.setItem("playerIndex",index.toString());
    this.gameService.selectIndex();
    this.gameService.joinOrInitGame();
    this.router.navigate(['/game']);
  }
}
