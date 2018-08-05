import { Injectable } from '@angular/core';
import { OAuthService } from '../../../node_modules/angular-oauth2-oidc';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private _oauthService: OAuthService
  ) { }

  isUserLoggedIn(): boolean {
    return this._oauthService.hasValidIdToken() && this._oauthService.hasValidAccessToken();
  }

  getUserClaims(): object {
    return this._oauthService.getIdentityClaims();
  }
}
