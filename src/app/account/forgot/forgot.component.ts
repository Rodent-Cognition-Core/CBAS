import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { AuthenticationService } from '../../services/authentication.service';
import { IdentityService } from '../../services/identity.service';
import { Signin } from '../signin';
import { ManageUserService } from '../../services/manageuser.service';
import { map, catchError } from 'rxjs/operators';

@Component({
    templateUrl: './forgot.component.html',
    styleUrls: ['../account.scss'],

})
export class ForgotComponent extends Signin {

    model: any = {};
    showCheckYourEmail = false;
    //selectedPiSiteIds: any;
    //PISite = new FormControl();

    constructor(
        protected router: Router,
        protected oAuthService: OAuthService,
        protected authenticationService: AuthenticationService,
        private identityService: IdentityService,
        protected manageuserService: ManageUserService) {
        super(router, oAuthService, authenticationService, manageuserService);
    }

    forgot(): void {

        this.identityService.generatePasswordResetToken(this.model.username)
            .pipe(
                map(() => {
                    this.showCheckYourEmail = true;
                }),
                catchError((error) => {
                    console.error('Error processing password reset request', error);
                    return [];
                })
            )
            .subscribe();

    }

}
