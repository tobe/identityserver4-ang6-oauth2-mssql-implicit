import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { ProtectedAreaComponent } from './components/protected-area/protected-area.component';
import { HttpClientModule } from '../../node_modules/@angular/common/http';
import { OAuthModule } from '../../node_modules/angular-oauth2-oidc';
import { environment } from '../environments/environment';

import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { AuthGuard } from './services/auth.guard';

@NgModule({
  declarations: [
    AppComponent,
    ProtectedAreaComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,

    HttpClientModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: [environment.apiUrl],
        sendAccessToken: true
      }
    }),
    RouterModule.forRoot(
      [
        {
          path: 'protected-area',
          component: ProtectedAreaComponent,
          canActivate: [AuthGuard],
        },
        { path: 'home', component: HomeComponent },
        { path: '', redirectTo: '/home', pathMatch: 'full' }
      ]
    )
  ],
  providers: [AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }
