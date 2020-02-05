import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ImagingComponent } from './imaging.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: ImagingComponent, pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ImagingRoutingModule { } 

