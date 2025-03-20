import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VideoTutorialComponent } from './video-tutorial.component';

const routes: Routes = [
    { path: '', component: VideoTutorialComponent, pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VideoTutorialRoutingModule { } 

