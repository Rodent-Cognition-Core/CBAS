import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MBDashboardComponent } from './mb-dashboard.component';
//import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: MBDashboardComponent, pathMatch: 'full'}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class MBDashboardRoutingModule { }

