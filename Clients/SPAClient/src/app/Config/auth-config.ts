import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  issuer: 'https://localhost:44313/',
  redirectUri: window.location.origin + '/signin-oidc',
  postLogoutRedirectUri : window.location.origin + '/signout-callback-oidc',
  clientId: 'spaClient',
  responseType: 'code',
  scope: 'openid profile el.manage vcms.manage',
  showDebugInformation: true,
  sessionChecksEnabled: true,
  disableAtHashCheck: true,
};
