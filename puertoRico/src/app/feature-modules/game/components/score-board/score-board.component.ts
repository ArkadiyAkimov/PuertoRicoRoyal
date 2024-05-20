import { GameStateJson, DataPlayer } from '../../classes/general';
import { GameService } from './../../services/game.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-score-board',
  templateUrl: './score-board.component.html',
  styleUrls: ['./score-board.component.scss']
})
export class ScoreBoardComponent implements OnInit{
  gs:GameStateJson = new GameStateJson();
  players:DataPlayer[] = [];

  constructor(private gameService:GameService){}

  ngOnInit(): void {
    this.gameService.gs.subscribe((gs:GameStateJson) => {
      this.gs = gs;
      this.players = gs.players;
      this.sortPlayers();
    })
  }

  sortPlayers(){
    this.players.sort((a,b) => b.score - a.score);
  }

  floor(value:number):number{
    return Math.floor(value);
  }

  breaker(value:number):number{
    return Math.round((value - Math.floor(value))*1000);
  }
}
