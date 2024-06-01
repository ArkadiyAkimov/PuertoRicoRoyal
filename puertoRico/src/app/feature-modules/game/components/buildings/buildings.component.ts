import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { Subscription } from 'rxjs';
import {  DataBuilding, GameStateJson, GoodType, PlayerUtility, RoleName, isAffordable } from '../../classes/general';
import { StylingService } from '../../services/styling.service';
import { HighlightService } from '../../services/highlight.service';
import { SelectionService } from '../../services/selection.service';


@Component({
  selector: 'app-buildings',
  templateUrl: './buildings.component.html',
  styleUrls: 
  [
    './buildings.component.scss',
    '../../../../styles/colors.scss',
    '../../../../styles/buildings.scss'
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
    public stylingService:StylingService,
    public highlightService:HighlightService,
    public selectionService:SelectionService,
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


    checkBuildingDraggable(building:DataBuilding){
      let disableDragging = false;

      if(this.gs.currentPlayerIndex != this.gameService.playerIndex) return true;

      switch(this.gs.currentRole){
        case RoleName.Builder:
          if(building.quantity == 0 ) disableDragging = true;
          else{
          let buildingAffordability = this.gameService.checkPlayerBuildingAffordabilityState(building);

          if(buildingAffordability[0] == isAffordable.Yes) disableDragging = false;
          else if(buildingAffordability[0] == isAffordable.WithBlackMarket){
            if(buildingAffordability[1] <= this.selectionService.getSelectedBlackMarketDiscountValue()) disableDragging = false;
            else disableDragging = true;
          } 
          if(buildingAffordability[0] == isAffordable.Not) disableDragging = true;
          }
          break;
        case RoleName.Draft:
          if(building.isDrafted || building.isBlocked) disableDragging = true;
          break;
        default:
          disableDragging = true;
          break;
      }
      return disableDragging 
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