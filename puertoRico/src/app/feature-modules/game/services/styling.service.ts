import { Injectable } from '@angular/core';
import { GameService } from './game.service';
import { BuildingName, BuildingType, DataBuilding, DataPlantation, DataPlayerBuilding, DataPlayerPlantation, RoleName } from '../classes/general';

@Injectable({
  providedIn: 'root'
})
export class StylingService {


  constructor(private gameService:GameService) { }


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
    return buildingClassString;
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
