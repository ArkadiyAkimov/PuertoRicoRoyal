import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { ScrollService } from "../../../services/scroll.service";
import { RoundPhase } from "./phaseInterface";

export class TraderPhase implements RoundPhase {
     gameService: GameService;
    constructor(
        gameService:GameService,
        private hotseatService:HotseatService,
        private scrollService:ScrollService
        ){
        this.gameService = gameService;
    }

     startPhase(): void {
        this.gameService.canSellGoods = true;
        this.hotseatService.loadPlayerBoard('trader start');
        this.canCurrPlayerSellAnything();
    }

     contPhase(): void {
        this.gameService.nextPlayer();
        if(this.gameService.activePlayer == this.gameService.privilege){
            this.endPhase();
            return;
        }
        this.hotseatService.loadPlayerBoard(`trader cont`);
        this.canCurrPlayerSellAnything();
    }
    
     endPhase(): void {
        this.gameService.canSellGoods = false;
        this.gameService.nextPriviledge();
        
        if(this.gameService.tradingHouse.numberOfGoods == 4){
            this.gameService.tradingHouse.numberOfGoods = 0;
        for(let i=0;i<4;i++){
            let good = this.gameService.tradingHouse.goods.pop();
            this.gameService.goodsMap.set(good!, this.gameService.goodsMap.get(good!)! + 1);
            //check if trading house full andreturn all goods to goodsMap;
            }
        }
        this.gameService.roleInProgress = false;
        this.hotseatService.loadPlayerBoard('trader end');
 
    }

    canCurrPlayerSellAnything(){
        let canSellAnyGoods = false;
        this.gameService.getCurrentPlayer().goods.forEach(good => {
            if(this.gameService.tradingHouse.canSellGood(good,this.gameService.getCurrentPlayer())){
                canSellAnyGoods = true;
            }
        });
        if(!canSellAnyGoods){
            this.contPhase();
        }
    }
}