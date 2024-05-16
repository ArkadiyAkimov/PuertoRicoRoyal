import { NgModule, Optional, SkipSelf} from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    SharedModule,
  ],
  providers:[],
  exports:[],
})
export class CoreModule { 
  constructor(@Optional() @SkipSelf() parentComponent:CoreModule){
    if(parentComponent){
      throw new Error('CoreModule has already been importend into AppModule');
    }
  }
}
