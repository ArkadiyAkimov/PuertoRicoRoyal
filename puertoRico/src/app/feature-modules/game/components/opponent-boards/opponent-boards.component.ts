import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { Subscription } from 'rxjs';
import { GameStateJson, DataPlayerBuilding, DataPlayer } from '../../classes/general';
import { StylingService } from '../../services/styling.service';
import { SelectionService } from '../../services/selection.service';

@Component({
  selector: 'app-opponent-boards',
  templateUrl: './opponent-boards.component.html',
  styleUrls: [
    '../../../../styles/colors.scss',
    '../../../../styles/plantations.scss',
    '../../../../styles/barrels.scss',
    '../../../../styles/buildings.scss',
    '../full-size/full-size.component.scss',
    '../full-size/full-size.component-part2.scss',
    '../my-controls/my-controls.component.scss',
    './opponent-boards.component.scss',
  ]
})
export class OpponentBoardsComponent implements OnInit {
  gs:GameStateJson = new GameStateJson();
  subscription: Subscription = new Subscription();
  buildingsMatrixes:DataPlayerBuilding[][][] = [];

  players:DataPlayer[] = [];

  desktopPattern:boolean[] = [false,true,true,false,false]

  constructor(
    public gameService:GameService,
    public stylingService:StylingService,
    public selectionService : SelectionService,
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
