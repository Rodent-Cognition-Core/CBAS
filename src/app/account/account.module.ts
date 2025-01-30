import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '../shared/shared.module';

import { SigninComponent } from './signin/signin.component';
import { SignupComponent } from './signup/signup.component';
import { ForgotComponent } from './forgot/forgot.component';
import { ResetComponent } from './reset/reset.component';

@NgModule({
  imports: [
  AccountRoutingModule,
  SharedModule,
  FormsModule,
  ReactiveFormsModule

  ],
  declarations: [
  SigninComponent,
  SignupComponent,
  ForgotComponent,
  ResetComponent
  ]
  })
export class AccountModule { }
