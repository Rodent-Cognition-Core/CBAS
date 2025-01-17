import { Component, OnInit, Inject } from '@angular/core';
//import { DataSource } from '@angular/cdk/collections';
//import { Observable } from 'rxjs';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { OAuthService } from 'angular-oauth2-oidc';
import { AuthenticationService } from '../services/authentication.service';
//import { AnimalService } from '../services/animal.service';
import { UploadResultDialogComponent } from '../upload-result-dialog/upload-result-dialog.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { UploadService } from '../services/upload.service';

import { ViewChild } from '@angular/core'
import {
    DropzoneComponent, DropzoneDirective,
    DropzoneConfigInterface
} from 'ngx-dropzone-wrapper';
import { SubExperiment } from '../models/subexperiment';
import { CANNOTUPLOADFILETYPE, FAILEDTOADDUPLOADDUETOSERVER, UPLOADERROR } from '../shared/messages';

@Component({
    selector: 'app-upload',
    templateUrl: './upload.component.html',
    styleUrls: ['./upload.component.scss'],

})
export class UploadComponent implements OnInit {

    //selectedExpValue: string;
    experimentName: string;
    experimentID: any;
    subExpID: any;
    subExpName: string;
    drugName: string;
    interventionDescription: string;
    isIntervention: boolean;
    isDrug: boolean;
    ageInMonth: string;
    sessionNameVal: any;
    expTask: string;
    expTaskID: any;
    SessionList: any;

    uploadErrorServer: string = "";
    uploadErrorFileType: string = "";

    uploadConfirmShowed: boolean = false;

    public dialogRefDelFile: MatDialogRef<DeleteConfirmDialogComponent>

    // DropZone

    @ViewChild(DropzoneComponent, { static: false }) componentRef: DropzoneComponent;
    @ViewChild(DropzoneDirective, { static: false }) directiveRef: DropzoneDirective;

    //fileToUpload: File = null;
    public type: string = 'component';
    proceedUpload: boolean = true;
    //public disabled: boolean = false;

    public config: DropzoneConfigInterface = {
        clickable: true,
        maxFiles: 5000,
        autoReset: null,
        errorReset: null,
        cancelReset: null,
        timeout: 36000000,
        headers: { 'Authorization': 'Bearer ' + this.oAuthService.getAccessToken() }
    };

    constructor(
        private oAuthService: OAuthService,
        private authenticationService: AuthenticationService,
        public dialog: MatDialog,
        private spinnerService: NgxSpinnerService,
        private uploadService: UploadService,

    ) {
        this.experimentName = '';
        this.subExpName = '';
        this.drugName = '';
        this.interventionDescription = '';
        this.isIntervention = false;
        this.isDrug = false;
        this.ageInMonth = '';
        this.expTask = '';


    }

    ngOnInit() {

        this.uploadService.getSessionInfo().subscribe((data : any) => { this.SessionList = data; /*console.log(this.SessionList);*/ });

    }

    SelectedExpChanged(experiment : any) {

        //console.log(experiment);
        this.experimentName = experiment.expName;
        this.experimentID = experiment.expID;
        this.expTask = experiment.taskName;
        this.expTaskID = experiment.taskID;

        this.uploadService.getSessionInfo().subscribe((data: any) => { this.SessionList = data; /*console.log(this.SessionList);*/ });
        this.subExpID = null;
        this.sessionNameVal = null;

    }

