import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PubScreenComponent } from './pubScreen.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: PubScreenComponent, pathMatch: 'full', canActivate: [AuthGuard] }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PubSCreenRoutingModule { } 
