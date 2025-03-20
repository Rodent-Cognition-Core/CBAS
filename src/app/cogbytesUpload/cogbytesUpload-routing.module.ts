import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CogbytesUploadComponent } from './cogbytesUpload.component';


const routes: Routes = [
    { path: '', component: CogbytesUploadComponent, pathMatch: 'full'}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CogbytesUploadRoutingModule { } 
