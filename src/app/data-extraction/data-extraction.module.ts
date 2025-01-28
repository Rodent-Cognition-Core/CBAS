import { NgModule } from '@angular/core';
import { DataExtractionRoutingModule } from './data-extraction-routing.module';
import { SharedModule } from '../shared/shared.module';
// import { MatSelectModule } from '@angular/material/select';
import { DataExtractionComponent } from './data-extraction.component';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { FormControl, Validators } from '@angular/forms';
// import { ExperimentService } from '../services/experiment.service';
import { AuthenticationService } from '../services/authentication.service';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { DataExtractionService } from '../services/dataextraction.service';
// import { MatDialogModule, MatButtonModule } from '@angular/material';
// import { UploadService } from '../services/upload.service';
// import { PostProcessingQcService } from '../services/postprocessingqc.service';
// import { PagerService } from '../services/pager.service';
// import { OrderModule } from 'ngx-order-pipe';
import { PagerService } from '../services/pager.service';
import { UploadService } from '../services/upload.service';
import { ExpDialogeService } from '../services/expdialoge.service';

@NgModule({
  imports: [
  DataExtractionRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  DataExtractionComponent,

  ],
  providers: [
  AuthenticationService,
  TaskAnalysisService,
  DataExtractionService,
  PagerService,
  UploadService,
  ExpDialogeService,

  ],
  bootstrap: [DataExtractionComponent],

  exports: [DataExtractionComponent]
  })
export class DataExtractionModule { }

