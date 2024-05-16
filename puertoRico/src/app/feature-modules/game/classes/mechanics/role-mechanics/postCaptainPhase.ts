import { RoleService } from './../../../services/role.service';
import { postCaptain } from './role';
import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { ScrollService } from "../../../services/scroll.service";
import { Player } from "../player";
import { RoundPhase } from "./phaseInterface";

export class PostCaptainPhase implements RoundPhase {
     gameService: GameService;
    constructor(
        gameService:GameService,
        private hotseatService:HotseatService,
        private scrollService:ScrollService,
        private roleService:RoleService
        ){
        this.gameService = gameService;
    }
    
    firstPlayer:number = -1;

     startPhase(): void {
        this.firstPlayer = this.gameService.activePlayer;
        this.gameService.canChooseGoodToKeep = true;
        this.hotseatService.loadPlayerBoard(`postCaptain start`);

        let player = this.gameService.getCurrentPlayer();
        if(!this.doesPlayerHaveChoice(player)){
            this.autoDropGoods(player);
            this.contPhase();
        }
    }

     contPhase(): void {
        this.gameService.nextPlayer();
        if(this.gameService.activePlayer == this.firstPlayer){
            this.endPhase();
            return;
        }

        this.hotseatService.loadPlayerBoard(`postCaptain cont`);

        let player = this.gameService.getCurrentPlayer();
        if(!this.doesPlayerHaveChoice(player)){
            this.autoDropGoods(player);
            this.contPhase();
        }
    }
    

     endPhase(): void {
        this.gameService.canChooseGoodToKeep = false;
        this.gameService.nextPriviledge();
        this.gameService.resetWharfs();
        this.scrollService.srcollTo(postCaptain);
        this.gameService.roleInProgress = false;
        this.hotseatService.loadPlayerBoard(`postCaptain end`);
    }


    doesPlayerHaveChoice(player:Player){
        let playerUniqueGoods = player.getUniqueGoodTypes();
        
        return playerUniqueGoods.length > 1;
    }

    autoDropGoods(player:Player){
        if(player.goods.length == 0) return;
        this.gameService.goodsMap.set(player.goods[0], this.gameService.goodsMap.get(player.goods[0])! + (player.goods.length - 1));
        player.goods = [player.goods[0]];
    }
}