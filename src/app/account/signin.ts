import { Injectable } from '@angular/core';

import { Router } from '@angular/router';

import { OAuthService } from 'angular-oauth2-oidc';

import { AuthenticationService } from '../services/authentication.service';

import { IdentityService } from '../services/identity.service';

import { ManageUserService } from '../services/manageuser.service';

/**
 * Provides signin method to signin & signup components.
 */
@Injectable()
export class Signin {

  model: any = {};

  errorMessages: any[] = [];

  isEmailApproved: boolean;
  isTermsConfirmed: boolean;

  constructor(
    protected router: Router,
    protected oAuthService: OAuthService,
    protected authenticationService: AuthenticationService,
    protected manageuserService: ManageUserService) {

    this.isEmailApproved = false;
    this.isTermsConfirmed = false;
  }

  signin(): void {
    this.oAuthService
      .fetchTokenUsingPasswordFlowAndLoadUserProfile(this.model.username, this.model.password)
      .then(() => {
        this.authenticationService.init();

        // Strategy for refresh token through a scheduler.
        this.authenticationService.scheduleRefresh();

        // Gets the redirect URL from authentication service.
        // If no redirect has been set, uses the default.
        const redirect: string = this.authenticationService.redirectUrl
          ? this.authenticationService.redirectUrl
          : '/home';
        // Redirects the user.
        this.router.navigate([redirect]);


      })
      .catch((error: any) => {


        this.errorMessages = [];

        // Checks for error in response (error from the Token endpoint).
        if (error.body !== '') {
          const body: any = error; // .json();
          switch (body.error.error) {
            case 'invalid_grant':

              // call another api and find out if the user is not approved.
              this.manageuserService.getEmailApprovalAndUserLockedStatus(this.model.username).subscribe((data: any) => {
                this.isEmailApproved = data.isUserApproved;

                if (this.isEmailApproved) {
                  this.errorMessages.push({ description: '- Invalid email or password.' });
                } else {
                  this.errorMessages.push({ description:
                      '- Your account has been created and needs to be approved by the Administrator.' +
                      'You will receive an email once your account is approved.' });
                }

                if (data.isUserLocked) {
                  this.errorMessages.push({
                    description:
                          '- Your account has been locked due to multiple login attemps.' +
                          'Please contact administrator at MouseBytes@uwo.ca or try again after 1 hour.'
                  });
                }

              });


              break;
            default:
              this.errorMessages.push({ description: 'Unexpected error. Try again.' });
          }
        } else {
          const errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
          // console.log(errMsg);
          this.errorMessages.push({ description: 'Server error. Try later.' });
        }
      });
  }

  clearMessages(): void {
    this.errorMessages = [];
  }

}
