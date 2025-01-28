import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CogbytesComponent } from './cogbytes.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: CogbytesComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
  })
export class CogbytesRoutingModule { }
