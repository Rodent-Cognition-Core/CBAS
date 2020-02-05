import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SessionInfoComponent } from './sessionInfo.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: SessionInfoComponent, pathMatch: 'full', canActivate: [AuthGuard] }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SessionInfoRoutingModule { }
