import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PubScreenEditComponent } from './pubScreenEdit.component';

const routes: Routes = [
    { path: '', component: PubScreenEditComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PubScreenEditRoutingModule { } 

