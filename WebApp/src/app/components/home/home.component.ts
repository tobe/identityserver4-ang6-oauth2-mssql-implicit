import { Component, OnInit } from '@angular/core';
import { OAuthService } from '../../../../node_modules/angular-oauth2-oidc';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    private _oauthService: OAuthService,
    private _userService: UserService
  ) { }

  ngOnInit() {
  }

  public isUserLoggedIn() {
    return this._userService.isUserLoggedIn();
  }

  onLogin() {
    this._oauthService.initImplicitFlow();
  }

  onLogout() {
    this._oauthService.logOut();
  }
}
