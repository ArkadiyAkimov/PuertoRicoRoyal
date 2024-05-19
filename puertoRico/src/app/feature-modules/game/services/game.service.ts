
import { DataPlayerGood, GameStartHttpService, GameStateJson, StartGameOutput } from './game-start-http.service';
import { Injectable, OnInit } from '@angular/core';
import { BuildingType, ColorName, DataBuilding, DataPlayerBuilding } from '../classes/deployables/building';
import { BehaviorSubject } from 'rxjs';
import { DataPlantation, DataPlayerPlantation, GoodType } from '../classes/deployables/plantation';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class GameService{
  
  startGameOutput!:StartGameOutput;

  playerIndex:number = 0;
  numOfPlayers:number = 4;

  gameId:number = 25;
  gs = new BehaviorSubject<GameStateJson>(new GameStateJson()); 
  buildingTypes: BuildingType[] = [];
  goodTypes: GoodType[] = [];
  selectedShip: number = 4;
  errorToUI:string = 'error to ui';
  storedGoodTypes:number[] = [6,6,6,6];
  targetStorageIndex = 1;
  isHotSeat:boolean = false;

  private hubConnection: HubConnection;

  constructor(private gameStartHttp:GameStartHttpService){ 
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(`${environment.apiUrl}/updatehub`)
    .build();

    this.initializeGoodTypes();
    this.initializeUpdateHub();
  }

  joinOrInitGame(){
    this.gameStartHttp.postNewGame(this.gameId ,this.numOfPlayers ,this.playerIndex)
    .subscribe({
      next:(startGameOutput:StartGameOutput) => {
      this.buildingTypes = startGameOutput.buildingTypes;
      this.startGameOutput = startGameOutput;
      this.gs.next(startGameOutput.gameState);
      //this.gameId = startGameOutput.gameState.id;
    },
    error: (error:JSON) => {
      this.errorToUI = JSON.stringify(error);
    }
    });
  }

  loadPlayerIndexFromStorage(){
    let indexFromStorage = localStorage.getItem("playerIndex");
      if(indexFromStorage) this.playerIndex = parseInt(indexFromStorage);
      console.log("index from storage ", indexFromStorage);
  }


  async initializeUpdateHub(){
     await this.hubConnection.start()
     .then(() => 
     {
      console.log('connection established');
      this.loadPlayerIndexFromStorage();
      this.selectIndex();
      this.joinOrInitGame();
    })
     .catch((reason:any) => { console.log(reason)});

     this.hubConnection.on('ReceiveUpdate', (gs:GameStateJson) => {
      console.log("update");
      if(this.isHotSeat)this.playerIndex = gs.currentPlayerIndex;  //hotseat adaptation (won't show vp correct)
      this.gs.next(gs);
    });
  }
  
  selectIndex() {
      this.hubConnection.invoke('SelectIndex', this.playerIndex);
  };

  getBuildingType(dataBuilding:DataBuilding):BuildingType|null{
    let buildingType = this.buildingTypes.find(bt => bt.name == dataBuilding.name);
    if(buildingType){
      if(dataBuilding.quantity == 0)
      {
        let copy:BuildingType = {...buildingType};
        copy.color = 8;
        return copy
      }
      return buildingType;
    } 
    else return null;
  }

  getPlayerBuildingType(dataBuilding:DataPlayerBuilding):BuildingType|null{
    let buildingType = this.buildingTypes.find(bt => bt.name == dataBuilding.name);
    if(buildingType){
      if(dataBuilding.quantity == 0)
      {
        let copy:BuildingType = {...buildingType};
        copy.color = 8;
        return copy
      }
      return buildingType;
    } 
    else return null;
  }

  getPlantationType(dataPlantation:DataPlantation):GoodType|null{
    let plantationType = this.goodTypes.find(pt => pt.good == dataPlantation.good);
    if(plantationType){

      return plantationType;
    } 
    else return null;
  }

  getPlayerPlantationType(dataPlantation:DataPlayerPlantation):GoodType|null{
    let plantationType = this.goodTypes.find(pt => pt.good == dataPlantation.good);
    if(plantationType){

      return plantationType;
    } 
    else return null;
  }

  getPlayerGoodType(dataGood:DataPlayerGood):GoodType|null{
    let goodType = this.goodTypes.find(gd => gd.good == dataGood.type);
    if(goodType) return goodType;
    else return null;
  }

  initializeGoodTypes(){      // move to back end same as building types
    let corn = new GoodType();
    corn.good = 0;
    corn.color = ColorName.yellow;
    corn.displayName = "corn";
    
    let indigo = new GoodType();
    indigo.good = 1;
    indigo.color = ColorName.blue;
    indigo.displayName = "indigo";
    
    let sugar = new GoodType();
    sugar.good = 2;
    sugar.color = ColorName.white;
    sugar.displayName = "sugar";
    
    let tobacco = new GoodType();
    tobacco.good = 3;
    tobacco.color = ColorName.burlywood;
    tobacco.displayName = "tobacco";

    let coffee = new GoodType();
    coffee.good = 4;
    coffee.color = ColorName.black;
    coffee.displayName = "coffee";

    let quarry = new GoodType();
    quarry.good = 5;
    quarry.color = ColorName.gray;
    quarry.displayName = "quarry";

    let upSideDown = new GoodType();
    upSideDown.good = 6;
    upSideDown.color = ColorName.green;
    upSideDown.displayName = "";

    this.goodTypes.push(corn,indigo,sugar,tobacco,coffee,quarry,upSideDown);
  }


  changeTargetStorageGood(good:DataPlayerGood){
    if(this.gs.value.currentRole != 7 || good.quantity == 0) return;
    let currPlayer = this.gs.value.players[this.gs.value.currentPlayerIndex];

    do{
      this.targetStorageIndex = (this.targetStorageIndex + 1)%4;
    }
    while(!this.hasWarehouseCondition(this.targetStorageIndex));

    if(this.storedGoodTypes.includes(good.type))
    {
      let index = this.storedGoodTypes.indexOf(good.type);
      this.storedGoodTypes[0] = good.type;
      this.storedGoodTypes[index] = 6;
      this.targetStorageIndex = (index-1)%4;
      return;
    } 

    
    this.storedGoodTypes[this.targetStorageIndex] = good.type;
  }

  hasWarehouseCondition(index:number)
  {
    let currPlayer = this.gs.value.players[this.gs.value.currentPlayerIndex];

    switch(index){
      case 0:
        if(!currPlayer.canUseSmallWarehouse && !currPlayer.canUseLargeWarehouse) return true;
        return false;
      case 1:
        if(currPlayer.canUseSmallWarehouse) return true;
        return false;
      case 2:
      case 3:
        if(currPlayer.canUseLargeWarehouse) return true;
        return false;
      default:
        return true;
    }
  }

  
  
  initMatrix(){
    let buildingsMatrix: DataPlayerBuilding[][] = [];
    for(let i=0 ; i< 4; i++){
      let newBuildings:DataPlayerBuilding[] = [];
      buildingsMatrix.push(newBuildings);
    }

    return buildingsMatrix;
  }

  sortBuildings(myBuildings:DataPlayerBuilding[]){
    myBuildings.sort((a,b)=> a.name - b.name);
    let buildingsMatrix = this.initMatrix();
    let occupiedBuildingSpaces:number[] = [0,0,0,0];

    for(let i=0; i< myBuildings.length; i++){
      if(Math.floor(i/4) == 0 && occupiedBuildingSpaces[i%4] + this.getPlayerBuildingType(myBuildings[i])!.size <= 3)
      {
        buildingsMatrix[i%4].push(myBuildings[i]);
        occupiedBuildingSpaces[i%4] += this.getPlayerBuildingType(myBuildings[i])!.size;
      }
      else if(occupiedBuildingSpaces[Math.floor((i-4)/2)] + this.getPlayerBuildingType(myBuildings[i])!.size <= 3)
      {
        buildingsMatrix[Math.floor((i-4)/2)].push(myBuildings[i]);
        occupiedBuildingSpaces[Math.floor((i-4)/2)] += this.getPlayerBuildingType(myBuildings[i])!.size;
      }
      else
      {
        buildingsMatrix[Math.floor(((i-4)/2)+1)].push(myBuildings[i]);
        occupiedBuildingSpaces[Math.floor(((i-4)/2)+1)] += this.getPlayerBuildingType(myBuildings[i])!.size;
      }
    }

    return buildingsMatrix;
  }


  numSequence(n: number): Array<number> {
    return Array(n);
  }

  pickAndRemoveRandomElement<T>(arr: T[]): T | undefined {
    if (arr.length === 0) {
      return undefined;
    }
    const randomIndex = Math.floor(Math.random() * arr.length);
    return arr.splice(randomIndex, 1)[0];
  }
}

