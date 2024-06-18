import { BuildingName, BuildingType, DataPlayerBuilding, DataPlayerGood, DataPlayerPlantation, GoodName, PlayerUtility, RoleName, SlotEnum } from '../classes/general';
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
    let playerUtility = new PlayerUtility();
    if(player == undefined) return "";

    if(gs.currentPlayerIndex != player.index) return "";
    if(playerUtility.getBuilding(buildingType.name, player)?.slots[0].state == SlotEnum.Vacant) return "";
    
    switch(buildingType?.name){
      case BuildingName.ForestHouse:
         if((gs.currentRole != RoleName.Settler) 
          && !(gs.currentRole == RoleName.Trader 
          && this.selectionService.isLandOfficeActive
          && playerUtility.getBuilding(BuildingName.LandOffice,player)?.slots[0].state == SlotEnum.Colonist)) return "";
         if(this.selectionService.takingForest) return "highlight-green";
         else return "highlight-yellow";
      case BuildingName.BlackMarket:
        if(gs.currentRole != RoleName.Builder) return "";
        if(this.selectionService.isBlackMarketActive)  return "highlight-green";
        else return "highlight-yellow";
      case BuildingName.TradingPost:
        if(gs.currentRole != RoleName.Trader) return "";
        if(this.selectionService.sellingToTradingPost) return "highlight-green";
        else return "highlight-yellow";
      case BuildingName.LandOffice:
        if(gs.currentRole != RoleName.Trader) return "";
        if(this.selectionService.isLandOfficeActive) return "highlight-green";
        else if(playerUtility.getBuilding(BuildingName.LandOffice,player)?.effectAvailable) return "highlight-yellow";
        break;
      case BuildingName.HuntingLodge:
        if(gs.currentRole != RoleName.Settler) return "";
        if(this.selectionService.isHuntingLodgeActive) return "highlight-green";
        else if(playerUtility.getBuilding(BuildingName.HuntingLodge,player)?.effectAvailable && (playerUtility.getBuilding(BuildingName.HuntingLodge,player)?.slots[0].state == SlotEnum.Colonist)) return "highlight-yellow";
         break;
        }

        return "";
  }

  getVictoryPointsHighlight():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if(gs.currentPlayerIndex != player.index) return "";

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellVictoryPoint) return "highlight-green";
    }
    return "";
  }

  getPlayerColonistsHighlight():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if(gs.currentPlayerIndex != player.index) return "";

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellColonist && this.selectionService.selectedSlotId == 0) return "highlight-green";
    }
    if(gs.currentRole == RoleName.Mayor){
      if(this.selectionService.noblesSelected == false && player.colonists > 0) return "highlight-blue";
    }
    return "";
  }

  getPlayerNoblesHighlight():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if(gs.currentPlayerIndex != player.index) return "";

    if(gs.currentRole == RoleName.Builder){
      if(this.selectionService.sellColonist && this.selectionService.selectedSlotId == 0) return "highlight-green";
    }
    if(gs.currentRole == RoleName.Mayor){
      if(this.selectionService.noblesSelected == true && player.nobles > 0) return "highlight-blue";
    }
    return "";
  }

  getColonistHighlight(slotId:number){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if(gs.currentPlayerIndex != player.index) return "";

    if(slotId == this.selectionService.selectedSlotId) return "colonist-highlight-green";

    return "";
  }

  getPlayerGoodHighlightClass(good:DataPlayerGood){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

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
        if((this.gameService.rawAndfinalProductionArrays.value[0][player.index][good.type] > 0) 
          && this.gameService.supplyGoods[good.type]) return "green";
        break;
      case RoleName.Captain:
        if(good.quantity > 0) return "green";
        break;
    }

    return "";
  }

  getPlayerGoodCountHighlightRule(good:DataPlayerGood):string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if(gs.currentRole == RoleName.PostCaptain && gs.currentPlayerIndex == player.index) return "red";

    return "";
  }

  getPlayerGoodBubbleHighlightRule():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(gs.currentRole == RoleName.Captain && gs.currentPlayerIndex == player.index) return "yellow";

    return "";
  }

  getPlayerVPBubbleHighlightRule():string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if(gs.currentRole == RoleName.Captain && gs.currentPlayerIndex == player.index) return "green";

    return "";
  }

  getCargoShipHighlights(shipIndex:number):string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return "";

    if((this.selectionService.selectedShip == shipIndex)
      && (gs.currentPlayerIndex == player.index)) return "ship-highlight";
    return "";
  }

  
}
