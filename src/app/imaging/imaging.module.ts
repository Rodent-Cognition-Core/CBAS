import { NgModule } from '@angular/core';
import { ImagingRoutingModule } from './imaging-routing.module';
import { SharedModule } from '../shared/shared.module';
//import { MatSelectModule } from '@angular/material/select';
import { ImagingComponent } from './imaging.component';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
//import { FormControl, Validators } from '@angular/forms';
//import { ExperimentService } from '../services/experiment.service';
import { AuthenticationService } from '../services/authentication.service';
import { TaskAnalysisService } from '../services/taskanalysis.service';
//import { ImagingService } from '../services/imaging.service'
//import { MatDialogModule, MatButtonModule } from '@angular/material';
//import { UploadService } from '../services/upload.service';
//import { PostProcessingQcService } from '../services/postprocessingqc.service';
//import { PagerService } from '../services/pager.service';
//import { OrderModule } from 'ngx-order-pipe';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        ImagingRoutingModule,
        SharedModule,
        //MatSelectModule,
        

    ],
    declarations: [
        ImagingComponent,
        
    ],
    providers: [
        AuthenticationService,
        TaskAnalysisService,
        //ImagingService,
        PagerService,
        
    ],
    bootstrap: [ImagingComponent],
    

})
export class ImagingModule { } 

