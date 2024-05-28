
import { Injectable, OnInit } from '@angular/core';
 import { BehaviorSubject } from 'rxjs';
 import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment.development';
import { StartGameOutput, GameStateJson, BuildingType, GoodType, DataBuilding, DataPlayerBuilding, DataPlantation, DataPlayerPlantation, DataPlayerGood, ColorName, BuildingName, PlayerUtility, RoleName, GameStartInput, DataPlayer } from '../classes/general';
import { GameStartHttpService } from './game-start-http.service';
import { ScrollService } from './scroll.service';

@Injectable({
  providedIn: 'root'
})
export class GameService{
  
  debugOptions:boolean = true;
  isHotSeat:boolean = true;
  startGameOutput!:StartGameOutput;
  errorToUI:string = 'error to ui';

  playerIndex:number = 0;
  gs = new BehaviorSubject<GameStateJson>(new GameStateJson()); 
  buildingTypes: BuildingType[] = [];
  goodTypes: GoodType[] = [];

  

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
    this.startGameInput.gameId = 33;
    this.startGameInput.numOfPlayers = 4;
    this.startGameInput.playerIndex = 0;
    this.startGameInput.isDraft = false;
    this.startGameInput.isBuildingsExpansion = true;
    this.startGameInput.isNoblesExpansion = false;

    this.gameStartHttp.postNewGame(this.startGameInput)
    .subscribe({
      next:(startGameOutput:StartGameOutput) => {
      this.buildingTypes = startGameOutput.buildingTypes;
      this.startGameOutput = startGameOutput;
      this.gs.next(startGameOutput.gameState);
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

    let forest = new GoodType();
    forest.good = 7;
    forest.color = ColorName.green;
    forest.displayName = "forest";
   

    this.goodTypes.push(corn,indigo,sugar,tobacco,coffee,quarry,upSideDown,forest);
  }

  calculateBuildingPoints(player:DataPlayer):number{
    let points = 0;

    player.buildings.forEach(building => {
      let buildingPoints = this.getBuildingType(building)?.victoryScore;
      if(buildingPoints != null) points += buildingPoints;
    });

    return points;
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

    myBuildings.forEach(building => {
      let buildingSize = this.getBuildingType(building)?.size
      if(buildingSize == null) return;

      let occupiedSum = occupiedBuildingSpaces[0] > 0   //is first line occupied
      && occupiedBuildingSpaces[1] > 0 
      && occupiedBuildingSpaces[2] > 0 
      && occupiedBuildingSpaces[3] > 0 ;

      if(buildingSize == 1 && !occupiedSum){    //first fill top line if possible
        for(let i=0; i<4; i++){                    //unless someone builds a large building revert to natural sorting
          if(occupiedBuildingSpaces[i] == 0){
          buildingsMatrix[i].push(building);
          occupiedBuildingSpaces[i] += buildingSize;
          return;
          }
        }
      } else{        //then fill in the rest
      for(let i=0; i<occupiedBuildingSpaces.length; i++){
        while(buildingSize + occupiedBuildingSpaces[i] > 3){
          i++ 
        }
        buildingsMatrix[i].push(building);
        occupiedBuildingSpaces[i] += buildingSize;
        return;
      }
    }});

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

