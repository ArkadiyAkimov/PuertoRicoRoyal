
import { GameService } from './../../services/game.service';
import { Component, Input } from '@angular/core';
import { RoleHttpService } from '../../services/role-http.service';
import { DataShip, GameStateJson } from '../../classes/general';

@Component({
  selector: 'app-cargo-ship',
  templateUrl: './cargo-ship.component.html',
  styleUrls: 
  [
    './cargo-ship.component.scss',
    '../../../../styles/barrels.scss',
    '../../../../styles/colors.scss'
  ]
})
export class CargoShipComponent{
   @Input() cargoShip:DataShip = new DataShip();
   @Input() shipIndex:number = 0;
   @Input() showShip:boolean = true;

  constructor(
    public gameService:GameService,
    public roleHttp:RoleHttpService,
    ){}

    selectShip(){
      if(this.cargoShip.load == this.cargoShip.capacity) return;
      if(this.gameService.selectedShip == this.shipIndex) this.gameService.selectedShip = 5;
      else this.gameService.selectedShip = this.shipIndex;

      if(this.cargoShip.type != 6){
        let player = this.gameService.gs.value.players[this.gameService.gs.value.currentPlayerIndex];
        
        this.roleHttp.postGood(player.goods[this.cargoShip.type].id , this.gameService.selectedShip, this.gameService.gs.value.id, this.gameService.playerIndex)   
        .subscribe({
          next: (result:GameStateJson) => {
            console.log('success:',result);
            this.gameService.gs.next(result);
          },
          error: (response:any)=> {
            console.log("error:",response.error.text);
          }
        });
        this.gameService.selectedShip = 4;
      }
    }

    canShipFlag():boolean{
      if(this.cargoShip.type == 6) return true;
      let player = this.gameService.gs.value.players[this.gameService.gs.value.currentPlayerIndex];
      if((player.goods[this.cargoShip.type].quantity > 0) && (this.cargoShip.load < this.cargoShip.capacity)) return true;
      return false;
    }

    getShipName(){
      return `${(this.shipIndex == 3)?"Wharf":`Ship of ${this.cargoShip.capacity}`}`;
    }
}
