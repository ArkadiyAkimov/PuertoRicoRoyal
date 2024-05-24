import { Component } from '@angular/core';
import { RoleHttpService } from '../../services/role-http.service';
import { SoundService } from '../../services/sound.service';
import { HighlightService } from '../../services/highlight.service';
import { GameService } from '../../services/game.service';
import { BuildingName, DataPlayer, DataPlayerGood, GameStateJson, PlayerUtility, RoleName } from '../../classes/general';

@Component({
  selector: 'app-supply-display',
  templateUrl: './supply-display.component.html',
  styleUrls: ['./supply-display.component.scss']
})
export class SupplyDisplayComponent {

  myGoods:DataPlayerGood[]
  supplyGoods:Number[]
  player:DataPlayer
  playerUtility:PlayerUtility

  constructor(
    public gameService:GameService,
    public highlightService: HighlightService,
    public soundService:SoundService,
    private roleHttp:RoleHttpService
    ){
      this.myGoods = [];
      this.supplyGoods = [0,0,0,0,0];
      this.player = new DataPlayer();
      this.playerUtility = new PlayerUtility();
    }


    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (gs:GameStateJson) => {
          if(gs.players.length <= 0) return;
          this.player = gs.players[this.gameService.playerIndex];
          this.myGoods = this.player.goods;

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
      else if(good.quantity > 0 && this.gameService.gs.value.currentRole == RoleName.PostCaptain) return 'highlight-green';
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
        && (this.gameService.gs.value.privilegeIndex == this.player.index)
        && (this.gameService.gs.value.currentRole == RoleName.Mayor)) 
        return 'highlight-yellow';
      else if (this.gameService.gs.value.currentRole == RoleName.Settler
              && this.playerUtility.hasActiveBuilding(BuildingName.Hospice,this.player) 
              && this.playerUtility.getBuilding(BuildingName.Hospice,this.player)?.effectAvailable) return 'highlight-yellow';
      else if (this.gameService.gs.value.currentRole == RoleName.Builder
              && this.player.tookTurn 
              && this.player.index == this.gameService.gs.value.currentPlayerIndex) return 'highlight-yellow';  
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
