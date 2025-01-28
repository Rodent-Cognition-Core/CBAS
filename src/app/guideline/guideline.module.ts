import { NgModule } from '@angular/core';
import { GuidelineRoutingModule } from './guideline-routing.module';
import { SharedModule } from '../shared/shared.module';
// import { MatSelectModule } from '@angular/material/select';
import { GuidelineComponent } from './guideline.component';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { FormControl, Validators } from '@angular/forms';
// import { ExperimentService } from '../services/experiment.service';
import { AuthenticationService } from '../services/authentication.service';
import { TaskAnalysisService } from '../services/taskanalysis.service';
// import { GenomicsService } from '../services/genomics.service'
// import { MatDialogModule, MatButtonModule } from '@angular/material';
// import { UploadService } from '../services/upload.service';
// import { PostProcessingQcService } from '../services/postprocessingqc.service';
// import { PagerService } from '../services/pager.service';
// import { OrderModule } from 'ngx-order-pipe';
import { PagerService } from '../services/pager.service';


@NgModule({
  imports: [
  GuidelineRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  GuidelineComponent,

  ],
  providers: [
  AuthenticationService,
  TaskAnalysisService,
// GenomicsService,
  PagerService,

  ],
  bootstrap: [GuidelineComponent],

  exports: [GuidelineComponent],
  })
export class GuidelineModule { }

