import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ManageUserComponent } from './manage-user.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: ManageUserComponent, pathMatch: 'full', canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
  })
export class ManageUserRoutingModule { }
