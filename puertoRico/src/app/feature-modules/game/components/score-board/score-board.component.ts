import { GameStateJson, DataPlayer } from '../../classes/general';
import { GameService } from './../../services/game.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-score-board',
  templateUrl: './score-board.component.html',
  styleUrls: ['./score-board.component.scss']
})
export class ScoreBoardComponent implements OnInit{
  gs:GameStateJson = new GameStateJson();
  players:DataPlayer[] = [];

  constructor(private gameService:GameService){}

  ngOnInit(): void {
    this.gameService.gs.subscribe((gs:GameStateJson) => {
      this.gs = gs;
      this.players = gs.players;
      this.sortPlayers();
    })
  }

  sortPlayers(){
    this.players.sort((a,b) => b.score - a.score);
  }

  floor(value:number):number{
    return Math.floor(value);
  }

  breaker(value:number):number{
    return Math.round((value - Math.floor(value))*1000);
  }

  getAchievments(player:DataPlayer):string[]{
    let achievmentsArray = [];
   
    // this.players.sort((a,b) => b.victoryPoints - a.victoryPoints);
    // if(this.players[0].id == player.id && achievmentsArray.length != 2) achievmentsArray.push("captain");
    // this.players.sort((a,b) => this.countPlayerGoods(b) - this.countPlayerGoods(a));
    // if(this.players[0].id == player.id && achievmentsArray.length != 2) achievmentsArray.push("craftsman");
    // this.players.sort((a,b) => this.countPlayerBuildingPoints(b) - this.countPlayerBuildingPoints(a));
    // if(this.players[0].id == player.id && achievmentsArray.length != 2) achievmentsArray.push("builder");
    // this.players.sort((a,b) => this.countPlayerColonists(b) - this.countPlayerColonists(a));
    // if(this.players[0].id == player.id && achievmentsArray.length != 2) achievmentsArray.push("mayor");
    // this.players.sort((a,b) => this.countPlayerPlantations(b) - this.countPlayerPlantations(a));
    // if(this.players[0].id == player.id && achievmentsArray.length != 2) achievmentsArray.push("settler");
    // this.players.sort((a,b) => b.doubloons - a.doubloons);
    // if(this.players[0].id == player.id && achievmentsArray.length != 2) achievmentsArray.push("trader");

    achievmentsArray.push("captain");
    achievmentsArray.push("mayor");
    if(this.players.length == 4)achievmentsArray.push("craftsman");
    if(this.players.length == 5)achievmentsArray.push("trader");
    achievmentsArray.push("prospector");
    

    this.players.sort((a,b) => b.score - a.score);

    // if(this.players[this.players.length -1].index == player.index) achievmentsArray.push("prospector")

    return achievmentsArray;
  }

  countPlayerBuildingPoints(player:DataPlayer):number{
    let count = 0;

    player.buildings.forEach(building => {
      let victoryScore = this.gameService.getBuildingType(building)?.victoryScore;
      if(victoryScore != undefined) count += victoryScore;
    });

    return count;
  }

  countPlayerPlantations(player:DataPlayer):number{

    let count = 0;
    player.plantations.forEach(plantation => {
      count++;
    })

    return count;
  }


  countPlayerColonists(player:DataPlayer):number{

    let count = 0;

    count+= player.colonists;

    player.buildings.forEach(building => {
      building.slots.forEach(slot => {
        if(slot.isOccupied) count++;
      });
    });

    player.plantations.forEach(plantation => {
      if(plantation.slot.isOccupied) count++
    })

    return count;
  }

  countPlayerGoods(player:DataPlayer):number{
    let count = 0;

    player.goods.forEach(good => {
      count+= good.quantity;
    });

    return count;
  }
}


