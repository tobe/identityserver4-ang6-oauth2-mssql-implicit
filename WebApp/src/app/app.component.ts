import { Component } from '@angular/core';
import { OAuthService, JwksValidationHandler } from '../../node_modules/angular-oauth2-oidc';
import { authConfig } from './auth.config';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: []
})
export class AppComponent {
  title = 'WebApp';

  constructor(
    private _oauthService: OAuthService,
  ) {
    this.configureWithNewConfigApi();
  }

  private configureWithNewConfigApi() {
    this._oauthService.configure(authConfig);
    this._oauthService.tokenValidationHandler = new JwksValidationHandler();
    this._oauthService.loadDiscoveryDocumentAndTryLogin();
  }
}
