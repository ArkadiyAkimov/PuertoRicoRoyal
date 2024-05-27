import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { GameStateJson, DataPlayerBuilding, DataPlayer } from '../../classes/general';
import { GameService } from '../../services/game.service';
import { StylingService } from '../../services/styling.service';

@Component({
  selector: 'app-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: [
  '../my-controls/my-controls.component.scss',
  './progress-bar.component.scss',
  ]
})
export class ProgressBarComponent {
  gs:GameStateJson = new GameStateJson();
  subscription: Subscription = new Subscription();
  buildingsMatrixes:DataPlayerBuilding[][][] = [];

  players:DataPlayer[] = [];

  desktopPattern:boolean[] = [true,false,false,true,true]

  constructor(
    public gameService:GameService,
    public stylingService:StylingService,
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
