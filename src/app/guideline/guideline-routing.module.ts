import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GuidelineComponent } from './guideline.component';


const routes: Routes = [
    { path: '', component: GuidelineComponent, pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GuidelineRoutingModule { } 

