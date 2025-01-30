import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DataVisualizationComponent } from './data-visualization.component';
// import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: DataVisualizationComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
  })
export class DataVisualizationRoutingModule { }

