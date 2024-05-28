import { BuildingName, BuildingType, DataPlayerBuilding, DataPlayerGood, DataPlayerPlantation, GoodName, RoleName } from '../classes/general';
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
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentPlayerIndex != player.index) return "";
    
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
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentPlayerIndex != player.index) return "";

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellVictoryPoint) return "highlight-green";
    }

    return "";
  }


  getColonistSupplyHighlight():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentPlayerIndex != player.index) return "";

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellColonist && this.selectionService.selectedSlotId == 0) return "highlight-green";
    }

    return "";
  }

  getColonistHighlight(slotId:number){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentPlayerIndex != player.index) return "";

    if(slotId == this.selectionService.selectedSlotId) return "colonist-highlight-green";

    return "";
  }

  getPlayerGoodHighlightClass(good:DataPlayerGood){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentPlayerIndex != player.index) return "";

    switch(gs.currentRole){
      case RoleName.Builder:
        if(this.selectionService.sellGood 
          && this.selectionService.selectedGoodType == good.type) return "highlight-green";
        break;
      case RoleName.PostCaptain:
        if(this.selectionService.windroseStoredGood == good.type) return "highlight-red";
        if(this.selectionService.storeHouseStoredGoods.includes(good.type)) return "highlight-yellow";
        if(this.selectionService.smallWarehouseStoredType == good.type) return "green";
        if(this.selectionService.largeWarehouseStoredTypes.includes(good.type)) return "green";
        break;
      case RoleName.Craftsman:
        break;
    }

    return "";
  }

  getPlayerGoodCountHighlightRule(good:DataPlayerGood):string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentRole == RoleName.PostCaptain && gs.currentPlayerIndex == player.index) return "red";

    return "";
  }
}
