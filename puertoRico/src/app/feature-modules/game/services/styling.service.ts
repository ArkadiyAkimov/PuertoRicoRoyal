import { Injectable } from '@angular/core';
import { GameService } from './game.service';
import { BuildingType, DataBuilding, DataPlantation, DataPlayerBuilding, DataPlayerPlantation, GoodType, RoleName } from '../classes/general';
import { HighlightService } from './highlight.service';

@Injectable({
  providedIn: 'root'
})
export class StylingService {

  constructor(private gameService:GameService,
    private highlightService:HighlightService,
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

  getBuildingClasses(building:DataBuilding|DataPlayerBuilding):string{    //defines building visual style
    let buildingType = this.gameService.getBuildingType(building);

    if(buildingType == null) return "";
    let buildingClassString = "building";
    buildingClassString += " color" + buildingType.color;
    if(building as DataBuilding) buildingClassString += " "+ this.getBuildingHighlight(building as DataBuilding);
    buildingClassString += " building-size-" + buildingType.size;
    buildingClassString += " " + this.highlightService.getBuildingEffectHighlight(buildingType);
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
    if(this.gameService.gs.value.currentRole == RoleName.Draft){
    if(building.isDrafted) return "isDrafted";
    if(building.isBlocked) return "isBlocked";
    };
    
    if(building.quantity == 0) return "soldOut";
    return "";
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
}
