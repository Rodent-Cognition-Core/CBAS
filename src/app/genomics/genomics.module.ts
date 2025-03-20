import { NgModule } from '@angular/core';
import { GenomicsRoutingModule } from './genomics-routing.module';
import { SharedModule } from '../shared/shared.module';
import { GenomicsComponent } from './genomics.component';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        GenomicsRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        GenomicsComponent,
        
    ],
    providers: [
        TaskAnalysisService,
        PagerService,
        
    ],
    bootstrap: [GenomicsComponent],
    

})
export class GenomicsModule { } 

