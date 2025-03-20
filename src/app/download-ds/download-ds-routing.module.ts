import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DownloadDsComponent } from './download-ds.component';


const routes: Routes = [
    { path: '', component: DownloadDsComponent, pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DownloadDsRoutingModule { } 

