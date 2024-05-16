import { CdkDragDrop, transferArrayItem } from '@angular/cdk/drag-drop';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataBuilding, DataPlayerBuilding } from '../../classes/deployables/building';
import { RoleHttpService } from '../../services/role-http.service';
import { GameStateJson } from '../../services/game-start-http.service';

@Component({
  selector: 'app-utility-drop',
  templateUrl: './utility-drop.component.html',
  styleUrls: ['./utility-drop.component.scss']
})
export class UtilityDropComponent implements OnInit{
    gs = new GameStateJson();
    myBuildings: DataPlayerBuilding[] = [];

    constructor(
      public gameService:GameService,
      public roleHttpService:RoleHttpService
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

      dropBuilding(event: CdkDragDrop<DataPlayerBuilding[]>){
  
        if (event.previousContainer === event.container) return;
        else this.roleHttpService.postBuilding(event.item.data.id, this.gameService.gs.value.id, this.gameService.playerIndex)
        .subscribe({
          next: (result:GameStateJson) => {
            this.gameService.gs.next(result);
          },
          error: (response:any)=> {
            console.log("error:",response.error.text);
          }
        });
        
        event.container.data.splice(event.currentIndex, 1);
      }
}
