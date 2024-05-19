import { DataSlot } from "../../services/game-start-http.service"

export class DataBuilding {
  id:number = 0;
  gameStateId:number = 0;
  name:number = 0;
  slots:DataSlot[] = [];
  quantity:number = 0;
}

export class DataPlayerBuilding {
  id:number = 0;
  dataPlayerId:number = 0;
  name:number = 0;
  slots:DataSlot[] = [];
  quantity:number = 0;
  buildOrder:number = 0;
}

export class BuildingType {
  name: number = 0;
  displayName:string = '';
  good:number = 0;
  color:ColorName = 0;
  price:number = 0;
  victoryScore:number = 0;
  isProduction:boolean = false;
  slots:number = 0;
  size:number = 0;
  startingQuantity:number = 0;
}

  export enum ColorName
  {
      yellow,
      blue,
      white,
      burlywood,
      black,
      violet,
      gray,
      green,
      zeroQuantBuilding
  }

