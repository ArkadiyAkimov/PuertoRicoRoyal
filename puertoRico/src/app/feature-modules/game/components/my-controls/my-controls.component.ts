import { SoundService } from './../../services/sound.service';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { RoleHttpService } from '../../services/role-http.service';
import { BuildingName, DataPlayer, DataPlayerGood, DataShip, GameStateJson, GoodName, GoodType, PlayerUtility, RoleName } from '../../classes/general';
import { StylingService } from '../../services/styling.service';
import { SelectionService } from '../../services/selection.service';
import { HighlightService } from '../../services/highlight.service';
import { observeOn } from 'rxjs';

@Component({
  selector: 'app-my-controls',
  templateUrl: './my-controls.component.html',
  styleUrls: ['./my-controls.component.scss',
    '../../../../styles/variables.scss'
  ]
})
export class MyControlsComponent implements OnInit {
  hideVictoryPoints:boolean = true;

  player:DataPlayer = new DataPlayer()
  myDoubloons:number = 0;
  myColonists:number = 0;
  myVictoryPoints:number = 0;
  myGoods:DataPlayerGood[] = [];

  constructor(
    public gameService:GameService,
    public soundService:SoundService,
    private roleHttp:RoleHttpService,
    public selectionService : SelectionService,
    public highlightService : HighlightService,
    public stylingService:StylingService,
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
          this.player = player;
        }
      })
    }


    onClickGood(good:DataPlayerGood){
      let gs = this.gameService.gs.value;
      let player = gs.players[this.gameService.playerIndex];

      switch(gs.currentRole){
        case RoleName.PostCaptain:
          this.selectionService.selectGoodToStore(good);// NEW SSHIT
          break;
        case RoleName.Captain:
          if(this.selectionService.selectedShip == 4) this.selectionService.selectSmallWharfGoods(good.type);
          else if(this.selectionService.selectedShip == 3){
            this.selectionService.fillWharf(good);
            setTimeout(()=>this.postGood(good),100);
          }
          else this.postGood(good);
          break;
        case RoleName.Trader:
          if(this.selectionService.sellingToTradingPost){
            this.roleHttp.postGoodTradingPost(good.id , this.selectionService.selectedShip, this.gameService.gs.value.id, this.gameService.playerIndex)   
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
          else this.postGood(good);
          break;
        default:
          this.postGood(good);
              break;
      }
    }

    postGood(good:DataPlayerGood){
      this.roleHttp.postGood(good.id , this.selectionService.selectedShip, this.gameService.gs.value.id, this.gameService.playerIndex)   
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

   

    getEndTurnHighlightRule():string{
      let canEndTurn:boolean = false;
      let gs = this.gameService.gs.value;
      let player = gs.players[this.gameService.playerIndex];
      if(player == undefined) return "";

      if(gs.currentPlayerIndex != this.gameService.playerIndex) return '';

      switch(this.gameService.gs.value.currentRole){
        case RoleName.Draft:
          canEndTurn = false;
          break;
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
          if(this.canEndTurnCaptain()) canEndTurn = true;
          break;
        case RoleName.PostCaptain:
          if(this.selectionService.canEndTurnPostCptain())canEndTurn = true; 
          break;
        default:
          canEndTurn = true;
          break;
      }

      return canEndTurn ? 'highlight-blue' : '';
    }

    canEndTurnCaptain(){
      let gs = this.gameService.gs.value;
      let player = gs.players[this.gameService.playerIndex];
      if(player == undefined) return 0;

      if(gs.currentPlayerIndex != player.index) return;
    
      let playerTotalGoods = 0;
      player.goods.forEach(goodType => {
        playerTotalGoods += goodType.quantity;
      });
      let canEndTurn = true;

      if(playerTotalGoods == 0) return true;
    
      for(let s=0; s<3; s++){
        let ship:DataShip = gs.ships[s];
        if(ship.type == 6) return false;
        else if(ship.type != 6){
          if((player.goods[ship.type].quantity > 0) && (ship.capacity > ship.load)){
            canEndTurn = false;
          } 
        }
      }

      return canEndTurn;
    }

    endTurn(){
    let gs = this.gameService.gs.value;
    let player = gs.players[this.gameService.playerIndex];
    if(player == undefined) return;

    if(gs.currentPlayerIndex != player.index) return;

      switch(gs.currentRole){
        case RoleName.PostCaptain:
          this.roleHttp.postEndTurnPostCaptain(
            this.gameService.gs.value.id,
            this.selectionService.windroseStoredGood,
            this.selectionService.storeHouseStoredGoods,
            this.selectionService.smallWarehouseStoredType,
            this.selectionService.smallWarehouseStoredQuantity,
            this.selectionService.largeWarehouseStoredTypes,
            this.selectionService.largeWarehouseStoredQuantities,
            this.gameService.playerIndex) //temp
          .subscribe({
            next: (result:GameStateJson) => {
              console.log('success:',result);
              this.gameService.gs.next(result);
            },
            error: (response:any)=> {
              console.log("error:",response.error.text);
            }
          });
          break;
        case RoleName.Captain:
          if(!this.canEndTurnCaptain()) return;
          if(this.selectionService.selectedShip == 4){
          this.roleHttp.postEndTurnSmallWharf(
            this.gameService.gs.value.id, this.selectionService.selectedGoodsSmallWharf, this.gameService.playerIndex) //temp
          .subscribe({
            next: (result:GameStateJson) => {
              console.log('success:',result);
              this.gameService.gs.next(result);
            },
            error: (response:any)=> {
              console.log("error:",response.error.text);
            }
          });
        }else{
          this.roleHttp.postEndTurn(
            this.gameService.gs.value.id,this.gameService.playerIndex) //temp
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
          break;
          default:
            this.roleHttp.postEndTurn(
              this.gameService.gs.value.id,this.gameService.playerIndex) //temp
            .subscribe({
              next: (result:GameStateJson) => {
                console.log('success:',result);
                this.gameService.gs.next(result);
              },
              error: (response:any)=> {
                console.log("error:",response.error.text);
              }
            });
            break;
      }

    }
  
}
