import { Subscription } from 'rxjs';
import { GameService } from './../../feature-modules/game/services/game.service';
import { Component } from '@angular/core'; 
import { GameStateJson, RoleName } from 'src/app/feature-modules/game/classes/general';
import { HighlightService } from 'src/app/feature-modules/game/services/highlight.service';


@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss']
})
export class GamePageComponent {
  displayPlayerBoard = true;
  subscription: Subscription = new Subscription();

  constructor(public gameService:GameService,
    public highlightService:HighlightService,
  ){
    
  }

  checkForCaptainOrPostCaptain(){
    return this.gameService.gs.value.currentRole == RoleName.Captain || this.gameService.gs.value.currentRole == RoleName.PostCaptain;
  }

  ngOnInit(): void {
    this.subscription = this.gameService.gs.subscribe((gs:GameStateJson)=>{
      if(gs.currentRole == RoleName.Settler && gs.currentPlayerIndex == this.gameService.playerIndex) this.displayPlayerBoard = true;
      
    });
  }


  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }


  switchBoards(){
    this.displayPlayerBoard = !this.displayPlayerBoard;
  }
}
