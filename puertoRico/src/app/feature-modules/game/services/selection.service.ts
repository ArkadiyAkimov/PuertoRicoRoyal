import { Injectable } from '@angular/core';
import { GameService } from './game.service';
import { BuildingName, DataBuilding, DataPlayerBuilding, DataPlayerGood, GameStateJson, GoodName, GoodType, PlayerUtility, RoleName, SlotEnum, isAffordable } from '../classes/general';

@Injectable({
  providedIn: 'root'
})
export class SelectionService {
  playerUtility:PlayerUtility = new PlayerUtility()
  
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

  selectedShip: number = 6;
  goodsOnWharf: GoodName[] = [];
  selectedGoodsSmallWharf: GoodName[] = [];
  selectedGoodsRoyalSupplier: GoodName[] = [];

  takingForest: boolean = false;
  sellingToTradingPost:boolean = false;

  noblesSelected: boolean = false;

  isLandOfficeActive: boolean = false;
  isHuntingLodgeActive: boolean = false;

  constructor(private gameService:GameService){ 
    this.gameService.gs.subscribe({
      next: (gs:GameStateJson) => {
        if(gs.players.length <= 0) return;
        this.selectionCleanUp();
      }
    })
  }


  toggleBlackMarket(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

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

  selectionCleanUp(){
    this.windroseStoredGood = GoodName.NoType;
    this.storeHouseStoredGoods = [
    GoodName.NoType,
    GoodName.NoType,
    GoodName.NoType,
    ];
    this.smallWarehouseStoredType = GoodName.NoType;
    this.smallWarehouseStoredQuantity = 0;
    this.largeWarehouseStoredTypes = [
      GoodName.NoType,
      GoodName.NoType,
    ]
    this.largeWarehouseStoredQuantities = [0,0];

    this.isBlackMarketActive = false;
    this.sellColonist = false;
    this.selectedSlotId = 0;
    this.sellGood = false;
    this.selectedGoodType = GoodName.NoType;
    this.sellVictoryPoint = false;

    this.selectedShip = 6;
    this.goodsOnWharf = [];
    this.selectedGoodsSmallWharf = [];
    this.selectedGoodsRoyalSupplier = [];

    this.takingForest = false;

    this.sellingToTradingPost = false;
    
    this.noblesSelected = false;
    
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player.colonists == 0 && player.nobles > 0) this.noblesSelected = true;  

    this.isLandOfficeActive = false;
    this.isHuntingLodgeActive = false;
  }


  selectNobles(){
    if(this.gameService.gs.value.isNoblesExpansion) this.noblesSelected = true;
  }

  selectColonists(){
    this.noblesSelected = false;
  }

  toggleSmallWharf(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(this.selectedShip == 4){
      this.selectedShip = 6;
      this.selectedGoodsSmallWharf.forEach(goodType => {
        player.goods[goodType].quantity++;
      });
      this.selectedGoodsSmallWharf = [];
    }
    else this.selectedShip = 4;
  }

  toggleRoyalSupplier(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(this.selectedShip == 5){
      this.selectedShip = 6;
      this.selectedGoodsRoyalSupplier.forEach(goodType => {
        player.goods[goodType].quantity++;
      });
      this.selectedGoodsRoyalSupplier = [];
    }
    else this.selectedShip = 5;
  }

  fillWharf(good:DataPlayerGood){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    for(let i =0; i<good.quantity;i++){
      this.goodsOnWharf.push(good.type);
    }
    
    good.quantity = 0;
  }

  toggleLandOffice(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(this.isLandOfficeActive){
      this.isLandOfficeActive = false;
      this.takingForest = false;
    } 
    else if(
        gs.currentRole == RoleName.Trader 
        && this.playerUtility.hasActiveBuilding(BuildingName.LandOffice,player)
        && this.playerUtility.getBuilding(BuildingName.LandOffice, player)?.effectAvailable
      ){
        this.isLandOfficeActive = true;
      }
  }

  toggleHuntingLodge(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(this.isHuntingLodgeActive){
      this.isHuntingLodgeActive = false;
    } 
    else if(
        gs.currentRole == RoleName.Settler 
        && this.playerUtility.hasActiveBuilding(BuildingName.HuntingLodge,player)
        && this.playerUtility.getBuilding(BuildingName.HuntingLodge, player)?.effectAvailable
        && this.playerUtility.getBuilding(BuildingName.HuntingLodge,player)?.slots[0].state == SlotEnum.Colonist
      ){
        this.isHuntingLodgeActive = true;
        this.takingForest = false;
      }
  }

  toggleForestHouse(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(this.takingForest){
      this.takingForest = false;
    } else{
      this.takingForest = true;
      this.isHuntingLodgeActive = false;
    }
  }

  getSelectedBlackMarketDiscountValue():number{
    let discount = 0;

    if(!this.isBlackMarketActive) return 0;
    if(this.sellColonist) discount++;
    if(this.sellGood) discount++;
    if(this.sellVictoryPoint) discount++

    return discount;
  }


