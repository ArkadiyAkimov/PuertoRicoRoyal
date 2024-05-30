import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { GameStateJson, DataPlayerBuilding, DataPlayer } from '../../classes/general';
import { GameService } from '../../services/game.service';
import { StylingService } from '../../services/styling.service';
import { HighlightService } from '../../services/highlight.service';
import { SelectionService } from '../../services/selection.service';

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
  rawAndfinalProductionArrays:number[][][] = this.gameService.getRawAndFinalProductionArrays();
  players:DataPlayer[] = [];

  constructor(
    public gameService:GameService,
    public stylingService:StylingService,
    public highlightService:HighlightService,
    public selectionService:SelectionService,
  ){}

  getSortedPlayerGoodButtons(player:DataPlayer){
    return player.goods.sort((a,b) => a.type - b.type );
  }

  ngOnInit(): void {
    this.subscription = this.gameService.gs.subscribe((gs:GameStateJson)=>{
      this.gs = gs;
      this.players = gs.players.slice(this.gameService.playerIndex).concat(gs.players.slice(0,this.gameService.playerIndex));
    })

    this.gameService.rawAndfinalProductionArrays.subscribe({
      next:(prodArr:number[][][]) => {
        this.rawAndfinalProductionArrays = prodArr;
      }
    })
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
