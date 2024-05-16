import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataPlantation } from '../../classes/deployables/plantation';
import { RoleHttpService } from '../../services/role-http.service';
import { GameStateJson } from '../../services/game-start-http.service';

@Component({
  selector: 'app-plantations',
  templateUrl: './plantations.component.html',
  styleUrls: ['./plantations.component.scss']
})
export class PlantationsComponent implements OnInit{
  exposedPlantations: DataPlantation[] = [];
  gs: GameStateJson = new GameStateJson();

  constructor(
    public gameService:GameService,
    private roleHttp: RoleHttpService
    ){
    }

    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (result:GameStateJson) => {
          this.gs = result;
          this.exposedPlantations = result.plantations.filter(plantation => plantation.isExposed);
        }
      });
    }

   
}
