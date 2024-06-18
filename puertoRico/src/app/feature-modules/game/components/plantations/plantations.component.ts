import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { RoleHttpService } from '../../services/role-http.service';
import { BuildingName, DataPlantation, DataPlayer, GameStateJson, PlayerUtility, RoleName, SlotEnum } from '../../classes/general';
import { StylingService } from '../../services/styling.service';
import { SelectionService } from '../../services/selection.service';
import { HighlightService } from '../../services/highlight.service';

@Component({
  selector: 'app-plantations',
  templateUrl: './plantations.component.html',
  styleUrls: ['./plantations.component.scss']
})
export class PlantationsComponent implements OnInit{
  exposedPlantations: DataPlantation[] = [];
  gs: GameStateJson = new GameStateJson();
  player:DataPlayer;
  playerUtility:PlayerUtility;
 

  constructor(
    public gameService:GameService,
    public selectionService : SelectionService,
    public stylingService:StylingService,
    public highlightService:HighlightService,
    ){
      this.player = new DataPlayer();
      this.playerUtility = new PlayerUtility();
      
    }

    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (result:GameStateJson) => {
          this.gs = result;
          this.player = this.gs.players[this.gameService.playerIndex];
          this.exposedPlantations = result.plantations.filter(plantation => plantation.isExposed);
        }
      });
    }

  

   disablePlantationInteractionCheck():boolean{
              return this.gs.currentRole != RoleName.Settler 
                    || this.gs.currentPlayerIndex != this.gameService.playerIndex
                    || (this.player.tookTurn 
                    && !(this.playerUtility.hasActiveBuilding(BuildingName.Library,this.player)
                    && !this.playerUtility.getBuilding(BuildingName.Library,this.player)?.effectAvailable))
                  
                    
   }

   disableQuarryInteractionCheck():boolean{
    let constructionHutCheck =  (this.playerUtility.hasActiveBuilding(BuildingName.ConstructionHut,this.player))

    let canTakeQuarry = false;
    if(constructionHutCheck != undefined) canTakeQuarry = constructionHutCheck;
    if(this.player.index == this.gs.privilegeIndex) canTakeQuarry = true;
    if(this.player.tookTurn) canTakeQuarry = false;

    return !canTakeQuarry || this.disablePlantationInteractionCheck();
   }

   disableUpSideDownInteractionCheck(){
    let haciendaCheck =  (this.playerUtility.hasActiveBuilding(BuildingName.Hacienda,this.player)
    && this.playerUtility.getBuilding(BuildingName.Hacienda,this.player)?.effectAvailable)
    && !this.player.tookTurn;

    if(this.selectionService.isLandOfficeActive
      && (this.playerUtility.getBuilding(BuildingName.LandOffice,this.player)?.slots[0].state == SlotEnum.Colonist)
      && (this.player.doubloons > 0)){
      return false;
    }

    let canUseHacienda = false;
    if(haciendaCheck != undefined){
      canUseHacienda = haciendaCheck
    }

    return !canUseHacienda || this.disablePlantationInteractionCheck();
   }

   showForestOverlay(isUpsideDown:boolean = false){

    if(this.gs.currentRole == RoleName.Trader 
      && this.selectionService.isLandOfficeActive 
      && this.selectionService.takingForest 
      && isUpsideDown) return true;
    return this.selectionService.takingForest && !isUpsideDown && (this.gs.currentRole == RoleName.Settler);
   }

   showPlantationsList(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    let playerUtility = new PlayerUtility();
    
    if(playerUtility.hasActiveBuilding(BuildingName.LandOffice, player)) return true;

    return this.exposedPlantations.length != 0
   }
}
