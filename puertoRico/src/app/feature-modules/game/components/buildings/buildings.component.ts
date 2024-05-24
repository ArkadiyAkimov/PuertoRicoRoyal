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
      
      if(this.gs.currentRole != RoleName.Draft) initBuildings = initBuildings.filter(building => building.isDrafted)

      initBuildings.sort((a,b) => a.name - b.name);

      this.buildings1 = [];
      this.buildings2 = [];
      this.buildings3 = [];
      this.buildings4 = [];
      this.buildings5 = [];

      let buildingsMatrix = [this.buildings1,this.buildings2,this.buildings3,this.buildings4,this.buildings5]
      let ArrayIndex = 0;

      let currentBuildingIndex = 0;

      do{
          if(this.gameService.getBuildingType(initBuildings[currentBuildingIndex])?.victoryScore == ArrayIndex + 1){
            buildingsMatrix[ArrayIndex].push(initBuildings[currentBuildingIndex]);
            currentBuildingIndex++
          } else if(this.gameService.getBuildingType(initBuildings[currentBuildingIndex])?.victoryScore == 8){
            //statue :|
            buildingsMatrix[3].push(initBuildings[currentBuildingIndex]);
            currentBuildingIndex++
          }
            else ArrayIndex++;

        }while (currentBuildingIndex < initBuildings.length- 1)

          buildingsMatrix[ArrayIndex-1].push(initBuildings[currentBuildingIndex]);
    }

    getBuildingHighlight(building:DataBuilding){
      if(this.gameService.gs.value.currentRole == RoleName.Draft){
      if(building.isDrafted) return "isDrafted";
      if(building.isBlocked) return "isBlocked";
      };
      return "";
    }

    checkBuildingDraggable(building:DataBuilding){
      let disableDragging = false;

      switch(this.gs.currentRole){
        case RoleName.Builder:
          if(building.quantity == 0) disableDragging = true;
          break;
        case RoleName.Draft:
          if(building.isDrafted) disableDragging = true;
          break;
        default:
          disableDragging = true;
          break;
      }
      return disableDragging 
    }
 
    onDragBuilding(building:DataBuilding){
      if(this.gameService.getBuildingType(building)?.size == 2) this.gameService.islargeBuildingDragging.next(true);
      else return;
    } 

    onDragBuildingEnded(){
      this.gameService.islargeBuildingDragging.next(false);
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