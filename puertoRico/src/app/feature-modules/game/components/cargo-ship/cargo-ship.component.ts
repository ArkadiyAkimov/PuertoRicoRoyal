
import { GameService } from './../../services/game.service';
import { Component, Input } from '@angular/core';
import { RoleHttpService } from '../../services/role-http.service';
import { DataShip, GameStateJson } from '../../classes/general';
import { SelectionService } from '../../services/selection.service';
import { StylingService } from '../../services/styling.service';
import { HighlightService } from '../../services/highlight.service';

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

  constructor(
    public gameService:GameService,
    public selectionService : SelectionService,
    public stylingService : StylingService,
    public highlightService:HighlightService,
    public roleHttp:RoleHttpService,
    ){}
    
    showShip():boolean{
      if(this.shipIndex < 3) return true;
      if(this.shipIndex == 3) return this.gameService.wharfDisplayCheck();
      if(this.shipIndex == 4) return this.gameService.smallWharfDisplayCheck();
      return false;
    }

    selectShip(){
      if(this.shipIndex == 4) this.selectionService.toggleSmallWharf();
      if(this.shipIndex == 4)  return;
      if(this.cargoShip.capacity == this.cargoShip.load) return;
      
      if(this.selectionService.selectedShip == this.shipIndex) this.selectionService.selectedShip = 5;
      else this.selectionService.selectedShip = this.shipIndex;


      if(this.cargoShip.type != 6){
        let player = this.gameService.gs.value.players[this.gameService.gs.value.currentPlayerIndex];
        
        this.roleHttp.postGood(player.goods[this.cargoShip.type].id , this.selectionService.selectedShip, this.gameService.gs.value.id, this.gameService.playerIndex)   
        .subscribe({
          next: (result:GameStateJson) => {
            console.log('success:',result);
            this.gameService.gs.next(result);
          },
          error: (response:any)=> {
            console.log("error:",response.error.text);
          }
        });
      }
    }

    canShipFlag():boolean{
      if(this.cargoShip.type == 6) return true;
      let player = this.gameService.gs.value.players[this.gameService.gs.value.currentPlayerIndex];
      if((player.goods[this.cargoShip.type].quantity > 0) && (this.cargoShip.load < this.cargoShip.capacity)) return true;
      return false;
    }

    getShipName(){
      if(this.shipIndex < 3) return "Ship of " + this.cargoShip.capacity;
      else if(this.shipIndex == 3) return "Wharf";
      else return "Small Wharf";
    }
}
