import { Injectable } from '@angular/core';
import { GameService } from './game.service';
import { BuildingName, BuildingType, DataBuilding, DataPlantation, DataPlayerBuilding, DataPlayerGood, DataPlayerPlantation, DataSlot, GoodName, GoodType, PlayerUtility, RoleName, SlotEnum, isAffordable } from '../classes/general';
import { HighlightService } from './highlight.service';
import { SelectionService } from './selection.service';

@Injectable({
  providedIn: 'root'
})
export class StylingService {
  playerUtility:PlayerUtility = new PlayerUtility();

  constructor(private gameService:GameService,
    private highlightService:HighlightService,
    private selectionService:SelectionService,
  ) { }

  getGovernorOrTurnClassesAndText(playerIndex:number):string[]{
    let classesAndText = ["",""];

    if(playerIndex == this.gameService.gs.value.governorIndex) classesAndText[0] += " red";
    if(playerIndex == this.gameService.gs.value.privilegeIndex) classesAndText[0] += " yellow";
    if(playerIndex == this.gameService.gs.value.currentPlayerIndex) classesAndText[0] += " green";
    
    if(playerIndex == this.gameService.gs.value.currentPlayerIndex) classesAndText[1] = "Turn";
    else classesAndText[1] = "No Turn"
    if(playerIndex == this.gameService.gs.value.privilegeIndex) classesAndText[1] = "Privilege";
    if(playerIndex == this.gameService.gs.value.governorIndex) classesAndText[1] = "Governor";

    return classesAndText;
  }

  getPlantationClasses(plantation:DataPlantation|DataPlayerPlantation|null){
    let plantationClasses = "plantation"
    
    if(plantation != null){
    if(plantation.good != 5)
      plantationClasses += " color" + plantation.good;
    }
    if(plantation?.good == 5) plantationClasses += " color7";
    if(plantation?.good == 7) plantationClasses += " forest";

    return plantationClasses;
  }

  getPlantationSlotClasses(plantation:DataPlantation|DataPlayerPlantation|null){
    let plantationSlotClasses = "colonist-slot ";
    if(plantation == null){
      plantationSlotClasses += this.getGoodTypeRingClass(5)
    }else
      plantationSlotClasses += this.getGoodTypeRingClass(plantation.good)

    return plantationSlotClasses;
  }

  getBuildingClasses(building:DataBuilding|DataPlayerBuilding,isPlayer:boolean):string{    //defines building visual style
    let buildingType = this.gameService.getBuildingType(building);

    if(buildingType == null) return "";
    let buildingClassString = "building";
    buildingClassString += " color" + buildingType.color;
    if(!isPlayer) buildingClassString += " "+ this.getBuildingHighlight(building as DataBuilding);
    buildingClassString += " building-size-" + buildingType.size;
    if(isPlayer) buildingClassString += " " + this.highlightService.getBuildingEffectHighlight(buildingType);
    return buildingClassString;
  }

  getBuildingArtClasses(building:DataBuilding|DataPlayerBuilding):string{
    let buildingType = this.gameService.getBuildingType(building);

    if(buildingType == null) return "";

    let buildingArtClassString = "building-art ";
    buildingArtClassString += buildingType.displayName.split(' ').join('-') +  " ";
    buildingArtClassString += " building-art-size-" + buildingType.size;

    return buildingArtClassString;
  }

  getBuildingHighlight(building:DataBuilding){        //defines building highlights
    let buildingClasses = "";

    if(this.gameService.gs.value.currentRole == RoleName.Draft){
    if(building.isDrafted) buildingClasses += " isDrafted ";
    if(building.isBlocked) buildingClasses += " isBlocked ";
    };

    if(building.quantity == 0) buildingClasses += "soldOut";
    return buildingClasses;
  }

  getBuildingPriceHighlight(building:DataBuilding){
    let priceClasses = "";

    let buildingAffordable = this.selectionService.checkPlayerBuildingAffordabilityState(building);

    switch(buildingAffordable){
      case isAffordable.Yes:
        return " affordable ";
      case isAffordable.WithBlackMarket:
        return " black-market-affordable ";
      case isAffordable.Not:
        return " unaffordable ";
      case isAffordable.BlackMarketBlocked:
        return " ";
    }
  
    return priceClasses;
  }

  getBuildingSlotClasses(building:DataBuilding|DataPlayerBuilding){
    let buildingSlotClasses = "colonist-slot ";
    let buildingType:BuildingType| null;

    buildingType = this.gameService.getBuildingType(building)
    if(buildingType == null)return;
    
    if(buildingType.isProduction){
      buildingSlotClasses += this.getGoodTypeRingClass(buildingType.good)
    }
    else buildingSlotClasses += " expansion-" + buildingType?.expansion;

    if(buildingType.size == 2) buildingSlotClasses += " large-building-ring";

    return buildingSlotClasses;
  }

  getColonistClass(slot:DataSlot){
    switch(slot.state){
    case SlotEnum.Colonist:
    return " colonist ";
    case SlotEnum.Noble:
    return " noble ";   
    default:
    return " display-none ";
    }
  }

  getGoodTypeRingClass(good:number){
    switch(good){
      case 1:
        return " indigo-ring";
      case 2:
        return " sugar-ring";
      case 3:
        return " tobacco-ring";
      case 4:
        return " coffee-ring";
      case 5:
        return " quarry-ring";
      default:
        return " corn-ring";
    }
  }

  getCargoShipClasses(shipIndex:number):string{
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    let shipClasses = "";

    switch(shipIndex){
      case 3: 
      shipClasses += " cargo-ship-card wharf ";
      if(this.playerUtility.hasActiveBuilding(BuildingName.Wharf, player) && !this.playerUtility.getBuilding(BuildingName.Wharf,player)?.effectAvailable){
      shipClasses += " sailed"
      }
        break;
      case 4: 
      shipClasses += " cargo-ship-card wharf ";
      if(this.playerUtility.hasActiveBuilding(BuildingName.SmallWharf, player) && !this.playerUtility.getBuilding(BuildingName.SmallWharf,player)?.effectAvailable){
        shipClasses += " sailed"
      }
        break;
        case 5: 
      shipClasses += " cargo-ship-card wharf ";
      if(this.playerUtility.hasActiveBuilding(BuildingName.RoyalSupplier, player) && !this.playerUtility.getBuilding(BuildingName.RoyalSupplier,player)?.effectAvailable){
        shipClasses += " sailed"
      }
        break;
      default:
      shipClasses += " cargo-ship-card ";
        break;
    }

    return shipClasses;
  }

  getGoodBarrelClass(good:DataPlayerGood){
    let goodClasses = this.gameService.getPlayerGoodType(good)?.displayName + "-barrel";

    return goodClasses;
  }
  
}
