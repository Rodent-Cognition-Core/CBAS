import { NgModule } from '@angular/core';
import { GuidelineRoutingModule } from './guideline-routing.module';
import { SharedModule } from '../shared/shared.module';
import { GuidelineComponent } from './guideline.component';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        GuidelineRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        GuidelineComponent,
        
    ],
    providers: [
        TaskAnalysisService,
        PagerService,
        
    ],
    bootstrap: [GuidelineComponent],

    exports: [GuidelineComponent],
})
export class GuidelineModule { } 

