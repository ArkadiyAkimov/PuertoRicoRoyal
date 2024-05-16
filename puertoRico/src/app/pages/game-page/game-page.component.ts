import { GameService } from './../../feature-modules/game/services/game.service';
import { Component } from '@angular/core'; 


@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss']
})
export class GamePageComponent {
  displayPlayerBoard = true;

  constructor(public gameService:GameService){}

  switchBoards(){
    this.displayPlayerBoard = !this.displayPlayerBoard;
  }
}
