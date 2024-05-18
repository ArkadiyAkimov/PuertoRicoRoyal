import { Component } from '@angular/core';
import { DataPlayerGood, GameStateJson } from '../../services/game-start-http.service';
import { RoleHttpService } from '../../services/role-http.service';
import { SoundService } from '../../services/sound.service';
import { HighlightService } from '../../services/highlight.service';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-supply-display',
  templateUrl: './supply-display.component.html',
  styleUrls: ['./supply-display.component.scss']
})
export class SupplyDisplayComponent {

  myGoods:DataPlayerGood[] = [];
  supplyGoods:Number[] = [0,0,0,0,0];

  constructor(
    public gameService:GameService,
    public highlightService: HighlightService,
    public soundService:SoundService,
    private roleHttp:RoleHttpService
    ){}


    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (gs:GameStateJson) => {
          if(gs.players.length <= 0) return;
          let player = gs.players[this.gameService.playerIndex];
          this.myGoods = player.goods;

          this.supplyGoods[0] = this.gameService.gs.value.cornSupply
          this.supplyGoods[1] = this.gameService.gs.value.indigoSupply
          this.supplyGoods[2] = this.gameService.gs.value.sugarSupply
          this.supplyGoods[3] = this.gameService.gs.value.tobaccoSupply
          this.supplyGoods[4] = this.gameService.gs.value.coffeeSupply
          
          this.gameService.storedGoodTypes= [6,6,6,6];
          this.gameService.targetStorageIndex = 0;
        }
      })
    }

    getSortedGoodButtons(goods:DataPlayerGood[]){
      return goods.sort((a,b) => a.type - b.type );
    }

    onClickGood(good:DataPlayerGood){
      if(this.gameService.selectedShip == 4 && this.gameService.gs.value.currentRole == 5) return;
      if(this.gameService.gs.value.currentRole == 7)
      { 
        this.gameService.changeTargetStorageGood(good);// NEW SSHIT
        return;
      }

      this.roleHttp.postGood(good.id , this.gameService.selectedShip, this.gameService.gs.value.id, this.gameService.playerIndex)   
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



    getGoodButtonHighlightRule(good:DataPlayerGood):string{
      if(this.gameService.storedGoodTypes[0] == good.type) return 'highlight-red';
      else if(this.gameService.storedGoodTypes.includes(good.type)) return 'highlight-yellow';
      else if(good.quantity > 0 && this.gameService.gs.value.currentRole == 7) return 'highlight-green';
      else return '';
    }

    endTurn(){
      this.roleHttp.postEndTurn(this.gameService.gs.value.id, this.gameService.storedGoodTypes, this.gameService.playerIndex)
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

    
    onClickColonistSupply(){
      this.roleHttp.postColonist(this.gameService.gs.value.id, this.gameService.playerIndex)
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

    getColonistSupplyButtonHighlightRule():string{
      if(!this.gameService.gs.value.mayorTookPrivilige 
        && (this.gameService.gs.value.privilegeIndex == this.gameService.playerIndex)
        && (this.gameService.gs.value.currentRole == 2)) return 'highlight-yellow';
      else return '';
    }

    getHotseatHighlight():string{
      if(!this.gameService.isHotSeat) return 'highlight-red';
      else return '';
    }

    toggleHotSeat(){
      this.gameService.isHotSeat = !this.gameService.isHotSeat
    }
}
