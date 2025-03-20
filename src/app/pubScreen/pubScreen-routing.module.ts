import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PubScreenComponent } from './pubScreen.component';

const routes: Routes = [
    { path: '', component: PubScreenComponent, pathMatch: 'full'}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PubSCreenRoutingModule { } 
