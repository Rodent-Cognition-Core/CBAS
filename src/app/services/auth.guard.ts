import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { map, concatMap } from 'rxjs/operators';

import { AuthenticationService } from './authentication.service';

/**
 * Decides if a route can be activated.
 */
@Injectable() export class AuthGuard {

    private signedIn: boolean;

    constructor(private authenticationService: AuthenticationService, private router: Router) {
        this.signedIn = false;
    }

    public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean {
        return this.authenticationService.isSignedIn().pipe(
            map((signedIn: boolean) => { this.signedIn = signedIn; }),
            concatMap(() => this.authenticationService.userChanged().pipe(
                map(() => {
                    const url: string = state.url;

                    if (this.signedIn) {
                        if (url !== "/dashboard" && url !== "/taskAnalysis" && url !== "/upload" && url !== "/animal-info" && url !== "/experiment") {
                            return true;
                        } else {
                            if (this.authenticationService.isInRole("administrator") || this.authenticationService.isInRole("user")) {
                                return true;
                            } else {
                                this.router.navigate(['/home']);
                                return false;
                            }
                        }
                    }

                    // Stores the attempted URL for redirecting.
                    this.authenticationService.redirectUrl = url;

                    //if (url.startsWith("/link")) {
                    //    return true;
                    //}

                    // Not signed in so redirects to signin page.
                    this.router.navigate(['/account/signin']);
                    return false;
                })
            ))
        );
    }

}
