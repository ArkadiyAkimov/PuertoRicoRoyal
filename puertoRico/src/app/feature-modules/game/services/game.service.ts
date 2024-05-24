
import { Injectable, OnInit } from '@angular/core';
 import { BehaviorSubject } from 'rxjs';
 import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from 'src/environments/environment.development';
import { StartGameOutput, GameStateJson, BuildingType, GoodType, DataBuilding, DataPlayerBuilding, DataPlantation, DataPlayerPlantation, DataPlayerGood, ColorName, BuildingName, PlayerUtility, RoleName, GameStartInput } from '../classes/general';
import { GameStartHttpService } from './game-start-http.service';
import { ScrollService } from './scroll.service';

@Injectable({
  providedIn: 'root'
})
export class GameService{
  
  debugOptions:boolean = true;
  isHotSeat:boolean = true;
  startGameOutput!:StartGameOutput;

  playerIndex:number = 0;

  gs = new BehaviorSubject<GameStateJson>(new GameStateJson()); 
  buildingTypes: BuildingType[] = [];
  goodTypes: GoodType[] = [];
  selectedShip: number = 4;
  errorToUI:string = 'error to ui';
  storedGoodTypes:number[] = [6,6,6,6];
  finishedInitialStorage:boolean = false;
  targetStorageIndex = 1;

  private hubConnection: HubConnection;

  startGameInput:GameStartInput = new GameStartInput()

  constructor(private gameStartHttp:GameStartHttpService, private scrollService:ScrollService){ 
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(`${environment.apiUrl}/updatehub`)
    .build();

    this.initializeGoodTypes();
    this.initializeUpdateHub();
  }

  joinOrInitGame(){
    this.startGameInput.gameId = 38;
    this.startGameInput.numOfPlayers = 4;
    this.startGameInput.playerIndex = 0;
    this.startGameInput.isDraft = true;
    this.startGameInput.isBuildingsExpansion = false;
    this.startGameInput.isNoblesExpansion = false;

    this.gameStartHttp.postNewGame(this.startGameInput)
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
      this.scrollService.autoScroll(gs.currentRole);
      this.gs.next(gs);
    });
  }
  
  selectIndex() {
      this.hubConnection.invoke('SelectIndex', this.playerIndex);
  };


  getBuildingType(dataBuilding:DataBuilding|DataPlayerBuilding):BuildingType|null{
    let buildingType = this.buildingTypes.find(bt => bt.name == dataBuilding.name);
    if(buildingType) return buildingType;
    else return null;
  }

  getPlantationType(dataPlantation:DataPlantation|DataPlayerPlantation):GoodType|null{
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
    if(this.gs.value.currentRole != RoleName.PostCaptain || good.quantity == 0 || this.playerIndex != this.gs.value.currentPlayerIndex) return;
    
    if(this.storedGoodTypes[0] != 6) this.finishedInitialStorage = true
    this.finishedInitialStorage = this.storedGoodTypes[0] != 6;

    let player = this.gs.value.players[this.playerIndex];
      let playerGoodTypes = 0;
      let playerStoredGoodTypes = 0;

      player.goods.forEach(good => {
        if(good.quantity > 0) playerGoodTypes++;
      });

      this.storedGoodTypes.forEach(goodType => {
        if(goodType != 6) playerStoredGoodTypes++;
      });


    if(!this.finishedInitialStorage){
    if(this.storedGoodTypes.includes(good.type)) return;

    do{
      this.targetStorageIndex = (this.targetStorageIndex + 1)%4;
    }
    while(!this.hasWarehouseCondition(this.targetStorageIndex));
    this.storedGoodTypes[this.targetStorageIndex] = good.type;
    
    }else if(playerGoodTypes == playerStoredGoodTypes){
        this.storedGoodTypes= [6,6,6,6];
        this.targetStorageIndex = 0;
        this.finishedInitialStorage=false
    }else{

      if(this.storedGoodTypes.includes(good.type)){
        let index = this.storedGoodTypes.indexOf(good.type)
         this.targetStorageIndex = index
      }else{
        this.storedGoodTypes[this.targetStorageIndex] = good.type
      }
    }
  }

  hasWarehouseCondition(index:number)
  {
    let player = this.gs.value.players[this.gs.value.currentPlayerIndex];
    let playerUtility = new PlayerUtility()

    switch(index){
      case 0:
        if(!playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,player) 
          && !playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)
          || this.allWarehousesFull()) return true;
        return false;
      case 1:
        if(playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,player)) return true;
        return false;
      case 2:
      case 3:
        if(playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)) return true;
        return false;
      default:
        return true;
    }
  }

  allWarehousesFull():boolean{
    let player = this.gs.value.players[this.gs.value.currentPlayerIndex];
    let playerUtility = new PlayerUtility()
    
    let warehouseSlots = 0;
    let storedTypes = 0;
    if(playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,player)) warehouseSlots += 1;
    if(playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)) warehouseSlots += 2;

    this.storedGoodTypes.forEach(goodType => {
      if(goodType != 6) storedTypes++;
    });

    return storedTypes == warehouseSlots;
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
    myBuildings.sort((a,b)=> ( a.buildOrder - b.buildOrder));
    let buildingsMatrix = this.initMatrix();
    let occupiedBuildingSpaces:number[] = [0,0,0,0];

    for(let i=0; i< myBuildings.length; i++){
      if(Math.floor(i/4) == 0 && occupiedBuildingSpaces[i%4] + this.getBuildingType(myBuildings[i])!.size <= 3)
      {
        buildingsMatrix[i%4].push(myBuildings[i]);
        occupiedBuildingSpaces[i%4] += this.getBuildingType(myBuildings[i])!.size;
      }
      else if(occupiedBuildingSpaces[Math.floor((i-4)/2)] + this.getBuildingType(myBuildings[i])!.size <= 3)
      {
        buildingsMatrix[Math.floor((i-4)/2)].push(myBuildings[i]);
        occupiedBuildingSpaces[Math.floor((i-4)/2)] += this.getBuildingType(myBuildings[i])!.size;
      }
      else
      {
        buildingsMatrix[Math.floor(((i-4)/2)+1)].push(myBuildings[i]);
        occupiedBuildingSpaces[Math.floor(((i-4)/2)+1)] += this.getBuildingType(myBuildings[i])!.size;
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

