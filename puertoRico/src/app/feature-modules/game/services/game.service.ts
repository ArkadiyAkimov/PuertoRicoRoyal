
import { Injectable } from '@angular/core';
 import { BehaviorSubject } from 'rxjs';
 import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment.development';
import { StartGameOutput, GameStateJson, BuildingType, GoodType, DataBuilding, DataPlayerBuilding, DataPlantation, DataPlayerPlantation, DataPlayerGood, ColorName, BuildingName, PlayerUtility, RoleName, GameStartInput, DataPlayer, isAffordable, GoodName, SlotEnum } from '../classes/general';
import { GameStartHttpService } from './game-start-http.service';
import { ScrollService } from './scroll.service';

@Injectable({
  providedIn: 'root'
})
export class GameService{
  
  playerUtility:PlayerUtility = new PlayerUtility();
  debugOptions:boolean = true;
  isHotSeat:boolean = true;
  startGameOutput!:StartGameOutput;
  errorToUI:string = 'error to ui';


  playerIndex:number = 0;
  gs = new BehaviorSubject<GameStateJson>(new GameStateJson()); 
  buildingTypes: BuildingType[] = [];
  goodTypes: GoodType[] = [];
  rawAndfinalProductionArrays = new BehaviorSubject<number[][][]>([]);
  supplyGoods:Number[] = [0,0,0,0,0];

  private hubConnection: HubConnection;

  startGameInput:GameStartInput = new GameStartInput()

  constructor(private gameStartHttp:GameStartHttpService, private scrollService:ScrollService){ 
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(`${environment.apiUrl}/updatehub`)
    .build();

    this.initializeGoodTypes();
    this.initializeUpdateHub();
  }

  getSortedGoodButtons(goods:DataPlayerGood[]){
    return goods.sort((a,b) => a.type - b.type );
  }

