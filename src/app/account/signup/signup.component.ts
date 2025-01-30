import { Component } from '@angular/core';
import { Router } from '@angular/router';
// import { FormControl } from '@angular/forms';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import {
    MatDialog, MatDialogRef,
    MAT_DIALOG_DATA
} from '@angular/material/dialog';
import { OAuthService } from 'angular-oauth2-oidc';

import { AuthenticationService } from '../../services/authentication.service';
import { IdentityService } from '../../services/identity.service';
import { Signin } from '../signin';
import { PISiteService } from '../../services/piSite.service';
import { ManageUserService } from '../../services/manageuser.service';
import { TermsDialogeComponent } from '../../termsDialoge/termsDialoge.component';

@Component({
  templateUrl: './signup.component.html',
  styleUrls: ['../account.scss'],

  })
export class SignupComponent extends Signin {

  piSiteList: any;
  // selectedPiSiteIds: any;
  // PISite = new FormControl();

  constructor(
    protected router: Router,
    protected oAuthService: OAuthService,
    protected authenticationService: AuthenticationService,
    private identityService: IdentityService,
    private piSiteService: PISiteService,
    public dialogTerms: MatDialog,
    protected manageuserService: ManageUserService) {
    super(router, oAuthService, authenticationService, manageuserService);
  }

  // Get the list of PISite
  getPISiteList() {

    this.piSiteService.getPISite().subscribe((data) => {
      this.piSiteList = data.piSite;
      // console.log(this.piSiteList);
    });

  }

  //
  openDialogTerms(): void {

    const dialogref = this.dialogTerms.open(TermsDialogeComponent, {
      // height: '700px',
      // width: '900px',
      data: {}

    });

    dialogref.afterClosed().subscribe();
  }

  signup(): void {

    // Check to see if terms and condtion was approved by user
    if (!this.model.termsChecked) {
      this.errorMessages.push({ description: 'Please read the terms and conditions and approve it by checking the checkbox' });
    } else {


      this.identityService.create(this.model)
        .subscribe(
          (res: any) => {
            // IdentityResult.
            if (res.succeeded) {
              // Signs in the user.
              this.signin();
            } else {
              this.errorMessages = res.errors;
            }
          },
          (error: any) => {
            const errMsg = (error.message) ? error.message :
              error.status ? `${error.status} - ${error.statusText}` : 'Server error';
            // console.log(errMsg);
            this.errorMessages.push({ description: 'Server error. Try later.' });
          });

    }


  }

  ngOnInit() {
    this.getPISiteList();

  }



}
