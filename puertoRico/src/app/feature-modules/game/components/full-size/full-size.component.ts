import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { Subscription } from 'rxjs';
import { RoleHttpService } from '../../services/role-http.service';
import { CdkDragDrop, transferArrayItem } from '@angular/cdk/drag-drop';
import { GameStateJson, DataPlayer, DataPlayerBuilding, DataPlayerPlantation, DataSlot, BuildingType, BuildingName, RoleName, DataBuilding, PlayerUtility, SlotEnum } from '../../classes/general';
import { StylingService } from '../../services/styling.service';
import { SelectionService } from '../../services/selection.service';
import { HighlightService } from '../../services/highlight.service';

@Component({
  selector: 'app-full-size',
  templateUrl: './full-size.component.html',
  styleUrls: [
    './full-size.component.scss',
    './full-size.component-part2.scss',
    '../../../../styles/colors.scss',
    '../../../../styles/plantations.scss',
    '../../../../styles/buildings.scss',
    '../../../../styles/variables.scss',
  ]
})
export class FullSizeComponent implements OnInit {
gs:GameStateJson = new GameStateJson();
subscription: Subscription = new Subscription();
player:DataPlayer = new DataPlayer();

myBuildings:DataPlayerBuilding[] = [];
myPlantations:DataPlayerPlantation[] = [];
buildingsMatrix:DataPlayerBuilding[][] = [];

  constructor(
    public gameService: GameService,
    public roleHttp: RoleHttpService,
    public selectionService : SelectionService,
    public stylingService:StylingService,
    public highlightService:HighlightService,
  ){}

  ngOnInit(): void {
    this.subscription = this.gameService.gs.subscribe((gs:GameStateJson) => {
      this.gs = gs;
      if(gs.players.length <= 0) return; 
      this.player = gs.players[this.gameService.playerIndex];
      this.myPlantations = this.player.plantations;  
      this.buildingsMatrix = this.gameService.sortBuildings(this.player.buildings);
    })
  }

  ngOnDestroy(): void{
    this.subscription.unsubscribe();
  }

  sortSlots(slots: DataSlot[]):DataSlot[]{
      return slots.sort((a,b) => a.id - b.id);
  }

  onSlotClick(slot:DataSlot, building:DataPlayerBuilding|null = null){ 
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    let playerUtility = new PlayerUtility();

    if(((building != null) && (building.name == BuildingName.GuestHouse)) && (gs.currentRole != RoleName.Mayor)) return;

    if(this.selectionService.isBlackMarketActive) this.selectionService.selectColonistForBlackMarket(slot.id);
    else if(gs.currentRole == RoleName.Mayor 
         && (((building != null) && (building.name != BuildingName.GuestHouse)) || building == null) 
         && (slot.state == SlotEnum.Vacant)
         && player.index == gs.privilegeIndex 
         && player.colonists == 0 
         && (!gs.mayorTookPrivilige || (playerUtility.hasActiveBuilding(BuildingName.Library,player) 
                                     && playerUtility.getBuilding(BuildingName.Library,player)?.effectAvailable))){
      this.roleHttp.postColonist(this.gameService.gs.value.id, this.gameService.playerIndex)
      .subscribe({
        next: (result:GameStateJson) => {
          console.log('success: Took mayor privilege.');
          this.postSlot(slot)
          this.gameService.gs.next(result);
        },
        error: (error:any)=> {
          console.log("error: postColonist");
          this.postSlot(slot)
        }
      });
      //setTimeout(() => this.postSlot(slot), 200);
    } 
    else{  
    this.postSlot(slot);
    }
  }

  postSlot(slot:DataSlot){
    let subscription = this.roleHttp.postSlot(slot.id, this.selectionService.noblesSelected, this.gameService.gs.value.id, this.gameService.playerIndex)
    .subscribe({
      next: (result:GameStateJson) => {
        console.log('success: postSlot');
        this.gameService.gs.next(result);
      },
      error: (error:any)=> {
        console.log("error: postSlot");
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
    else if(this.selectionService.takingForest){
      this.roleHttp.postForest(event.item.data.id, this.gameService.gs.value.id, this.gameService.playerIndex)
    .subscribe({
      next: (result:GameStateJson) => {
        this.gameService.gs.next(result);
      },
      error: (response:any)=> {
        console.log("error:",response.error.text);
      }
    });
    }
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
      event.container.data.length,
    );
  }


}
