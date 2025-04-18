import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CogbytesSearchComponent } from './cogbytesSearch.component';


const routes: Routes = [
    { path: '', component: CogbytesSearchComponent, pathMatch: 'full'}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CogbytesSearchRoutingModule { } 
