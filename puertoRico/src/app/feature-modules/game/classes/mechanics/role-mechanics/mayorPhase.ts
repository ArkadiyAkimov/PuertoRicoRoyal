import { Player } from './../player';
import { mayor } from './role';
import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { ScrollService } from "../../../services/scroll.service";
import { RoundPhase } from "./phaseInterface";

export class MayorPhase implements RoundPhase {
     gameService: GameService;
    constructor(
        gameService:GameService,
        private hotseatService:HotseatService,
        private scrollService:ScrollService
        ){
        this.gameService = gameService;
    }

     startPhase(): void {
        this.scrollService.srcollTo(mayor,true);
        this.gameService.canMoveColonists = true;

        let takenColoninsts = Math.ceil(this.gameService.colonistsShip/this.gameService.players.length);
        this.gameService.getCurrentPlayer().colonists += takenColoninsts + 1;
        this.gameService.colonistsShip -= takenColoninsts;

        console.log('before');
        this.hotseatService.loadPlayerBoard('mayor start');
        console.log('after');
        this.uselessTurnSkipper();
    }

     contPhase(): void {
        //this.gameService.nextPlayer();
        if(this.gameService.activePlayer == this.gameService.privilege)
        {
            this.endPhase();
            return;
        }

        let takenColoninsts = Math.ceil(this.gameService.colonistsShip/this.gameService.players.length);
        this.gameService.getCurrentPlayer().colonists += takenColoninsts;
        this.gameService.colonistsShip -= takenColoninsts;  
        this.hotseatService.loadPlayerBoard('mayor cont');
        this.uselessTurnSkipper();
    }
    
     endPhase(): void {
        this.gameService.canMoveColonists = false;
        this.gameService.nextPriviledge();

       let freeSlots:number = 0;

        // this.gameService.players.forEach(player => {
        //     player.myBuildings.forEach(building => {
        //       freeSlots += building.freeSlots();
        //     });
        // });

        this.gameService.colonistsShip = Math.max(freeSlots,this.gameService.players.length);
        
        this.scrollService.srcollTo(mayor);
        this.gameService.roleInProgress = false;
        
        this.hotseatService.loadPlayerBoard(`mayor end`);
    }


    isPlayerInputNecessary(player:Player):boolean{
        let freeSlots = 0;
        // player.myBuildings.forEach(building => {
        //     freeSlots += building.freeSlots();
        //   });

        player.myPlantations.forEach(plantation => {
            freeSlots += plantation.freeSlots();
        });
        
        if(player.colonists >= freeSlots) return false;
        return true;
    }

    fillInEmptySlots(player:Player){
        player.myBuildings.forEach(building => {
            //   building.slots.forEach(slot => {
            //     if(!slot.isOccupied){
            //         player.colonists--;
            //         slot.isOccupied = true;
            //     }
            // });
          });

          player.myPlantations.forEach(plantation => {
           let slot = plantation.slot;
              if(!slot.isOccupied){
                 player.colonists--;
                slot.isOccupied = true;
              }
        });

          this.contPhase();
    }

    uselessTurnSkipper(){
        let player = this.gameService.getCurrentPlayer();
        if(player.myBuildings.length == 0 && player.myPlantations.length == 0){
            console.log(player.name + ' has no slots to fill');
            this.contPhase();
        } 
        else if (!this.isPlayerInputNecessary(player)){ 
        console.log('No player input necessary '+ player.name);
        this.fillInEmptySlots(player);
        }
    }


}