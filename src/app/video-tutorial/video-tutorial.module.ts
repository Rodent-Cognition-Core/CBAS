import { NgModule } from '@angular/core';
import { VideoTutorialRoutingModule } from './video-tutorial-routing.module';
import { SharedModule } from '../shared/shared.module';
// import { MatSelectModule } from '@angular/material/select';
import { VideoTutorialComponent } from './video-tutorial.component';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { FormControl, Validators } from '@angular/forms';

import { AuthenticationService } from '../services/authentication.service';
import { TaskAnalysisService } from '../services/taskanalysis.service';

import { PagerService } from '../services/pager.service';


@NgModule({
  imports: [
  VideoTutorialRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  VideoTutorialComponent,

  ],
  providers: [
  AuthenticationService,
  TaskAnalysisService,
// GenomicsService,
  PagerService,

  ],
  bootstrap: [VideoTutorialComponent],

  exports: [VideoTutorialComponent],
  })
export class VideoTutorialModule { }

