import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { RoleHttpService } from '../../services/role-http.service';
import { BuildingName, DataPlantation, DataPlayer, GameStateJson, PlayerUtility, RoleName } from '../../classes/general';

@Component({
  selector: 'app-plantations',
  templateUrl: './plantations.component.html',
  styleUrls: ['./plantations.component.scss']
})
export class PlantationsComponent implements OnInit{
  exposedPlantations: DataPlantation[] = [];
  gs: GameStateJson = new GameStateJson();
  player:DataPlayer;
  playerUtility:PlayerUtility;

  constructor(
    public gameService:GameService,
    private roleHttp: RoleHttpService
    ){
      this.player = new DataPlayer();
      this.playerUtility = new PlayerUtility();
    }

    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (result:GameStateJson) => {
          this.gs = result;
          this.player = this.gs.players[this.gameService.playerIndex];
          this.exposedPlantations = result.plantations.filter(plantation => plantation.isExposed);
        }
      });
    }

   disablePlantationInteractionCheck():boolean{
              return this.gs.currentRole != RoleName.Settler 
                    || this.gs.currentPlayerIndex != this.gameService.playerIndex
                    || this.player.tookTurn
   }

   disableQuarryInteractionCheck():boolean{
    let constructionHutCheck =  (this.playerUtility.hasActiveBuilding(BuildingName.ConstructionHut,this.player))

    let canTakeQuarry = false;
    if(constructionHutCheck != undefined) canTakeQuarry = constructionHutCheck;
    if(this.player.index == this.gs.privilegeIndex) canTakeQuarry = true;

    return !canTakeQuarry || this.disablePlantationInteractionCheck();
   }

   disableUpSideDownInteractionCheck(){
    let haciendaCheck =  (this.playerUtility.hasActiveBuilding(BuildingName.Hacienda,this.player)
    && this.playerUtility.getBuilding(BuildingName.Hacienda,this.player)?.effectAvailable);

    let canUseHacienda = false;
    if(haciendaCheck != undefined){
      canUseHacienda = haciendaCheck
    }

    return !canUseHacienda || this.disablePlantationInteractionCheck();
   }
}
