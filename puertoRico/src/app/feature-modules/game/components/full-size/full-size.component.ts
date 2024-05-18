import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import {  DataSlot, GameStateJson } from '../../services/game-start-http.service';
import { Subscription } from 'rxjs';
import { RoleHttpService } from '../../services/role-http.service';
import { CdkDragDrop, transferArrayItem } from '@angular/cdk/drag-drop';
import {  DataPlayerPlantation } from '../../classes/deployables/plantation';
import { DataPlayerBuilding } from '../../classes/deployables/building';

@Component({
  selector: 'app-full-size',
  templateUrl: './full-size.component.html',
  styleUrls: [
    './full-size.component.scss',
    './full-size.component-part2.scss',
    '../../../../styles/colors.scss',
    '../../../../styles/plantations.scss',
    '../../../../styles/buildings.scss'
  ]
})
export class FullSizeComponent implements OnInit {
gs:GameStateJson = new GameStateJson();
subscription: Subscription = new Subscription();

myBuildings:DataPlayerBuilding[] = [];
myPlantations:DataPlayerPlantation[] = [];
buildingsMatrix:DataPlayerBuilding[][] = [];

  constructor(
    public gameService: GameService,
    public roleHttp: RoleHttpService
  ){}

  ngOnInit(): void {
    this.subscription = this.gameService.gs.subscribe((gs:GameStateJson) => {
      this.gs = gs;
      if(gs.players.length <= 0) return; 
      let player = gs.players[this.gameService.playerIndex];
      this.myPlantations = player.plantations;  
      this.buildingsMatrix = this.gameService.sortBuildings(player.buildings);
    })
  }

  ngOnDestroy(): void{
    this.subscription.unsubscribe();
  }

  sortSlots(slots: DataSlot[]):DataSlot[]{
      return slots.sort((a,b) => a.id - b.id);
  }

  onSlotClick(slot:DataSlot){
    this.roleHttp.postSlot(slot.id, this.gameService.gs.value.id, this.gameService.playerIndex)
    .subscribe({
      next: (result:GameStateJson) => {
        console.log('success: ',result);
        
        this.gameService.gs.next(result);
      },
      error: (response:any)=> {
        console.log("error:",response.error.text);
      }
    });
  }

  dropPlantation(event: CdkDragDrop<DataPlayerPlantation[]>){

    if(event.item.data == 5){
      this.roleHttp.postQuarry(this.gs.id, this.gameService.playerIndex).subscribe({
        next: (gs:GameStateJson) => {
          this.gameService.gs.next(gs);
        }
      });
      return;
    } 
    if(event.item.data == 6){
      this.roleHttp.postUpsideDown(this.gs.id, this.gameService.playerIndex).subscribe({
        next: (gs:GameStateJson) => {
          this.gameService.gs.next(gs);
        }
      });
      return;
    } 

    if (event.previousContainer === event.container) return;
    else this.roleHttp.postPlantation(event.item.data.id, this.gameService.gs.value.id, this.gameService.playerIndex)
    .subscribe({
      next: (result:GameStateJson) => {
        this.gameService.gs.next(result);
      },
      error: (response:any)=> {
        console.log("error:",response.error.text);
      }
    });
    
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex,
    );
  }

}
