import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { ScrollService } from "../../../services/scroll.service";
import { RoundPhase } from "./phaseInterface";

export class SettlerPhase implements RoundPhase {
    gameService: GameService;
   constructor(
    gameService:GameService,
    private hotseatService:HotseatService,
    private scrollService:ScrollService
    ){
       this.gameService = gameService;
   }


  startPhase(){
    this.gameService.getCurrentPlayer().tookPlantation = false;
    
    if(this.gameService.exposedPlantations.length == 0){
        this.drawPlantations();
        this.drawQuarries();
        this.drawUpsideDown();
    }

    this.gameService.getCurrentPlayer().canUseHacienda = true;
    this.gameService.canTakePlantation = true;
    this.hotseatService.loadPlayerBoard(`settler start`);
}

  contPhase(){
    this.gameService.nextPlayer();
    this.gameService.getCurrentPlayer().canUseHacienda = true;
  
    if(this.gameService.activePlayer == this.gameService.privilege){
        this.endPhase();
        return;
    }

    this.hotseatService.loadPlayerBoard(`settler cont`);
}

  endPhase(){
    let length = this.gameService.exposedPlantations.length;
    for(let i=0; i<length; i++){
        this.gameService.exposedPlantations.pop();
    }

    this.drawPlantations();
    this.drawQuarries();
    this.drawUpsideDown();
    this.gameService.nextPriviledge();
    this.gameService.canTakePlantation = false;
    this.gameService.roleInProgress = false;
    this.hotseatService.loadPlayerBoard(`settler end`);
}


drawPlantations(){
  for(let i=0; i <= this.gameService.players.length; i++){
      this.gameService.exposedPlantations.push(
          this.gameService.pickAndRemoveRandomElement<Plantation>(this.gameService.plantations)!
          );
  }
}

drawUpsideDown(){
if(this.gameService.plantations.length > 0){
  this.gameService.exposedPlantations.push(new UpsideDown());
}
}

drawQuarries(){
if(this.gameService.quarries > 0){
this.gameService.exposedPlantations.push(new Quarry());
}
}


}
