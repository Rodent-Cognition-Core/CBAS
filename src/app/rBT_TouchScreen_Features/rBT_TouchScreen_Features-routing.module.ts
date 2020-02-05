import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { RBT_TouchScreen_FeaturesComponent } from './rBT_TouchScreen_Features.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: RBT_TouchScreen_FeaturesComponent, pathMatch: 'full', canActivate: [AuthGuard] }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RBT_TouchScreen_FeaturesRoutingModule { }
