import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../environments/environment';

export const authConfig: AuthConfig = {
  // Url of the Identity Provider
  issuer: environment.issuer,

  // URL of the SPA to redirect the user to after login
  redirectUri: window.location.origin + '/home',

  // The SPA's id. The SPA is registerd with this id at the auth-server
  clientId: environment.clientId,

  // set the scope for the permissions the client should request
  // The first three are defined by OIDC. The 4th is a usecase-specific one
  scope: environment.scope,

  // To also enable single-sign-out set the url for your auth-server's logout-endpoint here
  logoutUrl: environment.issuer + '/connect/endsession?id_token={{id_token}}',
};
