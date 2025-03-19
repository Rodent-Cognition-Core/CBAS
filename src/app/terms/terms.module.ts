import { NgModule } from '@angular/core';
import { TermsRoutingModule } from './terms-routing.module';
import { SharedModule } from '../shared/shared.module';
import { TermsComponent } from './terms.component';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        TermsRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        TermsComponent,
        
    ],
    providers: [
        TaskAnalysisService,
        PagerService,
        
    ],
    bootstrap: [TermsComponent],

    exports: [TermsComponent],

})
export class TermsModule { } 

