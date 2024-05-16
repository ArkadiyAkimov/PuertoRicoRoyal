import { NgModule, isDevMode, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { RouterModule, ExtraOptions, Routes} from '@angular/router';

import { CoreModule } from './core/core.module';
import { UserModule } from './feature-modules/user/user.module';
import { GameModule } from './feature-modules/game/game.module';

import { AppComponent } from './app.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { GamePageComponent } from './pages/game-page/game-page.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ServiceWorkerModule } from '@angular/service-worker';

import { HttpClientModule } from '@angular/common/http'
import { FormsModule } from '@angular/forms';

const routerOptions: ExtraOptions = {
  anchorScrolling: "enabled"
  //scrollPositionRestoration: "enabled"
};

const routes: Routes = [
  {
    path: "",
    component: AppComponent
  }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginPageComponent,
    GamePageComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    RouterModule,
    RouterModule.forRoot(routes, routerOptions),
    CoreModule,
    UserModule,
    GameModule,
    BrowserAnimationsModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: !isDevMode(),
      // Register the ServiceWorker as soon as the application is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    })
  ],
  providers: [],
  bootstrap: [AppComponent],
  schemas:[CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { 
}
