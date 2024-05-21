import { SoundService } from './../../services/sound.service';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { RoleHttpService } from '../../services/role-http.service';
import { BuildingName, DataPlayerGood, GameStateJson, GoodType, PlayerUtility, RoleName } from '../../classes/general';

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
          this.gameService.finishedInitialStorage=false
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

    getEndTurnHighlightRule():string{
      let canEndTurn:boolean = false;
      let gs = this.gameService.gs.value;
      let player = gs.players[this.gameService.playerIndex];

      if(gs.currentPlayerIndex != player.index) return '';

      switch(this.gameService.gs.value.currentRole){
        case RoleName.NoRole:
          break;
        case RoleName.Mayor:
          let emptySlots = 0;

          player.buildings.forEach(building => {
            building.slots.forEach(slot => {
              if(!slot.isOccupied) emptySlots++;
            });
          });

          player.plantations.forEach(plantation => {
            if(!plantation.slot.isOccupied) emptySlots++;
          });

          if(player.colonists == 0 || emptySlots == 0) canEndTurn = true;
          break;
        case RoleName.Craftsman:
          break;
        case RoleName.Captain:
          break;
        case RoleName.PostCaptain:
          if(this.canEndTurnPostCptain(this.gameService.storedGoodTypes)) canEndTurn = true;
          break;
        default:
          canEndTurn = true;
          break;
      }

      return canEndTurn ? 'highlight-yellow' : '';
    }

    canEndTurnPostCptain(storageGoods:number[]):boolean{
      let player = this.gameService.gs.value.players[this.gameService.playerIndex];
      let playerUtility = new PlayerUtility()
      let playerGoodTypes = 0;
      let playerAvailableStorageSpace = 1;
      let playerStoredGoodTypes = 0;
      let canEndTurn = false;
      if (playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse, player)) playerAvailableStorageSpace += 1;
      if (playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse, player)) playerAvailableStorageSpace += 2;
      
      player.goods.forEach(good => {
        if(good.quantity > 0) playerGoodTypes++;
      });
      
      storageGoods.forEach(goodType => {
        if(goodType != 6) playerStoredGoodTypes++;
      });

      let totalGoodTypesMustStore = Math.min(playerGoodTypes, playerAvailableStorageSpace);
      if(totalGoodTypesMustStore == playerStoredGoodTypes) canEndTurn = true;
      return canEndTurn;
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
