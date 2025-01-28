import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { OAuthService } from 'angular-oauth2-oidc';

import { AuthenticationService } from '../../services/authentication.service';
import { Signin } from '../signin';

import { ManageUserService } from '../../services/manageuser.service';

@Component({
  templateUrl: './signin.component.html',
  styleUrls: ['../account.scss'],

  })
export class SigninComponent extends Signin {

  constructor(
    protected router: Router,
    protected oAuthService: OAuthService,
    protected authenticationService: AuthenticationService,
    protected manageuserService: ManageUserService) {
    super(router, oAuthService, authenticationService, manageuserService);
  }

}
