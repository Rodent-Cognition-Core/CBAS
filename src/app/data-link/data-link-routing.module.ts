import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DataLinkComponent } from './data-link.component';
// import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: DataLinkComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DataLinkRoutingModule { }

