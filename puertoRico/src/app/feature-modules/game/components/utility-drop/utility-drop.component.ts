import { CdkDragDrop, transferArrayItem } from '@angular/cdk/drag-drop';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { RoleHttpService } from '../../services/role-http.service';
import { GameStateJson, DataPlayerBuilding } from '../../classes/general';
import { SelectionService } from '../../services/selection.service';
import { HighlightService } from '../../services/highlight.service';

@Component({
  selector: 'app-utility-drop',
  templateUrl: './utility-drop.component.html',
  styleUrls: ['./utility-drop.component.scss']
})
export class UtilityDropComponent implements OnInit{
    gs = new GameStateJson();
    myBuildings: DataPlayerBuilding[] = [];
    isLargeBuildingDragging:boolean = false;

    constructor(
      public gameService:GameService,
      private selectionService:SelectionService,
      public roleHttpService:RoleHttpService,
      public highlightService:HighlightService,
      ){}

      ngOnInit(): void {
        this.gameService.gs.subscribe({
          next: (gs:GameStateJson) => {
            this.gs = gs;
            if(gs.players.length <= 0) return;
            this.myBuildings = gs.players[gs.currentPlayerIndex].buildings;
          }
        })
      }

      largeBuildingDragging(){
        if(this.isLargeBuildingDragging) return "large-building-dragging";
        else return "";
      }


      dropBuilding(event: CdkDragDrop<DataPlayerBuilding[]>){
  
        if (event.previousContainer === event.container) return;

        else if(this.selectionService.isBlackMarketActive){
          this.roleHttpService.postBlackMarketBuilding(event.item.data.id, this.gameService.gs.value.id, this.gameService.playerIndex, this.selectionService.sellColonist, this.selectionService.selectedSlotId, this.selectionService.sellGood,
            this.selectionService.selectedGoodType,this.selectionService.sellVictoryPoint)
            .subscribe({
              next: (result:GameStateJson) => {
                this.gameService.gs.next(result);
              },
              error: (response:any)=> {
                console.log("error:",response.error.text);
              }
            });}
        else this.roleHttpService.postBuilding(event.item.data.id, this.gameService.gs.value.id, this.gameService.playerIndex)
            .subscribe({
              next: (result:GameStateJson) => {
                this.gameService.gs.next(result);
              },
              error: (response:any)=> {
                console.log("error:",response.error.text);
              }
            });
        
        //event.container.data.splice(event.currentIndex, 1);
      }
}
