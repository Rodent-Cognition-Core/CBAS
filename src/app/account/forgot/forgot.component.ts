import { Component } from '@angular/core';
import { Router } from '@angular/router';
//import { FormControl } from '@angular/forms';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';

import { OAuthService } from 'angular-oauth2-oidc';

import { AuthenticationService } from '../../services/authentication.service';
import { IdentityService } from '../../services/identity.service';
import { Signin } from '../signin';
import { PISiteService } from '../../services/piSite.service';
import { ManageUserService } from '../../services/manageuser.service';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';

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

        this.identityService.generatePasswordResetToken(this.model.username).map(res => {
            this.showCheckYourEmail = true;


            //if (res == "resetDone!") {
            //    this.showCheckYourEmail = true;
                
            //} else {
            //    return;
            //}

        }).subscribe();

    }

}
