import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  issuer: 'https://localhost:5443',
  redirectUri: window.location.origin + '/signin-oidc',
  postLogoutRedirectUri : window.location.origin + '/signin-oidc',
  clientId: 'spaClient',
  responseType: 'code',
  scope: 'openid profile el.manage',
  showDebugInformation: true,
  sessionChecksEnabled: true,
  disableAtHashCheck: true,
};
