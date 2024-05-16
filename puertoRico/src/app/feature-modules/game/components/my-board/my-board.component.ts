import { Component } from '@angular/core';
import { GameService } from '../../services/game.service';


@Component({
  selector: 'app-my-board',
  templateUrl: './my-board.component.html',
  styleUrls: ['./my-board.component.scss']
})
export class MyBoardComponent {
  constructor(
    public gameService: GameService,
  ){}
}
