import { Produce } from "../goods/goodTypes";
import { DataBuilding } from '../deployables/building';

export class Player {
    constructor(name:string,index:number,doubloons:number){
        this.name = name;
        this.index = index;
        this.doubloons = doubloons;
        //this.layoutTest();
    }
    name:string;
    index:number;
    doubloons:number = 0;
    victoryPoints:number = 0;
    colonists:number = 0;
    nobles: number = 0;
    goods:Produce[] = [];
    canUseWharf:boolean = true;
    isFirstCaptainShipment:boolean = false;
    canUseHacienda = false;
    tookPlantation = false;
    craftsmanPrivilige = false;

    singleGoodStored: string = '';
    smallWarehouseGoodType: string = '';
    largeWarehouseGoodType: string[] = [];
    wharfGoodType: string = '';

    freeBuildingSlots = 12;
    myBuildings: DataBuilding[] = [];  
    myBuildingNames: string[] = [];
    
    freePlantationSlots = 12;



    chargePlayer(price:number){
      this.doubloons = this.doubloons - price;
    }

    getUniqueGoodTypes(){
      let uniqueGoodTypes:string[] =[];
      this.goods.forEach(good => {
        if(!uniqueGoodTypes.includes(good.name)){
          uniqueGoodTypes.push(good.name);
        }        
      });
      return uniqueGoodTypes;
    }

    getGoodCount(name:string){
      let goodCount:number=0;
      this.goods.forEach(good => {
        if(good.name == name){
          goodCount++;
        }
      });
      return goodCount;
    }

    // countActiveQuaries(){
    //   let count = 0;
    //   this.myPlantations.forEach(plantation => {
    //     // if(isQuarry(plantation.name) && plantation.slot.isOccupied){
    //     //   count++;
    //     // }
    //   });
    //   return count;
    // }

    removeGoods(amount:number, type:string){
      let newGoodsArr:Produce[] = [];
      this.goods.forEach(good => {
        if(good.name != type) newGoodsArr.push(good);
        else if(amount > 0) amount--;
        else{
          newGoodsArr.push(good);
        }
      });
      this.goods = newGoodsArr;
    }

    isBuildingOccupied(building:DataBuilding){
    //   let buildingOccupied = false;
    //   this.myBuildings.forEach(playerBuilding => {
    //     if(playerBuilding.name == building.name && playerBuilding.isOccupied()){
    //       buildingOccupied = true;
    //     }
    //   });
    //   return buildingOccupied;
    }
}