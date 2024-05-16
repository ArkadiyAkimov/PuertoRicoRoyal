import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import {DragDropModule} from '@angular/cdk/drag-drop';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    DragDropModule,
    FormsModule
  ],
  providers:[],
  exports:[
    DragDropModule,
    FormsModule
  ],
})
export class SharedModule { }
