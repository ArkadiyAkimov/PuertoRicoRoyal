
import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { coffee, corn, indigo, Produce, sugar, tobacco } from "../../goods/goodTypes";
import { Player } from "../player";
import { RoundPhase } from "./phaseInterface";

export class CraftsmanPhase implements RoundPhase {

    constructor(
        public gameService:GameService,
        public hotseatService:HotseatService,
        ){}

     startPhase(): void {

        this.hotseatService.loadPlayerBoard('craftsman start');
        this.addGoodsToPlayer(this.gameService.getCurrentPlayer());
        
    }

     contPhase(): void {
        this.gameService.nextPlayer();
        if(this.gameService.activePlayer == this.gameService.privilege){
            this.endPhase();
            return;
        }

        this.hotseatService.loadPlayerBoard('craftsman cont');
        this.addGoodsToPlayer(this.gameService.getCurrentPlayer());
      
    }
    

     endPhase(): void {
        this.gameService.nextPriviledge();
        this.hotseatService.loadPlayerBoard('craftsman end');
        this.gameService.roleInProgress = false;
    }

    handlePriviledge(){

    }

    addGoodsToPlayer(player:Player){
        let cornCount:number = 0;
        let indigoCount:number[] = [0,0];
        let sugarCount:number[] = [0,0];
        let tobaccoCount:number[] = [0,0];
        let coffeeCount:number[] = [0,0];
        
        player.myPlantations.forEach(plantation => {
        if(plantation.numberOfColonists() > 0){   
           switch(plantation.name){
            case corn.name:
                cornCount++;
                break;
            case indigo.name:
                indigoCount[0]++;
                break;
            case sugar.name:
                sugarCount[0]++;
                break;
            case tobacco.name:
                tobaccoCount[0]++;
                break;
            case coffee.name:
                coffeeCount[0]++;
                break;
            default:
                break;
           } 
            }

        });

        player.myBuildings.forEach(building => {
            // building.slots.forEach(slot => {
            //     if(slot.isOccupied && building instanceof ActiveProdBuilding){
            //         switch( (<ActiveProdBuilding>building).good.name ){
            //             case indigo.name:
            //                 indigoCount[1]++;
            //                 break;
            //             case sugar.name:
            //                 sugarCount[1]++;
            //                 break;
            //             case tobacco.name:
            //                 tobaccoCount[1]++;
            //                 break;
            //             case coffee.name:
            //                 coffeeCount[1]++;
            //                 break;
            //             default:
            //                 break;
            //            } 
            //     }                
            // });
         });

         let totalIndigo = Math.min(indigoCount[0],indigoCount[1]);
         let totalSugar = Math.min(sugarCount[0],sugarCount[1]);
         let totalTobacco = Math.min(tobaccoCount[0],tobaccoCount[1]);
         let totalCoffe = Math.min(coffeeCount[0],coffeeCount[1]);

         let uniqueGoodsProduced = 
         this.giveGoodsOfType(corn,player,cornCount) +
         this.giveGoodsOfType(indigo,player,totalIndigo) +
         this.giveGoodsOfType(sugar,player,totalSugar) +
         this.giveGoodsOfType(tobacco,player,totalTobacco) +
         this.giveGoodsOfType(coffee,player,totalCoffe);

        //  if(player.isBuildingOccupied(factory)){
        //     if(uniqueGoodsProduced == 5) player.chargePlayer(-5);
        //     else if(uniqueGoodsProduced > 0) player.chargePlayer(-(uniqueGoodsProduced-1));
        //  }
         
         if(this.gameService.activePlayer == this.gameService.privilege){
            if( uniqueGoodsProduced >= 2 ){
            player.craftsmanPrivilige = true;
            this.hotseatService.loadPlayerBoard('craftsman priviledge');
            return;
            } else if ( uniqueGoodsProduced == 1){
                if(cornCount > 0) this.giveGoodsOfType(corn,player,1);
                if(totalIndigo > 0) this.giveGoodsOfType(indigo,player,1);
                if(totalSugar > 0) this.giveGoodsOfType(sugar,player,1);
                if(totalTobacco > 0) this.giveGoodsOfType(tobacco,player,1);
                if(totalCoffe > 0) this.giveGoodsOfType(coffee,player,1);
            } 
         }

         this.contPhase();
    }

     giveGoodsOfType(goodType:Produce,player:Player,goodCount:number){
        let didProduce = false;
        for(let i=0; i<Math.trunc(goodCount);i++){
            console.log( goodType.name +' to ' + player.name);
            this.supplySufficiencyCheckAndPush(goodType,player)
            didProduce = true;
         }
        if(didProduce) return 1;
        return 0;
    }

    supplySufficiencyCheckAndPush(good:Produce,player:Player){
        let desiredGoodSupply = this.gameService.goodsMap.get(good);
        if(desiredGoodSupply == null) return;
        //console.log(good.name, this.gameService.goodsMap.get(good));
        if(desiredGoodSupply != 0){
            this.gameService.goodsMap.set(good,desiredGoodSupply - 1);
            player.goods.push(good)
        }
    }
}