import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from './material.module';
import { SharedExperimentComponent } from '../shared-experiment/shared-experiment.component';
import { SharedPubscreenComponent } from '../shared-pubscreen/shared-pubscreen.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

const sharedModules: any[] = [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    NgxMatSelectSearchModule,

];

@NgModule({
    imports: [sharedModules],
    declarations: [
        SharedExperimentComponent, SharedPubscreenComponent
    ],
    exports: [sharedModules, SharedExperimentComponent, SharedPubscreenComponent]
})

export class SharedModule { }