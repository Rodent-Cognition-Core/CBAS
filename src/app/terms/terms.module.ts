import { NgModule } from '@angular/core';
import { TermsRoutingModule } from './terms-routing.module';
import { SharedModule } from '../shared/shared.module';
// import { MatSelectModule } from '@angular/material/select';
import { TermsComponent } from './terms.component';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { FormControl, Validators } from '@angular/forms';

import { AuthenticationService } from '../services/authentication.service';
import { TaskAnalysisService } from '../services/taskanalysis.service';

import { PagerService } from '../services/pager.service';


@NgModule({
  imports: [
  TermsRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  TermsComponent,

  ],
  providers: [
  AuthenticationService,
  TaskAnalysisService,
// GenomicsService,
  PagerService,

  ],
  bootstrap: [TermsComponent],

  exports: [TermsComponent],

  })
export class TermsModule { }

