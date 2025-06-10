import { NgModule } from '@angular/core';
import { TaskAnalysisRoutingModule } from './taskAnalysis-routing.module';
import { SharedModule } from '../shared/shared.module';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { TaskAnalysisComponent } from './taskAnalysis.component';


@NgModule({
    imports: [
        TaskAnalysisRoutingModule,
        SharedModule
    ],
    declarations: [
        TaskAnalysisComponent,

    ],
    providers: [
        TaskAnalysisService
    ]
})
export class TaskAnalysisModule { }
