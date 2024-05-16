import { DataSlot } from "../../services/game-start-http.service";
import { ColorName } from "./building";

export class DataPlantation {
    id:number = 0;
    gameStateId:number = 0;
    slot:DataSlot = new DataSlot();
    isExposed:boolean = false;
    isDiscarded:boolean = false;
    good:number = 0;
  }
  
  export class DataPlayerPlantation {
    id:number = 0;
    dataPlayerId:number = 0;
    slot:DataSlot = new DataSlot();
    good:number = 0;
  }
  
  export class GoodType {
    good:number = 0;
    displayName:string = '';
    color:ColorName = 0;
  }