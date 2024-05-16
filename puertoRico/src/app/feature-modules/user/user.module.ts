import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from 'src/app/shared/shared.module';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { ProfileComponent } from './components/profile/profile.component';

@NgModule({
  declarations: [
    RegisterComponent,
       LoginComponent,
       ProfileComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
  ],
  providers:[],
  exports:[
    RegisterComponent,
    LoginComponent,
    ProfileComponent,
  ],
})
export class UserModule { }
