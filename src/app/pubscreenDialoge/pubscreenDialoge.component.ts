import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PISiteService } from '../services/piSite.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
//import { UploadService } from '../services/upload.service';
import { SharedModule } from '../shared/shared.module';


@Component({

    selector: 'app-pubscreenDialoge',
    templateUrl: './pubscreenDialoge.component.html',
    styleUrls: ['./pubscreenDialoge.component.scss'],
    providers: [TaskAnalysisService,  PISiteService]

})
export class PubscreenDialogeComponent implements OnInit {


    

    constructor(public thisDialogRef: MatDialogRef<PubscreenDialogeComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, private location: Location,
        private taskAnalysisService: TaskAnalysisService, 
        private piSiteService: PISiteService, private spinnerService: Ng4LoadingSpinnerService, ) { }

    ngOnInit() {

        //console.log(this.data)
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {
        

    }

    




}
