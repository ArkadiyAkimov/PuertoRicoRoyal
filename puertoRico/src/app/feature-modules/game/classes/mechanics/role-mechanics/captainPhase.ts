import { GameService } from "../../../services/game.service";
import { HotseatService } from "../../../services/hotseat.service";
import { RoleService } from "../../../services/role.service";
import { RoundPhase } from "./phaseInterface";
import { postCaptain } from "./role";

export class CaptainPhase implements RoundPhase {
     gameService: GameService;
    constructor(
        gameService:GameService,
        private hotseatService:HotseatService,
        private roleService:RoleService
        ){
        this.gameService = gameService;
    }
    
    playersSkipped = 0;

     startPhase(): void {
        this.gameService.getCurrentPlayer().isFirstCaptainShipment = true;
        this.gameService.canShipGoods = true;
        this.hotseatService.loadPlayerBoard('captain start');

        if(!this.checkIfPlayerHasValidGoods()){
            this.playersSkipped++;
            this.contPhase();
        } 
    }

     contPhase(): void {
        this.gameService.nextPlayer();
        if(this.playersSkipped == this.gameService.players.length){
            this.endPhase();
            return;
        }

        this.hotseatService.loadPlayerBoard('captain cont');

        if(!this.checkIfPlayerHasValidGoods()){
            this.playersSkipped++;
            this.contPhase();
            return;
        } 
    }
    
     endPhase(): void {
        this.gameService.canShipGoods = false;
        this.offloadShips();
        this.playersSkipped = 0;
        this.hotseatService.loadPlayerBoard('captain end');
        this.roleService.roleMap.get(postCaptain)!.startPhase();
    }

    checkIfPlayerHasValidGoods():boolean{
        if(this.gameService.getCurrentPlayer().goods.length == 0) return false;
        let playerTypes = this.gameService.getCurrentPlayer().getUniqueGoodTypes();

        for(let i=0; i<this.gameService.cargoShips.length;i++){
            let ship= this.gameService.cargoShips[i];
            
            if(!ship.isEmpty()){
                let index = playerTypes.indexOf(ship.goodType!.name,0);
                if (index > -1 && ship.isFull()){
                playerTypes.splice(index,1);
                }
                else if(index > -1){
                return true;
                }
            } 
        }
        if(!this.checkIfAvailableShip()) return false;
        return playerTypes.length > 0;
    }

    checkIfAvailableShip(){
        let availableShips = false;
        this.gameService.cargoShips.forEach(ship => {
            if(ship.isEmpty()) availableShips = true;
        })
        return availableShips;
    }

    offloadShips(){
        this.gameService.cargoShips.forEach(ship => {
            if(ship.isFull()){
                this.gameService.goodsMap.set(ship.goodType!, this.gameService.goodsMap.get(ship.goodType!)! + ship.size);
                ship.resetShip();
            }
        });
    }

}