import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataPlayer, DataPlayerBuilding, GameStateJson } from '../../services/game-start-http.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-opponent-boards',
  templateUrl: './opponent-boards.component.html',
  styleUrls: [
    './opponent-boards.component.scss',
    './opponent-board-part2.scss',
    '../../../../styles/colors.scss',
    '../../../../styles/plantations.scss',
    '../../../../styles/barrels.scss',
    '../../../../styles/buildings.scss'
  ]
})
export class OpponentBoardsComponent implements OnInit {
  gs:GameStateJson = new GameStateJson();
  subscription: Subscription = new Subscription();
  buildingsMatrixes:DataPlayerBuilding[][][] = [];

  players:DataPlayer[] = [];

  desktopPattern:boolean[] = [true,false,false,true,true]

  constructor(
    public gameService:GameService,
  ){}

  getSortedPlayerGoodButtons(player:DataPlayer){
    return player.goods.sort((a,b) => a.type - b.type );
  }

  ngOnInit(): void {
    this.subscription = this.gameService.gs.subscribe((gs:GameStateJson)=>{
      this.gs = gs;
      
      this.players = gs.players.slice(this.gameService.playerIndex).concat(gs.players.slice(0,this.gameService.playerIndex));

      this.buildingsMatrixes = []; 
      for(let i=0; i<this.players.length; i++){
        this.buildingsMatrixes.push(this.gameService.sortBuildings(this.players[i].buildings));
      }
      
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
