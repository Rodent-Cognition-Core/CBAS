import { NgModule } from '@angular/core';

import { SearchExperimentRoutingModule } from './search-experiment-routing.module';
import { SharedModule } from '../shared/shared.module';

import { SearchExperimentService } from '../services/searchexperiment.service';

import { SearchExperimentComponent } from './search-experiment.component';

import { PagerService } from '../services/pager.service';

// import { SharedExperimentComponent } from '../shared-experiment/shared-experiment.component';

@NgModule({
  imports: [
    SearchExperimentRoutingModule,
    SharedModule
  ],
  declarations: [
    SearchExperimentComponent,
    // SharedExperimentComponent

  ],
  providers: [
    SearchExperimentService, PagerService,
  ],

  exports: [SearchExperimentComponent]
})
export class SearchExperimentModule { }