    SelectedSubExpChanged(subExperiment : any) {
        //console.log(subExperiment);
        this.subExpID = subExperiment.subExpID;
        this.drugName = subExperiment.drugName;
        this.interventionDescription = subExperiment.interventionDescription;
        this.isIntervention = subExperiment.isIntervention;
        this.isDrug = subExperiment.isDrug;
        this.subExpName = subExperiment.subExpName;
        this.ageInMonth = subExperiment.ageInMonth;

        this.sessionNameVal = null;
        //console.log(this.expTaskID);

        switch (this.expTaskID) {
            case 2: { // 5-choice
                this.SessionList = this.SessionList.filter(((x : any) => x.taskID === 1 || x.taskID === 2));
                //console.log(this.SessionList);
                break;

            }
            case 3: { // PD
                this.SessionList = this.SessionList.filter((x: any) => x.taskID === 1 || x.taskID === 3);
                break;
            }
            case 4: { // PAL
                this.SessionList = this.SessionList.filter((x: any) => x.taskID === 1 || x.taskID === 4);
                break;
            }
            case 5: {  // LD
                this.SessionList = this.SessionList.filter((x: any) => x.taskID === 1 || x.taskID === 5);
                break;
            }
            case 9: { //PR
                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 9) && x.sessionName != 'Must_Initiate' && x.sessionName != 'Punish_Incorrect');
                break;

            }
            case 10: { //PRL
                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 10) && x.sessionName != 'Must_Initiate' && x.sessionName != 'Punish_Incorrect');
                break;

            }
            case 11: { //CPT
                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 11) && x.sessionName != 'Initial_Touch' && x.sessionName != 'Must_Touch' && x.sessionName != 'Must_Initiate' && x.sessionName != 'Punish_Incorrect');
                break;

            }
            case 12: { //VMCL
                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 12));
                break;

            }
            case 13: { // Autoshaping

                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 13) && x.sessionName != 'Initial_Touch' && x.sessionName != 'Must_Touch' && x.sessionName != 'Must_Initiate' && x.sessionName != 'Punish_Incorrect');
                break;

            }
            case 13: { // Autoshaping

                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 13) && x.sessionName != 'Initial_Touch' && x.sessionName != 'Must_Touch' && x.sessionName != 'Must_Initiate' && x.sessionName != 'Punish_Incorrect');
                break;

            }
            case 14: { //Extinction
                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 14));
                break;

            }
            case 15: { //Long Sequence
                this.SessionList = this.SessionList.filter((x: any) => (x.taskID === 1 || x.taskID === 15) && x.sessionName != 'Must_Initiate' && x.sessionName != 'Punish_Incorrect');
                break;

            }

            default: {
                //statements; {}
                break;
            }
        }


    }
    // *********************DropZone functions******************************

    onAddedFile(data : any): void {

        if (!this.uploadConfirmShowed) {


            this.uploadConfirmShowed = true;

            this.dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
                width: '700px',
                disableClose: true
            });

            // Based on Subexp features, generate the confirmation message
            if (this.isIntervention && this.isDrug) {

                this.dialogRefDelFile.componentInstance.confirmMessage = "Are you sure you want to upload <b>" + this.sessionNameVal + "</b> XML files to sub-exp <b>" + this.subExpName + "</b> with age: <b>" + this.ageInMonth + "</b> and intervention <b> " + this.drugName + "</b>?";
            }

            if (this.isIntervention && !this.isDrug) {
                this.dialogRefDelFile.componentInstance.confirmMessage = "Are you sure you want to upload <b>" + this.sessionNameVal + "</b> XML files to sub-exp <b>" + this.subExpName + "</b> with age: <b>" + this.ageInMonth + "</b> and intervention <b> " + this.interventionDescription + "</b>?";
            }

            if (!this.isIntervention && !this.isDrug) {
                this.dialogRefDelFile.componentInstance.confirmMessage = "Are you sure you want to upload <b>" + this.sessionNameVal + "</b> XML files to sub-exp <b>" + this.subExpName + "</b> with age: <b>" + this.ageInMonth + "</b>?";
            }



            this.dialogRefDelFile.afterClosed().subscribe(result => {
                if (result) {

                    if (this.uploadErrorFileType != "") {
                        alert(UPLOADERROR + this.uploadErrorFileType);

                        setTimeout(() => {
                            this.spinnerService.hide();
                        }, 500);

                    }
                    this.uploadErrorFileType = "";

                    this.componentRef.directiveRef.dropzone().processQueue();
                } else {
                    this.componentRef.directiveRef.dropzone().removeAllFiles();
                }

                this.uploadConfirmShowed = false;
                //this.dialogRefDelFile = null;
                this.dialogRefDelFile.close();
            });


        }

    }

    onQueueComplete(args: any): void {

        if (this.uploadErrorServer != "") {
            alert(this.uploadErrorServer);

            setTimeout(() => {
                this.spinnerService.hide();
            }, 500);

            this.uploadErrorFileType = ""; // when server error happens error file type shouldn't be shown

        }

        this.uploadErrorServer = "";

    }

    public resetDropzoneUploads() {

        if (this.type === 'directive') {
            this.directiveRef.reset();
        } else {
            this.componentRef.directiveRef.reset();
        }

    }

    public onUploadError(args: any) {

        if (args[1] == CANNOTUPLOADFILETYPE) {
            this.uploadErrorFileType = this.uploadErrorFileType + "You can't upload files of this type: '" + args[0].name + "'" + "\n\r";
            //console.log("You can't upload files of this type: '" + args[0].name + "'");
        } else {
            this.uploadErrorServer = FAILEDTOADDUPLOADDUETOSERVER;
        }

        this.resetDropzoneUploads();
        //console.log('onUploadError:', args);
    }

    public maxfilesexceeded(args: any) {

        //console.log('max exceeded!');
    }

    public onUploadSuccess(args: any) {

        this.resetDropzoneUploads();

        if (this.proceedUpload == true) {


            let dialogref = this.dialog.open(UploadResultDialogComponent, {
                height: '900px',
                width: '850px',
                data: { uploadResult: args[1], experimentName: this.experimentName } // data.uploadResult  & data.experimentName should be sent to UploadResultDialogComponent 
            });
        }

        this.proceedUpload = false;

        setTimeout(() => {
            this.spinnerService.hide();
        }, 500);

    }


    onSending(data : any): void {

        this.proceedUpload = true;
        this.spinnerService.show();


        var obj = this.SessionList.filter((x : any) => x.sessionName === this.sessionNameVal);
        var sessionID = obj[0].id


        const formData = data[2];
        formData.append('expId', this.experimentID);
        formData.append('subExpId', this.subExpID);
        formData.append('sessionName', this.sessionNameVal);
        formData.append('sessionID', sessionID);

    }

    //*******************End of DropZone Functions *****************************



}




