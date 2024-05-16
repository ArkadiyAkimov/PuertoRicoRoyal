import { LoginPageComponent } from './pages/login-page/login-page.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GamePageComponent } from './pages/game-page/game-page.component';

const routes: Routes = [
  { path: '',component: LoginPageComponent },
  { path: 'game',component: GamePageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
