import { SoundService } from './../../services/sound.service';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataPlayerGood, GameStateJson } from '../../services/game-start-http.service';
import { RoleHttpService } from '../../services/role-http.service';

@Component({
  selector: 'app-my-controls',
  templateUrl: './my-controls.component.html',
  styleUrls: ['./my-controls.component.scss']
})
export class MyControlsComponent implements OnInit {

  hideVictoryPoints:boolean = true;

  myDoubloons:number = 0;
  myColonists:number = 0;
  myVictoryPoints:number = 0;
  myGoods:DataPlayerGood[] = [];

  constructor(
    public gameService:GameService,
    public soundService:SoundService,
    private roleHttp:RoleHttpService
    ){}


    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (gs:GameStateJson) => {
          if(gs.players.length <= 0) return;
          let player = gs.players[this.gameService.playerIndex];
          this.myDoubloons = player.doubloons;
          this.myColonists = player.colonists;
          this.myVictoryPoints = player.victoryPoints;
          this.myGoods = player.goods;
          
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
}
