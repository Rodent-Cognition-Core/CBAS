import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { GenomicsComponent } from './genomics.component';

import { AuthGuard } from '../services/auth.guard';

const routes: Routes = [
    { path: '', component: GenomicsComponent, pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GenomicsRoutingModule { } 

