import { RoleHttpService } from './../../services/role-http.service';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { Subscription } from 'rxjs';
import { BuildingName, DataBuilding, DataPlantation, DataPlayer, GameStateJson, PlayerUtility, RoleName } from '../../classes/general';


@Component({
  selector: 'app-buildings',
  templateUrl: './buildings.component.html',
  styleUrls: 
  [
    './buildings.component.scss',
    '../../../../styles/buildings.scss',
    '../../../../styles/colors.scss'
  ]
})
export class BuildingsComponent implements OnInit {
  gs : GameStateJson = new GameStateJson();
  subscription: Subscription = new Subscription();
  
  buildingCols:DataBuilding[][] = [];
  buildings1:DataBuilding[] = [];
  buildings2:DataBuilding[] = [];
  buildings3:DataBuilding[] = [];
  buildings4:DataBuilding[] = [];
  buildings5:DataBuilding[] = [];

  
  constructor(
    public gameService:GameService,
    private roleHttpService:RoleHttpService,
    ){
      this.buildingCols = [
        this.buildings1,
        this.buildings2,
        this.buildings3,
        this.buildings4,
      ];
    }

    initializeBuildings(){
      let initBuildings = this.gs.buildings;

      initBuildings.sort((a,b) => a.name - b.name);

      this.buildings1 = [];
      this.buildings2 = [];
      this.buildings3 = [];
      this.buildings4 = [];
      this.buildings5 = [];
    
      let targetBuildingIndex = 6;
      let currentBuildingIndex = 0;
      for(; currentBuildingIndex<targetBuildingIndex; currentBuildingIndex++){
        //if(initBuildings[currentBuildingIndex].quantity > 0)
        this.buildings1.push(initBuildings[currentBuildingIndex]);
      }
      targetBuildingIndex = 12;
      for(; currentBuildingIndex<targetBuildingIndex; currentBuildingIndex++){
        //if(initBuildings[currentBuildingIndex].quantity > 0)
        this.buildings2.push(initBuildings[currentBuildingIndex]);
      }
      targetBuildingIndex = 18;
      for(; currentBuildingIndex<targetBuildingIndex; currentBuildingIndex++){
        //if(initBuildings[currentBuildingIndex].quantity > 0)
        this.buildings3.push(initBuildings[currentBuildingIndex]);
      }
      targetBuildingIndex = 21;
      for(; currentBuildingIndex<targetBuildingIndex; currentBuildingIndex++){
        //if(initBuildings[currentBuildingIndex].quantity > 0)
        this.buildings4.push(initBuildings[currentBuildingIndex]);
      }
      targetBuildingIndex = 23;
      for(; currentBuildingIndex<targetBuildingIndex; currentBuildingIndex++){
        //if(initBuildings[currentBuildingIndex].quantity > 0)
        this.buildings5.push(initBuildings[currentBuildingIndex]);
      }
    }

    ngOnInit(): void {
      this.subscription = this.gameService.gs.subscribe((gs:GameStateJson) => {
        this.gs = gs;
        if(gs.buildings.length > 0)
        this.initializeBuildings();
      })
    }
  
    ngOnDestroy(): void{
      this.subscription.unsubscribe();
    }


    buildingDragDelay = 0;
}