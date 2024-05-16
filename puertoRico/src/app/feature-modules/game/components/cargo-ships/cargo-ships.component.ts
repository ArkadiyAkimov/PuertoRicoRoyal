
import { GoodType } from './../../classes/deployables/plantation';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataShip, DataTradeHouse, GameStateJson } from '../../services/game-start-http.service';

@Component({
  selector: 'app-cargo-ships',
  templateUrl: './cargo-ships.component.html',
  styleUrls: ['./cargo-ships.component.scss']
})
export class CargoShipsComponent implements OnInit{
  cargoShips:DataShip[] = [];
  tradeHouse = new DataTradeHouse();
  gs:GameStateJson = new GameStateJson();

  constructor(
    public gameService:GameService,
    ){}

    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (gs:GameStateJson) => {
          if(gs.tradeHouse == null) return;
          this.gs = gs;
          this.tradeHouse = gs.tradeHouse;
          this.cargoShips = gs.ships;
        }
      })
    }

    removeGoodFromWarehouse(warehouseIndex:number){
      this.gameService.targetStorageIndex = warehouseIndex-1;
      this.gameService.storedGoodTypes[warehouseIndex] = 6;
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

  
}
