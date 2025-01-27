import { NgModule } from '@angular/core';
import { GenomicsRoutingModule } from './genomics-routing.module';
import { SharedModule } from '../shared/shared.module';
// import { MatSelectModule } from '@angular/material/select';
import { GenomicsComponent } from './genomics.component';
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
    GenomicsRoutingModule,
    SharedModule,
    // MatSelectModule,


  ],
  declarations: [
    GenomicsComponent,

  ],
  providers: [
    AuthenticationService,
    TaskAnalysisService,
    // GenomicsService,
    PagerService,

  ],
  bootstrap: [GenomicsComponent],


})
export class GenomicsModule { }

