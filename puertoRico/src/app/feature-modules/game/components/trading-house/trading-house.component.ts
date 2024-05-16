import { Component } from '@angular/core';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-trading-house',
  templateUrl: './trading-house.component.html',
  styleUrls: ['./trading-house.component.scss']
})
export class TradingHouseComponent {
  constructor(
    public gameService:GameService
    ){}
}
