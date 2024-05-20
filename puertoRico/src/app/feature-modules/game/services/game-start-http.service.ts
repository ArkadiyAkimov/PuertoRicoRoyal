import { User } from './../../user/models/user';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameStartHttpService {
  private url="Game" ;

  constructor(private http: HttpClient) { }
  
  public postNewGame(gameId: number, numOfPlayers: number, playerIndex:number): Observable<StartGameOutput> {
    console.log(`post new game ${environment.apiUrl}/${this.url} numOfPlayers: ${numOfPlayers}`);
    return this.http.post<StartGameOutput>(`${environment.apiUrl}/Game`,{ gameId, numOfPlayers,playerIndex});
  } 
}

export class GameStateJson{
  id:number = 0;
  isRoleInProgress:boolean = false;
  currentPlayerIndex:number = 0;
  privilegeIndex:number = 0;
  governorIndex:number = 0;
  victoryPointSupply:number =0;
  colonistsSupply:number = 0;
  colonistsOnShip:number = 0;
  quarryCount:number = 0;
  cornSupply:number = 0;
  indigoSupply:number = 0;
  sugarSupply:number = 0;
  tobaccoSupply:number = 0;
  coffeeSupply:number = 0;
  currentRole:number = 0;
  roles:DataRole[] = [];
  users:User[] = [];
  players:DataPlayer[] = [];
  buildings:DataBuilding[] = [];
  plantations:DataPlantation[] = [];
  ships:DataShip[] =[];
  tradeHouse:DataTradeHouse = new DataTradeHouse();
  captainPlayableIndexes:string ='';
  captainFirstShipment:boolean = true;
  mayorTookPrivilige:boolean = false;
  lastGovernor:boolean = false;
  gameOver:boolean = false;
}

export class DataShip{
  id:number = 0;
  GameStateId:number = 0;
  capacity:number = 0;
  load:number =0;
  type:number = 0;
}

export class DataSlot {
  id:number = 0;
  isOccupied:boolean = false;
}

export class DataRole {
  id:number = 0;
  gameStateId:number = 0;
  name:number = 0;
  isPlayable:boolean = true;
  bounty:number = 0;
  isFirstIteration:boolean = true;
}

export class DataPlayerGood{
  id:number = 0;
  dataPlayerId: number = 0;
  quantity: number = 0;
  type: number =0;
}

export class DataPlayer {
  id:number = 0;
  dataGameStateId:number = 0;
  index:number = 0;
  doubloons:number = 0;
  colonists:number = 0;
  victoryPoints:number = 0;
  canUseHacienda:boolean = false;
  canUseHospice:boolean = false;
  hospiceTargetPlantation:number = 0;
  canUseWharf:boolean = true;
  canUseSmallWarehouse:boolean = false;
  canUseLargeWarehouse:boolean = false;
  buildings:DataPlayerBuilding[] = [];
  plantations:DataPlayerPlantation[] = [];
  buildOrder:number = 0;
  goods:DataPlayerGood[] = [];
  score:number = 0;
}

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
  buildOrder:number = 0;
}

export class DataTradeHouse {
  id:number = 0;
  dataGameStateId:number =0;
  goods:string = "";
}

export class StartGameOutput{
  gameState:GameStateJson = new GameStateJson();
  buildingTypes:BuildingType[] = [];
}

export class GoodType {
  good:number = 0;
  displayName:string = '';
  color:ColorName = 0;
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
