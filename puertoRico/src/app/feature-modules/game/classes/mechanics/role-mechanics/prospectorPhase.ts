import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";

export function prospectorPhase(
    gameService:GameService,
    hotseatService:HotseatService
    ){
    gameService.getCurrentPlayer().chargePlayer(-1);
    gameService.nextPriviledge();
    hotseatService.loadPlayerBoard(`prospector end`);
}