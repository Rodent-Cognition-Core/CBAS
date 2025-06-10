import { NgModule } from '@angular/core';
import { DownloadDsRoutingModule } from './download-ds-routing.module';
import { SharedModule } from '../shared/shared.module';
import { DownloadDsComponent } from './download-ds.component';
import { SearchExperimentService } from '../services/searchexperiment.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        DownloadDsRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        DownloadDsComponent,
        
    ],
    providers: [
        PagerService,
        SearchExperimentService,
        
    ],
    bootstrap: [DownloadDsComponent],
    

})
export class DownloadDsModule { } 

