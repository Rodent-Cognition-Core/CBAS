import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DataExtractionComponent } from './data-extraction.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: DataExtractionComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DataExtractionRoutingModule { }

