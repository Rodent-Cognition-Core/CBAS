import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PubScreenSearchComponent } from './pubScreen-search.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: PubScreenSearchComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PubScreenSearchRoutingModule { }