  selectSmallWharfGoods(goodType:GoodName){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(player.goods[goodType].quantity > 0){
      player.goods[goodType].quantity--;
      this.selectedGoodsSmallWharf.push(goodType);
    }
  }
  
  selectRoyalSupplierGoods(goodType:GoodName){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(player.goods[goodType].quantity > 0
      && !this.selectedGoodsRoyalSupplier.includes(goodType)
      && this.selectedGoodsRoyalSupplier.length+1 <= this.gameService.countPlayerNobles()){
      player.goods[goodType].quantity--;
      this.selectedGoodsRoyalSupplier.push(goodType);
    }
  }


  toggleSupplyColonistForBlackMarket(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

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
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;


    if(!this.isBlackMarketActive || gs.currentPlayerIndex != player.index) return; //check black market activated

    if(slotId == this.selectedSlotId){ //if selecting same slot cancel colonist selection
      this.sellColonist = false;
      this.selectedSlotId = 0;
    } 
    else{                //check slot is not empty
      let slotIsEmpty = true;
      player.plantations.forEach(plantation => {
        if(plantation.slot.id == slotId && (plantation.slot.state != SlotEnum.Vacant)){
          slotIsEmpty = false;
        }
        if(slotIsEmpty) return;
      });
      player.buildings.forEach(building => {
        building.slots.forEach(slot => {
          if(slot.id == slotId && (slot.state != SlotEnum.Vacant)){
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
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;
    
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
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    
    if(!this.isBlackMarketActive || gs.currentPlayerIndex != player.index) return;
    if(player.victoryPoints <= 0) return;

    this.sellVictoryPoint = !this.sellVictoryPoint; //toggle victory point sale
  }

  selectGoodToStore(good:DataPlayerGood){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];

    if(good.quantity <= 0 
      || gs.currentRole != RoleName.PostCaptain 
      || gs.currentPlayerIndex != player.index) return;

    if(this.canEndTurnPostCptain() || good.type == this.windroseStoredGood){
      this.resetGoodSelection();
      return;
    } 

    if(this.playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,player)
      && this.largeWarehouseStoredTypes.includes(GoodName.NoType)
      && !this.largeWarehouseStoredTypes.includes(good.type)
      && this.smallWarehouseStoredType != good.type){
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
     && this.storeHouseStoredGoods.includes(GoodName.NoType)
     && this.smallWarehouseStoredType != good.type
     && !this.largeWarehouseStoredTypes.includes(good.type)){
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
    else if(this.windroseStoredGood == GoodName.NoType
            && this.smallWarehouseStoredType != good.type
             && !this.largeWarehouseStoredTypes.includes(good.type))
            {
            this.windroseStoredGood = good.type;
            player.goods[good.type].quantity--;
            }
  }

  getStoredGoodQuantity(good:DataPlayerGood):number{
    if(this.largeWarehouseStoredTypes.includes(good.type)) return this.largeWarehouseStoredQuantities[this.largeWarehouseStoredTypes.indexOf(good.type)];
    if(this.smallWarehouseStoredType == good.type) return this.smallWarehouseStoredQuantity;
    
    let quantity = 0;
    for(let i=0; i<3; i++) if(this.storeHouseStoredGoods[i] == good.type) quantity++;
    if(this.windroseStoredGood == good.type) quantity++;
    return quantity;
  }

  getSmallWharfGoodQuantity(good:DataPlayerGood):number{
    if(this.selectedGoodsSmallWharf.length <= 0) return 0;

    let goodsOfTypeOnSmallWharf = this.selectedGoodsSmallWharf.filter(x => x == good.type);
    return goodsOfTypeOnSmallWharf.length;
  }

  resetGoodSelection(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;
  
    if(gs.currentPlayerIndex != player.index) return;

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
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return false;
    let playerTotalGoods = 0;
  
    if(gs.currentPlayerIndex != player.index) return false;

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
  let gs = this.gameService.gs.value;
  let player = gs.players[this.gameService.playerIndex];
  if(player == undefined) return;

  if(gs.currentPlayerIndex != player.index) return;
  if(building.slots[0].state == SlotEnum.Vacant) return;

  let buildingType = this.gameService.getBuildingType(building);


  switch(buildingType?.name){
    case BuildingName.ForestHouse:
      if(gs.currentRole == RoleName.Settler) this.toggleForestHouse();
      else if(gs.currentRole == RoleName.Trader && this.isLandOfficeActive) this.takingForest = !this.takingForest;
      break;
    case BuildingName.BlackMarket:
        if(gs.currentRole == RoleName.Builder)this.toggleBlackMarket();
      break;    
    case BuildingName.TradingPost:
      if(gs.currentRole == RoleName.Trader) this.sellingToTradingPost = !this.sellingToTradingPost;
      break;
    case BuildingName.LandOffice:
      if(gs.currentRole == RoleName.Trader) this.toggleLandOffice();
      break;
    case BuildingName.HuntingLodge:
      if(gs.currentRole == RoleName.Settler) this.toggleHuntingLodge();
      break;
  }
}

getGoodBubbleCount(good:DataPlayerGood):number{
  let gs = this.gameService.gs.value;
  let player = gs.players[this.gameService.playerIndex];
  if(player == undefined) return 0;

  if(gs.currentPlayerIndex != player.index) return 0;

  switch(gs.currentRole){
    case RoleName.PostCaptain:
        return this.getStoredGoodQuantity(good);
    case RoleName.Captain:
        return this.getSmallWharfGoodQuantity(good);
  }

  return 0;
}

getVictoryPointBubbleCount():number{
  let gs = this.gameService.gs.value;
  let player = gs.players[this.gameService.playerIndex];
  if(player == undefined) return 0;

  let victoryPoints = 0;

  if(gs.currentPlayerIndex != player.index) return 0;
  switch(gs.currentRole){
    case RoleName.Captain:
        victoryPoints += Math.floor(this.selectedGoodsSmallWharf.length/2); 
        if(this.playerUtility.hasActiveBuilding(BuildingName.Harbor, player) && (this.selectedGoodsSmallWharf.length)) victoryPoints++;
        break;
  }

  

  return victoryPoints;
}

getSmallWharfGoodTypeArray():any{
  let array: any[] = [];

  this.selectedGoodsSmallWharf.forEach(GoodType => {
    array.push(this.gameService.goodTypes[GoodType]);
  });

  return array;
}

getWharfGoodTypeArray():any{
  let array: any[] = [];

  this.goodsOnWharf.forEach(GoodType => {
    array.push(this.gameService.goodTypes[GoodType]);
  });

  return array;
}

getRoyalSupplierGoodTypeArray():any{
  let array: any[] = [];

  this.selectedGoodsRoyalSupplier.forEach(GoodType => {
    array.push(this.gameService.goodTypes[GoodType]);
  });

  return array;
}

checkPlayerBuildingAffordabilityState(building:DataBuilding):isAffordable{
  let player = this.gameService.gs.value.players[this.gameService.playerIndex];
  let gs = this.gameService.gs.value;
  let type = this.gameService.getBuildingType(building);
  if(type == null || type == undefined) return isAffordable.Not;

  let playerDoubloons = player.doubloons;
  if((player.index == gs.privilegeIndex) 
    && (gs.currentPlayerIndex == player.index)
    && (gs.currentRole == RoleName.Builder)){
      playerDoubloons++; //privilege during builder
      if(this.playerUtility.hasActiveBuilding(BuildingName.Library,player)) playerDoubloons++;
    }
  let quarryMaxDiscount = 0;
  let forestFinalDiscount = 0;

  player.plantations.forEach(plantation => {
    if(plantation.good == GoodName.Quarry && (plantation.slot.state != SlotEnum.Vacant)) quarryMaxDiscount++;
  });

  player.plantations.forEach(plantation => {
    if(plantation.good == GoodName.Forest) forestFinalDiscount++;
  });

  forestFinalDiscount = Math.floor(forestFinalDiscount/2);
  let quarryFinalDiscount = Math.min(type.victoryScore, quarryMaxDiscount);
  
  let blackMarketMaxDiscount = 0;

  if(this.playerUtility.hasActiveBuilding(BuildingName.BlackMarket, player)){
    let sumOfPlayerGoods = 0;
    player.goods.forEach(good => {
      sumOfPlayerGoods += good.quantity;
    });

    if(sumOfPlayerGoods > 0) blackMarketMaxDiscount++;
    if(player.victoryPoints > 0) blackMarketMaxDiscount++;
    blackMarketMaxDiscount++ //has a colonist on black market dont need to calculate shit.
  }

  let zoningOfficeDiscount = 0;

  if(this.playerUtility.hasActiveBuilding(BuildingName.ZoningOffice, player)){
    if((this.playerUtility.getBuilding(BuildingName.ZoningOffice,player)?.slots[0].state == SlotEnum.Colonist) && (type.victoryScore < 4)) zoningOfficeDiscount = 1;
    if((this.playerUtility.getBuilding(BuildingName.ZoningOffice,player)?.slots[0].state == SlotEnum.Noble) && (type.victoryScore <= 4)) zoningOfficeDiscount = 2;
  }

  let blackMarketCurrentDiscount = this.getSelectedBlackMarketDiscountValue();

  let totalBudget = playerDoubloons + quarryFinalDiscount + forestFinalDiscount + zoningOfficeDiscount;

  if(this.isBlackMarketActive){
    if(type.price <= totalBudget) return isAffordable.BlackMarketBlocked;
    else if (type.price == totalBudget + blackMarketCurrentDiscount) return isAffordable.Yes;
    else if (type.price <= totalBudget + blackMarketMaxDiscount) return isAffordable.WithBlackMarket;
    else return isAffordable.Not;
  }else{
  if(type.price <= totalBudget) return isAffordable.Yes;
  else if (type.price <= totalBudget + blackMarketMaxDiscount) return isAffordable.WithBlackMarket;
  else return isAffordable.Not;
  }
}

}
