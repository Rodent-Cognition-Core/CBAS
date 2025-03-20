import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable ,  BehaviorSubject, Subscription, timer } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';
import { User } from '../models/user';

/**
 * ROPC Authentication service.
 */
@Injectable({ 
    providedIn: 'root',
 }) export class AuthenticationService {

    // As in OAuthConfig.
    public storage: Storage = localStorage;

    /**
     * Stores the URL so we can redirect after signing in.
     */
    public redirectUrl: string;

    /**
     * Behavior subjects of the user's status & data.
     */
    private _user: User = { Email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' }

    /**
     * Scheduling of the refresh token.
     */
    private refreshSubscription: Subscription | undefined;

    /**
     * Offset for the scheduling to avoid the inconsistency of data on the client.
     */
    private offsetSeconds: number = 60;

    private signinStatus = new BehaviorSubject<boolean>(false);

    public signinStatus$ = this.signinStatus.asObservable();

    private user = new BehaviorSubject<User>(this._user);

    public user$ = this.user.asObservable();

    constructor(
        private router: Router,
        private oAuthService: OAuthService
    ) {
        this.redirectUrl = "";
    }

    public init(): void {
        // Tells all the subscribers about the new status & data.
        this.signinStatus.next(true);
        this.user.next(this.getUser());
    }

    public signout(): void {
        this.oAuthService.logOut(true);

        this.redirectUrl = "";

        // Tells all the subscribers about the new status & data.
        this.signinStatus.next(false);
        this._user = { Email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' };
        this.user.next(this._user);

        // Unschedules the refresh token.
        this.unscheduleRefresh();
    }

    public getAuthorizationHeader(): HttpHeaders {
        // Creates header for the auth requests.
        let headers: HttpHeaders = new HttpHeaders().set('Content-Type', 'application/json');
        headers = headers.append('Accept', 'application/json');

        const token: string = this.oAuthService.getAccessToken();
        if (token) {
            headers = headers.set('Authorization', `Bearer ${token}`);
        }
        return headers;
    }

    public isSignedIn(): Observable<boolean> {
        return this.signinStatus.asObservable();
    }

    public userChanged(): Observable<User> {
        return this.user.asObservable();
    }

    public isInRole(role: string): boolean {
        const user: User = this.getUser();
        const roles: string[] = user && typeof user.roles !== "undefined" ? user.roles : [];
        return roles.indexOf(role) != -1;
    }

    public getUser(): User {
        const user: User = this._user;
        if (this.oAuthService.hasValidAccessToken()) {
            const userInfo: any = this.oAuthService.getIdentityClaims();

            user.givenName = userInfo.given_name;
            user.familyName = userInfo.family_name;
            user.userName = userInfo.name;
            user.roles = userInfo.role;
        }
        return user;
    }

    /**
     * Strategy for refresh token through a scheduler.
     * Will schedule a refresh at the appropriate time.
     */
    public scheduleRefresh(): void {
        this.unscheduleRefresh();
        const expiresAt: number = this.oAuthService.getAccessTokenExpiration();
        const delay = this.calcDelay(expiresAt);
        const source: Observable<number> = timer(delay);

        this.refreshSubscription = source.subscribe(() => {
            this.oAuthService.refreshToken()
                .then(() => {
                    this.scheduleRefresh();
                })
                .catch((_error: any) => {
                    this.handleRefreshTokenError();
                });
        });
    }

    /**
     * Case when the user comes back to the app after closing it.
     */
    public startupTokenRefresh(): void {
        if (this.oAuthService.hasValidAccessToken()) {
            this.scheduleRefresh();
        }
    }

    /**
     * Unsubscribes from the scheduling of the refresh token.
     */
    private unscheduleRefresh(): void {
        if (this.refreshSubscription) {
            this.refreshSubscription.unsubscribe();
        }
    }

    /**
     * Handles errors on refresh token, like expiration.
     */
    private handleRefreshTokenError(): void {
        this.redirectUrl = this.router.url;

        // Tells all the subscribers about the new status & data.
        this.signinStatus.next(false);
        this.user.next(this._user);

        // Unschedules the refresh token.
        this.unscheduleRefresh();

        // The user is forced to sign in again.
        this.router.navigate(['/account/signin']);
    }

    private calcDelay(time: number): number {
        const now = new Date().valueOf();
        const delay: number = time - now - (this.offsetSeconds * 1000);
        return delay > 0 ? delay : 1000;
    }

    private getAuthTime(): number {
        const storageitem: any = this.storage.getItem('access_token_stored_at');
        if(storageitem != null) {
            return parseInt(storageitem, 10);
        } else {
            return 0;
        }

    }

}