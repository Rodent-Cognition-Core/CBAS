import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TermsComponent } from './terms.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: TermsComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
  })
export class TermsRoutingModule { }

