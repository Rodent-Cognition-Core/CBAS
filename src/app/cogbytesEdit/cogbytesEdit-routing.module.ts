import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CogbytesEditComponent } from './cogbytesEdit.component';

const routes: Routes = [
    { path: '', component: CogbytesEditComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CogbytesEditRoutingModule { } 

