import { BuildingName, BuildingType, DataPlayerBuilding, DataPlayerPlantation, GoodName, RoleName } from '../classes/general';
import { GameService } from './game.service';
import { Injectable } from '@angular/core';
import { SelectionService } from './selection.service';

@Injectable({
  providedIn: 'root'
})
export class HighlightService {

  constructor(private gameService:GameService,
    private selectionService:SelectionService,
  ) {}

  getBuildingEffectHighlight(buildingType:BuildingType):string{
    
    switch(buildingType?.name){
      case BuildingName.ForestHouse:
         if(this.selectionService.takingForest) return "highlight-green";
        break;
      case BuildingName.BlackMarket:
        if(this.selectionService.isBlackMarketActive)  return "highlight-green";
        break;
      default:  
        return "";
    }
  
    return "";
  }

  getVictoryPointsHighlight():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellVictoryPoint) return "highlight-green";
    }

    return "";
  }

  getGoodHighlight(goodType:GoodName):string{
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellGood && this.selectionService.selectedGoodType == goodType) return "highlight-green";
    }

    return "";
  }

  getColonistSupplyHighlight():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellColonist && this.selectionService.selectedSlotId == 0) return "highlight-green";
    }

    return "";
  }

  getColonistHighlight(slotId:number){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(slotId == this.selectionService.selectedSlotId) return "colonist-highlight-green";

    return "";
  }
}