  joinOrInitGame(){
    this.startGameInput.gameId = 132;
    this.startGameInput.numOfPlayers = 4;
    this.startGameInput.playerIndex = 0;
    this.startGameInput.isDraft = false;
    this.startGameInput.isBuildingsExpansion = true;
    this.startGameInput.isNoblesExpansion = true;
    

    this.gameStartHttp.postNewGame(this.startGameInput)
    .subscribe({
      next:(startGameOutput:StartGameOutput) => {
      this.buildingTypes = startGameOutput.buildingTypes;
      this.startGameOutput = startGameOutput;
      this.gs.next(startGameOutput.gameState);
      this.rawAndfinalProductionArrays.next(this.getRawAndFinalProductionArrays());
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
      this.rawAndfinalProductionArrays.next(this.getRawAndFinalProductionArrays());
    });
  }
  
  selectIndex() {
      this.hubConnection.invoke('SelectIndex', this.playerIndex);
  };

  countPlayerNobles(){
    let player = this.gs.value.players[this.playerIndex];
    let gs = this.gs.value;

    let noblesCount = player.nobles;

    player.plantations.forEach(plantation => {
      if(plantation.slot.state == SlotEnum.Noble) noblesCount++;
    });

    player.buildings.forEach(building => {
      building.slots.forEach(slot => {
        if(slot.state == SlotEnum.Noble) noblesCount++;
      });
    });

    return noblesCount;
  }


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

  getPlayerProductionArray(player:DataPlayer):number[]{
    let gs = this.gs.value;

    let plantsArray = [0,0,0,0,0];
    let buildsArray = [0,0,0,0,0];

    player.plantations.forEach(plant => {
      if(plant.slot.state != SlotEnum.Vacant) plantsArray[plant.good]++;
    });

    player.buildings.forEach(building =>{
      let buildingType = this.getBuildingType(building);

      if(buildingType?.isProduction){
        building.slots.forEach(slot => {
        if((slot.state != SlotEnum.Vacant) && buildingType != null){
          buildsArray[buildingType.good]++;
        } 
      });
    }});

    let resultArray = []

    resultArray.push(plantsArray[0]);
    for(let i=1; i<5; i++){
      resultArray.push(Math.min(plantsArray[i],buildsArray[i]));
    }

    if(this.playerUtility.hasActiveBuilding(BuildingName.Aqueduct,player)){
      if(resultArray[1] > 0 && this.playerUtility.hasActiveBuilding(BuildingName.LargeIndigoPlant,player)) resultArray[1]++;
      if(resultArray[2] > 0 && this.playerUtility.hasActiveBuilding(BuildingName.LargeSugarMill,player)) resultArray[2]++;
    }

    resultArray.push(0);

    
      if(this.playerUtility.hasActiveBuilding(BuildingName.Factory,player)){
        let typesCount = 0;
        resultArray.forEach(number => {
          if(number > 0) typesCount++;
        });
        if(typesCount != 5 && typesCount != 0) typesCount--;
        resultArray[5] += typesCount;
      }
      if(this.playerUtility.hasActiveBuilding(BuildingName.SpecialtyFactory,player)){
        let highestProduction = Math.max(resultArray[1],resultArray[2],resultArray[3],resultArray[4]);
        if(highestProduction != 0) highestProduction--;
        resultArray[5] += highestProduction;
      }

    return resultArray;
  }

  getRawAndFinalProductionArrays():number[][][]{
    let gs = this.gs.value;
    if(gs.roles[RoleName.Craftsman] == undefined) return [];

    let supplyGoods = [gs.cornSupply,gs.indigoSupply,gs.sugarSupply,gs.tobaccoSupply,gs.cornSupply];
    let finalSupplyArrays:number[][] = []
    let rawSupplyArray:number[][] = []

    gs.players.forEach(x => {
      finalSupplyArrays.push(this.getPlayerProductionArray(x))
      rawSupplyArray.push(this.getPlayerProductionArray(x))
    });

    let lastPrivilege = this.mod(gs.governorIndex - 1, gs.players.length);
    let nextGovernor = this.mod(gs.governorIndex + 1 , gs.players.length);
    let nextPrivilege = this.mod(gs.privilegeIndex + 1 , gs.players.length);
    
    let nextPotentialCraftsman = 0;

    // console.log("last privilege: "+ (lastPrivilege +1))
    // console.log("next governor: "+ (nextGovernor +1))
    // console.log("next privilege: "+ (nextPrivilege +1))
    
    if(gs.roles[RoleName.Craftsman].isPlayable && (gs.currentRole != RoleName.Craftsman)){
      if(gs.currentRole == RoleName.NoRole){
        nextPotentialCraftsman = gs.currentPlayerIndex;
      }
      else{
        nextPotentialCraftsman = nextPrivilege;
      }
    }else{
      nextPotentialCraftsman = nextGovernor;
    }

    // console.log("craftsman playable: " + gs.roles[RoleName.Craftsman].isPlayable)
    // console.log("current role: " + gs.currentRole)
    //  console.log("next craftsman: " + (nextPotentialCraftsman +1));

    finalSupplyArrays = this.reorderArray(finalSupplyArrays, nextPotentialCraftsman);

    finalSupplyArrays.forEach(demandGoods => {
      for(let i= 0; i<5 ; i++){
        if(supplyGoods[i] > 0){
          demandGoods[i] = Math.min(demandGoods[i], supplyGoods[i]);
          supplyGoods[i] -= demandGoods[i];
        } else demandGoods[i] = 0;
      }
    });

    finalSupplyArrays = this.restoreArray(finalSupplyArrays, nextPotentialCraftsman);
    
    for(let x=0; x<gs.players.length; x++){
      finalSupplyArrays[x][5] = 0;
      if(this.playerUtility.hasActiveBuilding(BuildingName.Factory,gs.players[x])){
        let typesCount = 0;
        finalSupplyArrays[x].forEach(number => {
          if(number > 0) typesCount++;
        });
        if(typesCount != 5 && typesCount != 0) typesCount--;
        finalSupplyArrays[x][5] += typesCount;
      }
      if(this.playerUtility.hasActiveBuilding(BuildingName.SpecialtyFactory,gs.players[x])){
        let highestProduction = Math.max(finalSupplyArrays[x][1],finalSupplyArrays[x][2],finalSupplyArrays[x][3],finalSupplyArrays[x][4]);
        if(highestProduction != 0) highestProduction--;
        finalSupplyArrays[x][5] += highestProduction;
      }
    }

    return [[...rawSupplyArray],[...finalSupplyArrays]];
  }
 
  initMatrix(){
    let buildingsMatrix: DataPlayerBuilding[][] = [];
    for(let i=0 ; i< 4; i++){
      let newBuildings:DataPlayerBuilding[] = [];
      buildingsMatrix.push(newBuildings);
    }

    return buildingsMatrix;
  }

  wharfDisplayCheck():boolean{
    let gs = this.gs.value;
    let player = gs.players[this.playerIndex];

    let temp = (BuildingName.Wharf) 
    && this.playerUtility.hasActiveBuilding(BuildingName.Wharf,player)
    && gs.currentRole == RoleName.Captain
    if(temp != undefined) return temp;
    else return false
  }
  
  smallWharfDisplayCheck():boolean{
    let gs = this.gs.value;
    let player = gs.players[this.playerIndex];

    let temp = (BuildingName.SmallWharf) 
    && this.playerUtility.hasActiveBuilding(BuildingName.SmallWharf,player)
    && gs.currentRole == RoleName.Captain
    if(temp != undefined) return temp;
    else return false
  }

  royalSupplierDisplayCheck():boolean{
    let gs = this.gs.value;
    let player = gs.players[this.playerIndex];

    if(this.countPlayerNobles() <= 0) return false;

    let temp = (BuildingName.RoyalSupplier) 
    && this.playerUtility.hasActiveBuilding(BuildingName.RoyalSupplier, player)
    && gs.currentRole == RoleName.Captain
    if(temp != undefined) return temp;
    else return false
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

  floor(number:number):number{
    return Math.floor(number);
  }

  mod(n:number,m:number)
  {
      return ((n % m) + m) % m;
  }

  reorderArray<T>(arr: T[], startIndex: number): T[] {
    // Make sure startIndex is within the bounds of the array
    if (startIndex < 0 || startIndex >= arr.length) {
        throw new Error("Start index out of bounds");
    }

    // Get the elements from startIndex to the end
    const part1 = arr.slice(startIndex);

    // Get the elements from the beginning up to startIndex
    const part2 = arr.slice(0, startIndex);

    // Concatenate the two parts in the desired order
    return [...part1, ...part2];
}

restoreArray<T>(arr: T[], startIndex: number): T[] {
  // Make sure startIndex is within the bounds of the array
  if (startIndex < 0 || startIndex >= arr.length) {
      throw new Error("Start index out of bounds");
  }

  // Get the elements from the startIndex to the end
  const part1 = arr.slice(0, arr.length - startIndex);

  // Get the elements from the beginning up to the startIndex
  const part2 = arr.slice(arr.length - startIndex);

  // Concatenate the two parts in the desired order
  return [...part2, ...part1];
}
}

