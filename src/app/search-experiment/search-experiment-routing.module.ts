import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SearchExperimentComponent } from './search-experiment.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: SearchExperimentComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
  })
export class SearchExperimentRoutingModule { }
