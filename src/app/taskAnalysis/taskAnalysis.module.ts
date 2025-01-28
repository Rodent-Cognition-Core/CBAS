import { NgModule } from '@angular/core';

import { TaskAnalysisRoutingModule } from './taskAnalysis-routing.module';
import { SharedModule } from '../shared/shared.module';

import { TaskAnalysisService } from '../services/taskanalysis.service';

import { TaskAnalysisComponent } from './taskAnalysis.component';

// import { SharedExperimentComponent } from '../shared-experiment/shared-experiment.component';

@NgModule({
  imports: [
  TaskAnalysisRoutingModule,
  SharedModule
  ],
  declarations: [
  TaskAnalysisComponent,
// SharedExperimentComponent

  ],
  providers: [
  TaskAnalysisService
  ]
  })
export class TaskAnalysisModule { }
