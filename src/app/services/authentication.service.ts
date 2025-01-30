import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable ,  BehaviorSubject } from 'rxjs';
import { interval } from 'rxjs';
import { timer } from 'rxjs';

import { OAuthService } from 'angular-oauth2-oidc';

import { User } from '../models/user';

/**
 * ROPC Authentication service.
 */
@Injectable() export class AuthenticationService {

  // As in OAuthConfig.
  public storage: Storage = localStorage;

  /**
     * Stores the URL so we can redirect after signing in.
     */
  public redirectUrl: string;

  public signinStatus$: Observable<boolean>;

  public user$: Observable<User>;

  /**
     * Behavior subjects of the user's status & data.
     */
  private _user: User = { email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' }
    ;
    // private signinStatus = new BehaviorSubject<boolean>(false);
    // private user = new BehaviorSubject<User>(this._user);



  /**
     * Scheduling of the refresh token.
     */
  private refreshSubscription: any;

  /**
     * Offset for the scheduling to avoid the inconsistency of data on the client.
     */
  private offsetSeconds = 30;

  private signinStatus = new BehaviorSubject<boolean>(false);

  private user = new BehaviorSubject<User>(this._user);

  constructor(
    private router: Router,
    private oAuthService: OAuthService
  ) {
    this.redirectUrl = '';
    this.signinStatus$ = this.signinStatus.asObservable();

    this.user$ = this.user.asObservable();
    // this._user = { Email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' }
  }

  public init(): void {
    // Tells all the subscribers about the new status & data.
    this.signinStatus.next(true);
    this.user.next(this.getUser());
  }

  public signout(): void {
    this.oAuthService.logOut(true);

    this.redirectUrl = '';

    // Tells all the subscribers about the new status & data.
    this.signinStatus.next(false);
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
    const roles: string[] = user && typeof user.roles !== 'undefined' ? user.roles : [];
    return roles.indexOf(role) !== -1;
  }

  public getUser(): User {
    const user: User = { email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' };
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
    const source: Observable<number> = interval(
      this.calcDelay(this.getAuthTime())
    );

    this.refreshSubscription = source.subscribe(() => {
      this.oAuthService.refreshToken()
        .then(() => {
          // Scheduler works.
        })
        .catch((error: any) => {
          this.handleRefreshTokenError();
        });
    });
  }

  /**
     * Case when the user comes back to the app after closing it.
     */
  public startupTokenRefresh(): void {
    if (this.oAuthService.hasValidAccessToken()) {
      const source: Observable<number> = timer(this.calcDelay(new Date().valueOf()));

      // Once the delay time from above is reached, gets a new access token and schedules additional refreshes.
      source.subscribe(() => {
        this.oAuthService.refreshToken()
          .then(() => {
            this.scheduleRefresh();
          })
          .catch((error: any) => {
            this.handleRefreshTokenError();
          });
      });
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
    const expiresAt: number = this.oAuthService.getAccessTokenExpiration();
    const delay: number = expiresAt - time - this.offsetSeconds * 1000;
    return delay > 0 ? delay : 0;
  }

  private getAuthTime(): number {
    const storageitem: any = this.storage.getItem('access_token_stored_at');
    if (storageitem != null) {
      return parseInt(storageitem, 10);
    } else {
      return 0;
    }
  }

}
