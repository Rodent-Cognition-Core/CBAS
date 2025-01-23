import { Component } from '@angular/core';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
//import { FormControl } from '@angular/forms';
//import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';

import { OAuthService } from 'angular-oauth2-oidc';

import { AuthenticationService } from '../../services/authentication.service';
import { IdentityService } from '../../services/identity.service';
import { Signin } from '../signin';
//import { PISiteService } from '../../services/piSite.service';
import { ManageUserService } from '../../services/manageuser.service';

@Component({
    templateUrl: './reset.component.html',
    styleUrls: ['../account.scss'],

})
export class ResetComponent extends Signin {

    model: any = {};
    userID: string;
    token: string;
    showSuccess = false;
    //selectedPiSiteIds: any;
    //PISite = new FormControl();

    constructor(
        protected router: Router,
        private route: ActivatedRoute,
        protected oAuthService: OAuthService,
        protected authenticationService: AuthenticationService,
        private identityService: IdentityService,
        protected manageuserService: ManageUserService) {

        super(router, oAuthService, authenticationService, manageuserService);
        this.userID = '';
        this.token = '';

        this.route.queryParams.subscribe(params => {
            this.userID = params['userId'];
            this.token = params['code'];

        });
    }

    reset(): void {


        this.identityService.submitNewPass(this.model.username, this.token, this.model.password).subscribe(
            (res: any) => {
                // IdentityResult.
                if (res == 'Invalid Email!') {
                    this.errorMessages.push({ description: "Invalid Email!" });
                } else {
                    if (res.succeeded) {
                        this.showSuccess = true;
                    } else {
                        this.errorMessages = res.errors;
                    }
                }

            },
            (error: any) => {
                const errMsg = (error.message) ? error.message :
                    error.status ? `${error.status} - ${error.statusText}` : "Server error";
                //console.log(errMsg);
                this.errorMessages.push({ description: "Server error. Try later." });
            });


    }

}
