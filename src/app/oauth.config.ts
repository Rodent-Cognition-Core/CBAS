import { Injectable } from '@angular/core';
import { AuthConfig, OAuthService } from 'angular-oauth2-oidc';

import { AuthenticationService } from './services/authentication.service';

import { environment } from '../environments/environment';

export const oAuthDevelopmentConfig: AuthConfig = {

    clientId: "AngularCBAS",
    scope: "openid offline_access WebAPI profile roles",
    oidc: false,
    issuer: "https://staging.mousebytes.ca",
    requireHttps: false,
    waitForTokenInMsec: 0

}

export const oAuthProductionConfig: AuthConfig = {

    clientId: "AngularCBAS",
    scope: "openid offline_access WebAPI profile roles",
    oidc: false,
    issuer: "https://staging.mousebytes.ca",
    requireHttps: false,
    waitForTokenInMsec: 0

}

/**
 * angular-oauth2-oidc configuration.
 */
@Injectable({
    providedIn: 'root',
})
export class OAuthConfig {
    constructor(private oAuthService: OAuthService,
                private authenticationService: AuthenticationService
    ) { }

    load(): Promise<void> {
        const config = environment.production
            ? oAuthProductionConfig
            : oAuthDevelopmentConfig;

        // Configure OAuthService
        this.oAuthService.configure(config);

        // Set storage (e.g., localStorage)
        this.oAuthService.setStorage(localStorage);

        // Load discovery document and try login
        return this.oAuthService
            .loadDiscoveryDocumentAndTryLogin()
            .then(() => {
                // Additional logic after successful configuration, if needed
                this.authenticationService.startupTokenRefresh();
                console.log('OAuth configuration loaded successfully');
            })
            .catch((error) => {
                console.error('Error loading OAuth configuration', error);
                throw error;
            });
    }
}
