import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { FormsModule,  FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MaterialModule } from './material.module';
import { SharedExperimentComponent } from '../shared-experiment/shared-experiment.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
//import { OwlModule } from 'ngx-owl-carousel';

const sharedModules: any[] = [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MaterialModule,
    NgxMatSelectSearchModule,
    AngularFontAwesomeModule,
    //OwlModule

];

@NgModule({
    imports: sharedModules,
    exports: [sharedModules, SharedExperimentComponent],
    declarations: [
        SharedExperimentComponent
    ]
})

export class SharedModule { }
