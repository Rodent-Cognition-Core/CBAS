import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TaskAnalysisComponent } from './taskAnalysis.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: TaskAnalysisComponent, pathMatch: 'full', canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TaskAnalysisRoutingModule { }
