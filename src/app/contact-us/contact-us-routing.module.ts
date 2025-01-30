import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ContactUsComponent } from './contact-us.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
  { path: '', component: ContactUsComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
  })
export class ContactUsRoutingModule { }

