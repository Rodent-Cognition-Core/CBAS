import { NgModule } from '@angular/core';
import { GuidelineDataLabRoutingModule } from './guidelineDataLab-routing.module';
import { SharedModule } from '../shared/shared.module';
import { GuidelineDataLabComponent } from './guidelineDataLab.component';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        GuidelineDataLabRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        GuidelineDataLabComponent,
        
    ],
    providers: [
        TaskAnalysisService,
        PagerService,
        
    ],
    bootstrap: [GuidelineDataLabComponent],
    

})
export class GuidelineDataLabModule { } 

