import { builder } from './role';
import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { ScrollService } from "../../../services/scroll.service";
import { RoundPhase } from "./phaseInterface";

export class BuilderPhase implements RoundPhase {
     gameService: GameService;
    constructor(
        gameService:GameService,
        private hotseatService:HotseatService,
        private scrollService:ScrollService,
        ){
        this.gameService = gameService;
    }

     startPhase(): void {
        this.scrollService.srcollTo(builder,true);
        this.gameService.canTakeBuilding = true;
        this.hotseatService.loadPlayerBoard(`builder start`);
    }

     contPhase(): void {
        this.gameService.nextPlayer();
        if(this.gameService.activePlayer == this.gameService.privilege){
            this.endPhase();
            return;
        }
        this.hotseatService.loadPlayerBoard(`builder cont`);
    }
    

     endPhase(): void {
        this.gameService.canTakeBuilding = false;
        this.gameService.nextPriviledge();
        this.scrollService.srcollTo(builder);
        this.gameService.roleInProgress = false;
        this.hotseatService.loadPlayerBoard('builder end');
    }
}