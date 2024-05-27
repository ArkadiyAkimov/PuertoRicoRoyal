import { UserModule } from './../user/user.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


import { SharedModule } from 'src/app/shared/shared.module';
import { BuildingsComponent } from './components/buildings/buildings.component';
import { PlantationsComponent } from './components/plantations/plantations.component';
import { CargoShipsComponent } from './components/cargo-ships/cargo-ships.component';
import { RolesComponent } from './components/roles/roles.component';
import { OpponentBoardsComponent } from './components/opponent-boards/opponent-boards.component';
import { MyControlsComponent } from './components/my-controls/my-controls.component';
import { UtilityDropComponent } from './components/utility-drop/utility-drop.component';
import { FullSizeComponent } from './components/full-size/full-size.component';
import { ScrollAsistComponent } from './components/scroll-asist/scroll-asist.component';
import { TradingHouseComponent } from './components/trading-house/trading-house.component';
import { CargoShipComponent } from './components/cargo-ship/cargo-ship.component';
import { ScoreBoardComponent } from './components/score-board/score-board.component';
import { SupplyDisplayComponent } from './components/supply-display/supply-display.component';
import { ProgressBarComponent } from './components/progress-bar/progress-bar.component';



@NgModule({
  declarations: [
    BuildingsComponent,
    PlantationsComponent,
    CargoShipsComponent,
    RolesComponent,
    OpponentBoardsComponent,
    MyControlsComponent,
    UtilityDropComponent,
    FullSizeComponent,
    ScrollAsistComponent,
    TradingHouseComponent,
    CargoShipComponent,
    ScoreBoardComponent,
    SupplyDisplayComponent,
    ProgressBarComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    UserModule
  ],
  providers:[],
  exports:[
    BuildingsComponent,
    PlantationsComponent,
    CargoShipsComponent,
    RolesComponent,
    OpponentBoardsComponent,
    MyControlsComponent,
    UtilityDropComponent,
    FullSizeComponent,
    ScrollAsistComponent,
    ScoreBoardComponent,
    SupplyDisplayComponent,
    ProgressBarComponent,
  ],
})
export class GameModule { }
