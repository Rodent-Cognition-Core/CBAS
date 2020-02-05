import { NgModule } from '@angular/core';
import { DataLinkRoutingModule } from './data-link-routing.module';
import { SharedModule } from '../shared/shared.module';
import { DataLinkComponent } from './data-link.component';
import { DataExtractionService } from '../services/dataextraction.service'
import { PagerService } from '../services/pager.service';

@NgModule({
    imports: [
        DataLinkRoutingModule,
        SharedModule,
        //MatSelectModule,
        

    ],
    declarations: [
        DataLinkComponent,
        
    ],
    providers: [
        DataExtractionService,
        PagerService,
        
    ],
    bootstrap: [DataLinkComponent],
    

})
export class DataLinkModule { } 

