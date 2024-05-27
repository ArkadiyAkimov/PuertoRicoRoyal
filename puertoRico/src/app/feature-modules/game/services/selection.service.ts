import { Injectable } from '@angular/core';
import { GameService } from './game.service';
import { BuildingName, DataPlayer, DataPlayerBuilding, DataPlayerGood, GoodName, GoodType, PlayerUtility, RoleName } from '../classes/general';

@Injectable({
  providedIn: 'root'
})
export class SelectionService {
  playerUtility:PlayerUtility = new PlayerUtility()
  selectedShip: number = 4;
  windroseStoredGood:GoodName = GoodName.NoType;
  storeHouseStoredGoods:GoodName[] = [
    GoodName.NoType,
    GoodName.NoType,
    GoodName.NoType,
  ];
  smallWarehouseStoredType:GoodName = GoodName.NoType;
  smallWarehouseStoredQuantity:number = 0;
  largeWarehouseStoredTypes:GoodName[] = [
    GoodName.NoType,
    GoodName.NoType,
  ]
  largeWarehouseStoredQuantities:number[] = [0,0];

  isBlackMarketActive: boolean = false;
  sellColonist: boolean = false;
  selectedSlotId: number = 0;
  sellGood:boolean = false;
  selectedGoodType:GoodName = GoodName.NoType;
  sellVictoryPoint:boolean = false;

  finishedInitialStorage:boolean = false;
  targetTypeStorageIndex = 0;
  takingForest: boolean = false;

  constructor(private gameService:GameService){ 
  }

  toggleBlackMarket(){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if( gs.currentPlayerIndex != player.index || gs.currentRole != RoleName.Builder) return; //check black market activated

    if(this.isBlackMarketActive){
      this.isBlackMarketActive = false;
      this.sellColonist = false;
      this.selectedSlotId = 0;
      this.sellGood = false;
      this.selectedGoodType = GoodName.NoType;
      this.sellVictoryPoint = false;
    }
    else this.isBlackMarketActive = true;
  }

  toggleSupplyColonistForBlackMarket(){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(!this.isBlackMarketActive || gs.currentPlayerIndex != player.index) return; //check black market activated

    if(player.colonists <= 0) return;  //can select supply colonist

    if(this.selectedSlotId != 0){  //remove previous selection
      this.selectedSlotId = 0;
      this.sellColonist = true
    }else {
      if(this.sellColonist){        //toggle on
        this.sellColonist = false;
      }
      else this.sellColonist = true; //toggle off
    }
  }

  selectColonistForBlackMarket(slotId:number){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];


    if(!this.isBlackMarketActive || gs.currentPlayerIndex != player.index) return; //check black market activated

