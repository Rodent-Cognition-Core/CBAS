import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Mapping_SessionInfoComponent } from './mapping_SessionInfo.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: Mapping_SessionInfoComponent, pathMatch: 'full', canActivate: [AuthGuard] }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class Mapping_SessionInfoRoutingModule { }
