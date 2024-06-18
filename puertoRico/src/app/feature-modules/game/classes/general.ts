import { User } from "../../user/models/user";

export class GameStateJson{
    id:number = 0;
    isDraft:boolean = false;
    isBuildingsExpansion:boolean = false;
    isNoblesExpansion:boolean = false;
    isRoleInProgress:boolean = false;
    currentPlayerIndex:number = 0;
    privilegeIndex:number = 0;
    governorIndex:number = 0;
    victoryPointSupply:number =0;
    colonistsSupply:number = 0;
    colonistsOnShip:number = 0;
    noblesSupply:number = 0;
    noblesOnShip:number = 0;
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
    guestHouseNextRole:RoleName = RoleName.NoRole;
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
    state:SlotEnum = SlotEnum.Vacant;
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
    nobles:number = 0;
    victoryPoints:number = 0;
    tookTurn:boolean = false;  
    buildings:DataPlayerBuilding[] = [];
    plantations:DataPlayerPlantation[] = [];
    buildOrder:number = 0;
    goods:DataPlayerGood[] = [];
    score:number = 0;
  }

  export class PlayerUtility{
    hasActiveBuilding(name:BuildingName,player:DataPlayer):boolean{

      if(player == undefined) return false;
      let temp = player.buildings.find(building => building.name == name)
      if(temp == undefined) return false;
      else return temp.slots[0].state != SlotEnum.Vacant;
    }
  
    getBuilding(name:BuildingName,player:DataPlayer):DataPlayerBuilding|undefined{
      return player.buildings.find(building => building.name == name)
    }
  }
  
  export class DataBuilding {
    id:number = 0;
    gameStateId:number = 0;
    name:number = 0;
    slots:DataSlot[] = [];
    quantity:number = 0;
    effectAvailable:boolean = false;
    isDrafted:boolean = false;
    isBlocked:boolean = false;
  }
  
  export class DataPlayerBuilding {
    id:number = 0;
    dataPlayerId:number = 0;
    name:number = 0;
    slots:DataSlot[] = [];
    quantity:number = 0;
    buildOrder:number = 0;
    effectAvailable:boolean = false;
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
    expansion:number = 0;
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

  export class GameStartInput{
    gameId:number = 0;
    numOfPlayers:number = 0;
    playerIndex:number = 0;
    isDraft:boolean = false;
    isBuildingsExpansion:boolean = false;
    isNoblesExpansion:boolean = false;
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
      red,
      gray,
      green,
      zeroQuantBuilding
  }
  
  export enum BuildingName
  {
      SmallIndigoPlant,
      SmallSugarMill,
      SmallMarket,
      Aqueduct,//1
      Hacienda,
      ConstructionHut,
      ForestHouse,//1
      BlackMarket,//1
      SmallWarehouse, 
      Storehouse,//1
      LandOffice,//2
      Chapel,//2
      LargeIndigoPlant,
      LargeSugarMill,
      Hospice,
      GuestHouse,//1
      Office,
      LargeMarket,
      TradingPost,//1
      Church,//1
      LargeWarehouse,
      SmallWharf,//1
      HuntingLodge,//2
      ZoningOffice,//2
      RoyalSupplier,//2
      TobaccoStorage,
      CoffeeRoaster,
      Univercity,
      Lighthouse,//1
      Factory,
      Harbor,
      SpecialtyFactory,//1
      Library,//1
      Wharf,
      UnionHall,//1
      Villa,//2
      Jeweler,//2
      GuildHall,
      Residence,
      Fortress,
      CustomsHouse,
      CityHall,
      Statue,//1
      Cloister,//1
      RoyalGarden,//2
  }

  export enum isAffordable
  {
      Not,
      WithBlackMarket,
      Yes,
      BlackMarketBlocked,
  }
  
  export enum RoleName
  {
      Settler,
      Builder,
      Mayor,
      Trader,
      Craftsman,
      Captain,
      Prospector,
      PostCaptain,
      NoRole,
      Draft,
      GuestHouse,
  }

  export enum GoodName
  {
      Corn,
      Indigo,
      Sugar,
      Tobacco,
      Coffee,
      Quarry,
      NoType,
      Forest,
  }

  export enum SlotEnum{
    Vacant,
    Colonist,
    Noble,
  }