    if(slotId == this.selectedSlotId){ //if selecting same slot cancel colonist selection
      this.sellColonist = false;
      this.selectedSlotId = 0;
    } 
    else{                //check slot is not empty
      let slotIsEmpty = true;
      player.plantations.forEach(plantation => {
        if(plantation.slot.id == slotId && plantation.slot.isOccupied){
          slotIsEmpty = false;
        }
        if(slotIsEmpty) return;
      });
      player.buildings.forEach(building => {
        building.slots.forEach(slot => {
          if(slot.id == slotId && slot.isOccupied && building.name != BuildingName.BlackMarket){
            slotIsEmpty = false;
          }
          if(slotIsEmpty) return;
        });
      });

    if(slotIsEmpty) return;
    this.selectedSlotId = slotId;   //select slot for colonist sell
    this.sellColonist = true;
    }
  }

  selectGoodForBlackMarket(good:GoodName){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];
    
    if(!this.isBlackMarketActive || gs.currentPlayerIndex != player.index) return;


    if(this.selectedGoodType == good){ //if selecting same good cancel good selection
      this.sellGood = false;
      this.selectedGoodType = GoodName.NoType;
    }
    else if(player.goods[good].quantity > 0){
    this.selectedGoodType = good;   //select good for good sell
    this.sellGood = true;
    }
  }

  toggleVictoryPointSellForBlackMarket(){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    
    if(!this.isBlackMarketActive || gs.currentPlayerIndex != player.index) return;
    if(player.victoryPoints <= 0) return;

    this.sellVictoryPoint = !this.sellVictoryPoint; //toggle victory point sale
  }

  selectGoodToStore(good:DataPlayerGood){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(good.quantity <= 0 || gs.currentRole != RoleName.PostCaptain || gs.currentPlayerIndex != player.index) return;

    if(this.canEndTurnPostCptain()){
      this.resetGoodSelection();
      return;
    } 

    if(this.playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)
      && this.largeWarehouseStoredTypes.includes(GoodName.NoType)
      && !this.largeWarehouseStoredTypes.includes(good.type)){
      if(this.largeWarehouseStoredTypes[0] == GoodName.NoType){
        this.largeWarehouseStoredTypes[0] = good.type;
        this.largeWarehouseStoredQuantities[0] = good.quantity;
        player.goods[good.type].quantity = 0;
      }
      else{
        this.largeWarehouseStoredTypes[1] = good.type;
        this.largeWarehouseStoredQuantities[1] = good.quantity;
        player.goods[good.type].quantity = 0;
      }
    }
    else if(this.playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,player)
      && this.smallWarehouseStoredType == GoodName.NoType
      && !this.largeWarehouseStoredTypes.includes(good.type)){
          this.smallWarehouseStoredType = good.type;
          this.smallWarehouseStoredQuantity = good.quantity;
          player.goods[good.type].quantity = 0;
    }
    else if(this.playerUtility.hasActiveBuilding(BuildingName.Storehouse,player)
     && this.storeHouseStoredGoods.includes(GoodName.NoType)){
      if(this.storeHouseStoredGoods[0] == GoodName.NoType){
        this.storeHouseStoredGoods[0] = good.type;
        player.goods[good.type].quantity--;
      }
        else if(this.storeHouseStoredGoods[1] == GoodName.NoType){
          this.storeHouseStoredGoods[1] = good.type;
          player.goods[good.type].quantity--;
        }
          else if(this.storeHouseStoredGoods[2] == GoodName.NoType){
            this.storeHouseStoredGoods[2] = good.type;
            player.goods[good.type].quantity--;
          }
    }
    else if(this.windroseStoredGood == GoodName.NoType){
            this.windroseStoredGood = good.type;
            player.goods[good.type].quantity--;
    }
  }


  resetGoodSelection(){
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];

    if(this.playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)){
      for(let i= 0; i< 2; i++){
        if(this.largeWarehouseStoredTypes[i] != GoodName.NoType){
          player.goods[this.largeWarehouseStoredTypes[i]].quantity = this.largeWarehouseStoredQuantities[i];
          this.largeWarehouseStoredQuantities[i] = 0;
          this.largeWarehouseStoredTypes[i] = GoodName.NoType;
        }
      }
    }
    if(this.playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,player)){ 
        if(this.smallWarehouseStoredType != GoodName.NoType){
          player.goods[this.smallWarehouseStoredType].quantity = this.smallWarehouseStoredQuantity;
          this.smallWarehouseStoredQuantity= 0;
          this.smallWarehouseStoredType = GoodName.NoType;
        }
    }
    if(this.playerUtility.hasActiveBuilding(BuildingName.Storehouse,player)){
      for(let i= 0; i< 3; i++){
        if(this.storeHouseStoredGoods[i] != GoodName.NoType){
          player.goods[this.storeHouseStoredGoods[i]].quantity += 1;
          this.storeHouseStoredGoods[i] = GoodName.NoType;
        }
      }
    }
    if(this.windroseStoredGood != GoodName.NoType){
      player.goods[this.windroseStoredGood].quantity += 1;
      this.windroseStoredGood = GoodName.NoType;
    }
  }

  canEndTurnPostCptain():boolean{
    let gs = this.gameService.gs.value;
    let player = gs.players[gs.currentPlayerIndex];
    let playerTotalGoods = 0;

    player.goods.forEach(good => {
      playerTotalGoods += good.quantity;
    });

    if(playerTotalGoods == 0) return true;

    if(this.playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)){
      if(this.largeWarehouseStoredTypes.includes(GoodName.NoType)) return false;
    }
    if(this.playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,player)){
      if(this.smallWarehouseStoredType == GoodName.NoType) return false;
    }
    
    if(this.playerUtility.hasActiveBuilding(BuildingName.Storehouse,player)){
      if(this.storeHouseStoredGoods.includes(GoodName.NoType)) return false;
    }

    if(this.windroseStoredGood == GoodName.NoType) return false;
    
    return true;
}

toggleBuildingEffect(building:DataPlayerBuilding){

  if(!building.slots[0].isOccupied) return;

  let buildingType = this.gameService.getBuildingType(building);


  switch(buildingType?.name){
    case BuildingName.ForestHouse:
      this.takingForest = !this.takingForest;
      break;
    case BuildingName.BlackMarket:
        this.toggleBlackMarket();
      break;    
  }
}



}
