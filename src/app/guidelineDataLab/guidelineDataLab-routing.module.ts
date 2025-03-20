import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GuidelineDataLabComponent } from './guidelineDataLab.component';


const routes: Routes = [
    { path: '', component: GuidelineDataLabComponent, pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GuidelineDataLabRoutingModule { } 

