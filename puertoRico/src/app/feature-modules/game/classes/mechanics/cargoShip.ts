import { Produce } from "../goods/goodTypes";

export class cargoShip {
    size:number;
    numberOfGood:number = 0;
    goodType:Produce|null  = null;
    
    constructor(size:number){
        this.size = size;
    }

    isEmpty(){
        return this.numberOfGood == 0;
    }

    tryAddGoods(amount:number,type:Produce){
        if(this.goodType != type && !this.isEmpty()) return 0;
        let availableCap = this.size - this.numberOfGood;
        let transferedGooods = Math.min(amount,availableCap);
        this.numberOfGood += transferedGooods;
        return transferedGooods;
    }

    freeShipSlots(){
        return this.size - this.numberOfGood;
    }

     isFull(){
        return this.numberOfGood == this.size ;
     }

     resetShip(){
        this.numberOfGood = 0;
        this.goodType = null;
     }
}