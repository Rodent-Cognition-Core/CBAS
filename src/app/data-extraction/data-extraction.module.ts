import { NgModule } from '@angular/core';
import { DataExtractionRoutingModule } from './data-extraction-routing.module';
import { SharedModule } from '../shared/shared.module';
import { DataExtractionComponent } from './data-extraction.component';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { DataExtractionService } from '../services/dataextraction.service'
import { PagerService } from '../services/pager.service';
import { UploadService } from '../services/upload.service';
import { ExpDialogeService } from '../services/expdialoge.service';

@NgModule({
    imports: [
        DataExtractionRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        DataExtractionComponent,
        
    ],
    providers: [
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

