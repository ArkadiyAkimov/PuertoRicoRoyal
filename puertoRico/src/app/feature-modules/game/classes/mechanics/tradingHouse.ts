


export class TradingHouse{
    goods:Produce[] = [];
    numberOfGoods:number = 0;

    sellGood(good:Produce,player:Player){
        this.goods.push(good);
        this.numberOfGoods++;
        player.removeGoods(1,good.name);
        player.chargePlayer(-good.price);
        // if(player.isBuildingOccupied(smallMarket)) player.chargePlayer(-1);
        // if(player.isBuildingOccupied(largeMarket)) player.chargePlayer(-2);
    }

    canSellGood(good:Produce,player:Player){
        if(player.getGoodCount(good.name) == 0) return false;
        // if(this.numberOfGoods == 4) return false;
        // if(player.isBuildingOccupied(office)) return true;
        let canSellGood = true;
        this.goods.forEach(goodInMarket => {
            if(goodInMarket == good){
                canSellGood = false;
            }
        });
        return canSellGood;
    }
   
}