import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataShip, DataTradeHouse, GameStateJson, DataPlayer, GoodType, BuildingName, RoleName, PlayerUtility } from '../../classes/general';
import { SelectionService } from '../../services/selection.service';

@Component({
  selector: 'app-cargo-ships',
  templateUrl: './cargo-ships.component.html',
  styleUrls: ['./cargo-ships.component.scss']
})
export class CargoShipsComponent implements OnInit{
  cargoShips:DataShip[]
  tradeHouse:DataTradeHouse
  gs:GameStateJson
  player:DataPlayer
  playerUtility:PlayerUtility

  constructor(
    public gameService:GameService,
    public selectionService : SelectionService,
    ){
      this.cargoShips = [];
      this.tradeHouse = new DataTradeHouse();
      this.gs = new GameStateJson();
      this.player = new DataPlayer();
      this.playerUtility = new PlayerUtility();
    }

    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (gs:GameStateJson) => {
          if(gs.tradeHouse == null) return;
          this.gs = gs;
          this.player = gs.players[this.gameService.playerIndex];
          this.tradeHouse = gs.tradeHouse;
          this.cargoShips = gs.ships;
        }
      })
    }

    

    public deserializeTradeHouse(goodString:string):GoodType[]
    {
      let goodTypeArr:GoodType[] = []
      const goodNumArr = goodString.substring(1, goodString.length-1).split(',');

      for(let i = 0; i < goodNumArr.length; i++){
        if(goodNumArr[i] != '')
        goodTypeArr.push(this.gameService.goodTypes[Number(goodNumArr[i])]);
      }
      return goodTypeArr;
    }

    

    showSmallWarehouse():boolean{
      return this.gs.currentRole == RoleName.PostCaptain 
          && this.playerUtility.hasActiveBuilding(BuildingName.SmallWarehouse,this.player);
    }

    showLargeWarehouse():boolean{
      return this.gs.currentRole == RoleName.PostCaptain 
          && this.playerUtility.hasActiveBuilding(BuildingName.LargeWarehouse,this.player);
    }

    
}
