import { Component, OnInit, Inject, NgModule, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { CogbytesDialogueComponent } from '../cogbytesDialogue/cogbytesDialogue.component'
import { CogbytesUpload } from '../models/cogbytesUpload'
import { CogbytesService } from '../services/cogbytes.service'
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import {
    DropzoneComponent, DropzoneDirective,
    DropzoneConfigInterface
} from 'ngx-dropzone-wrapper';
import { ViewChild } from '@angular/core'
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component'
import { OAuthService } from 'angular-oauth2-oidc';


@Component({

    selector: 'app-cogbytesUpload',
    templateUrl: './cogbytesUpload.component.html',
    styleUrls: ['./cogbytesUpload.component.scss'],
    //providers: [CogbytesService],
})


export class CogbytesUploadComponent implements OnInit {

    @Input() repID: number;
    @Input() isEditMode: boolean;
    @Input() uploadObj: any;

    @Output() filesUploaded: EventEmitter<any> = new EventEmitter();
    @Output() repChange: EventEmitter<any> = new EventEmitter();

    readonly DATASET = 1;

    isUploadAdded = false;
    uploadID: number;

    //public parentRef: CogbytesDialogueComponent;

    public nameModel: any;
    //public dateModel: any;
    public fileTypeModel: any;
    public descriptionModel: any;
    public additionalNotesModel: any;
    public cognitiveTaskModel: any;
    public speciesModel: any;
    public sexModel: any;
    public strainModel: any;
    public genotypeModel: any;
    public ageModel: any;
    public housingModel: any;
    public lightModel: any;
    public interventionModel: any;
    public intDesModel: any;
    public imgDesModel: any;
    public taskBatteryModel: any;

    // Definiing List Variables 
    public fileTypeList: any;
    public taskList: any;
    public speciesList: any;
    public sexList: any;
    public strainList: any;
    public genosList: any;
    public ageList: any;

    public uploadFileList: any;

    _cogbytesUpload = new CogbytesUpload();

    // DropZone
    @ViewChild(DropzoneComponent) componentRef: DropzoneComponent;
    @ViewChild(DropzoneDirective) directiveRef: DropzoneDirective;

    //fileToUpload: File = null;
    public type: string = 'component';
    proceedUpload: boolean = true;
    //public disabled: boolean = false;

    uploadErrorServer: string = "";
    uploadErrorFileType: string = "";

    uploadConfirmShowed: boolean = false;

    dialogRefDelFile: MatDialogRef<DeleteConfirmDialogComponent>;

    //DropZone
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
        //public thisDialogRef: MatDialogRef<CogbytesUploadComponent>,
        private spinnerService: Ng4LoadingSpinnerService,
        public dialog: MatDialog,
        private cogbytesService: CogbytesService,
        @Inject(MAT_DIALOG_DATA) public data: any,
    )
    {
        this.resetFormVals();
    }
        
    
    ngOnInit() {

        this.cogbytesService.getFileTypes().subscribe(data => { this.fileTypeList = data; });
        this.cogbytesService.getTask().subscribe(data => { this.taskList = data; });
        this.cogbytesService.getSpecies().subscribe(data => { this.speciesList = data; });
        this.cogbytesService.getSex().subscribe(data => { this.sexList = data; });
        this.cogbytesService.getStrain().subscribe(data => { this.strainList = data; });
        this.cogbytesService.getGenos().subscribe(data => { this.genosList = data; });
        this.cogbytesService.getAges().subscribe(data => { this.ageList = data; });

        if (this.isEditMode) {

            this.isUploadAdded = true;

            this.uploadID = this.uploadObj.id;
            this.nameModel = this.uploadObj.name;
            this.fileTypeModel = this.uploadObj.fileTypeID;
            this.descriptionModel = this.uploadObj.description;
            this.additionalNotesModel = this.uploadObj.additionalNotes;
            this.cognitiveTaskModel = this.uploadObj.taskID;
            this.speciesModel = this.uploadObj.specieID;
            this.sexModel = this.uploadObj.sexID;
            this.strainModel = this.uploadObj.strainID;
            this.genotypeModel = this.uploadObj.genoID;
            this.ageModel = this.uploadObj.ageID;
            this.housingModel = this.uploadObj.housing;
            this.lightModel = this.uploadObj.lightCycle;
            this.interventionModel = this.uploadObj.isIntervention ? "true" : "false";
            this.intDesModel = this.uploadObj.interventionDescription;
            this.imgDesModel = this.uploadObj.imageDescription;
            this.taskBatteryModel = this.uploadObj.taskBattery;

            this.uploadFileList = this.uploadObj.uploadFileList;
            // this.UpdateFileList();
        }

    }

    ngOnChanges(changes: SimpleChanges) {
        for (const propName in changes) {
            if (changes.hasOwnProperty(propName)) {
                if (propName === 'repID') {
                    this.resetFormVals();
                    this.repChange.emit(null);
                }
            }
        }
    }


    name = new FormControl('', [Validators.required]);
    //date = new FormControl('', [Validators.required]);

    fileType = new FormControl('', [Validators.required]);
    cognitiveTask = new FormControl('', [Validators.required]);
    intervention = new FormControl('', [Validators.required]);

    getErrorMessageFileType() {
        return this.fileType.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageName() {
        return this.name.hasError('required') ? 'You must enter a value' : '';
    }
    getErrorMessageTask() {
        return this.cognitiveTask.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageIntervention() {
        return this.intervention.hasError('required') ? 'You must enter a value!' : '';
    }

    //getErrorMessageName() {

    //    return this.name.hasError('required') ? 'You must enter a value' : '';

    //}

    //getErrorMessageDate() {
    //    return this.date.hasError('required') ? 'You must enter a value' : '';
    //}

    setDisabledVal() {

        if (this.fileType.hasError('required') || this.name.hasError('required')) {
            return true;
        }

        if (this.fileTypeModel === this.DATASET) {
            if (this.cognitiveTask.hasError('required') || this.intervention.hasError('required')) {
                return true;
            }
        }

        if (this.isUploadAdded && !this.isEditMode) return true;

        return false;
    }

    resetFormVals() {

        this.fileTypeModel = null;
        this.nameModel = '';
        this.descriptionModel = '';
        this.additionalNotesModel = '';
        this.cognitiveTaskModel = [];
        this.speciesModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.genotypeModel = [];
        this.ageModel = [];
        this.housingModel = '';
        this.lightModel = '';
        this.intDesModel = '';
        this.imgDesModel = '';
        this.taskBatteryModel = '';
        this.interventionModel = null;

        if (!this.isEditMode) this.isUploadAdded = false;
    }

    AddUpload() {

        this._cogbytesUpload.repId = this.repID;
        this._cogbytesUpload.fileTypeId = this.fileTypeModel;
        this._cogbytesUpload.name = this.nameModel;
        let today = new Date();
        this._cogbytesUpload.dateUpload = today.toISOString().split('T')[0];
        this._cogbytesUpload.description = this.descriptionModel;
        this._cogbytesUpload.additionalNotes = this.additionalNotesModel;
        this._cogbytesUpload.isIntervention = this.interventionModel == "true" ? true : false;
        this._cogbytesUpload.interventionDescription = this.intDesModel;
        // IMAGE IDS TO BE IMPLEMENTED LATERthis._cogbytesUpload.imageIds =
        this._cogbytesUpload.imageDescription = this.imgDesModel;
        this._cogbytesUpload.housing = this.housingModel;
        this._cogbytesUpload.lightCycle = this.lightModel;
        this._cogbytesUpload.taskBattery = this.taskBatteryModel;

        this._cogbytesUpload.taskID = this.cognitiveTaskModel;
        this._cogbytesUpload.specieID = this.speciesModel;
        this._cogbytesUpload.sexID = this.sexModel;
        this._cogbytesUpload.strainID = this.strainModel;
        this._cogbytesUpload.genoID = this.genotypeModel;
        this._cogbytesUpload.ageID = this.ageModel;

        this.cogbytesService.addUpload(this._cogbytesUpload).subscribe(data => {

            if (data === null) {
                alert("Failed to add upload features to Cogbytes");
            }
            else {
                alert("Upload features were successfully added to the system! Please upload your files via the dropzone.");
                this.isUploadAdded = true;
                this.uploadID = data;
            }
        })
    }

    EditUpload() {

        this._cogbytesUpload.repId = this.repID;
        this._cogbytesUpload.fileTypeId = this.fileTypeModel;
        this._cogbytesUpload.name = this.nameModel;
        let today = new Date();
        this._cogbytesUpload.dateUpload = today.toISOString().split('T')[0];
        this._cogbytesUpload.description = this.descriptionModel;
        this._cogbytesUpload.additionalNotes = this.additionalNotesModel;
        this._cogbytesUpload.isIntervention = this.interventionModel == "true" ? true : false;
        this._cogbytesUpload.interventionDescription = this.intDesModel;
        // IMAGE IDS TO BE IMPLEMENTED LATERthis._cogbytesUpload.imageIds =
        this._cogbytesUpload.imageDescription = this.imgDesModel;
        this._cogbytesUpload.housing = this.housingModel;
        this._cogbytesUpload.lightCycle = this.lightModel;
        this._cogbytesUpload.taskBattery = this.taskBatteryModel;

        this._cogbytesUpload.taskID = this.cognitiveTaskModel;
        this._cogbytesUpload.specieID = this.speciesModel;
        this._cogbytesUpload.sexID = this.sexModel;
        this._cogbytesUpload.strainID = this.strainModel;
        this._cogbytesUpload.genoID = this.genotypeModel;
        this._cogbytesUpload.ageID = this.ageModel;

        this.cogbytesService.editUpload(this.uploadObj.id, this._cogbytesUpload).subscribe(data => {

            if (data === null) {
                alert("Failed to edit features to Cogbytes");
            }
            else {
                alert("Features were successfully edited to the system!");
            }
        })
    }
    public resetDropzoneUploads() {

        if (this.type === 'directive') {
            this.directiveRef.reset();
        } else {
            this.componentRef.directiveRef.reset();
        }

    }

    public onUploadError(args: any) {

        if (!this.isUploadAdded) {
            this.uploadErrorServer = 'Error: upload zone disabled until required features are selected and saved.'
        }
        else {
            this.uploadErrorServer = "Error in upload, please contact administrator at MouseBytes@uwo.ca";
        }
        
        this.resetDropzoneUploads();
        console.log('onUploadError:', args);
    }

    onSending(data): void {

        this.proceedUpload = true;
        this.spinnerService.show();

        const formData = data[2];
        formData.append('uploadID', this.uploadID);
        

    }

    public onUploadSuccess(args: any) {

        this.resetDropzoneUploads();

        alert('Files were successfully uploaded to the database!');

        this.proceedUpload = false;


    }

    onAddedFile(data): void {

        //this.componentRef.directiveRef.dropzone().processQueue();

        //this.uploadConfirmShowed = false;
        //this.dialogRefDelFile = null;
    }

    onQueueComplete(args: any): void {

        if (this.uploadErrorServer != "") {
            alert(this.uploadErrorServer);

            this.uploadErrorFileType = ""; // when server error happens error file type shouldn't be shown

        }

        setTimeout(() => {
            this.spinnerService.hide();
        }, 500);

        this.uploadErrorServer = "";

        if (this.isUploadAdded && !this.isEditMode) {
            this.resetFormVals();
            this.filesUploaded.emit(null);
        }

        else {
            this.UpdateFileList();
        }
        //this.filesUploaded.emit(null);
    }


    DownloadFile(file: any): void {

        let path = file.permanentFilePath + '\\' + file.sysFileName;
        this.cogbytesService.downloadFile(path)
            .subscribe(result => {

                // console.log('downloadresult', result);
                //let url = window.URL.createObjectURL(result);
                //window.open(url);

                var fileData = new Blob([result]);
                var csvURL = null;
                if (navigator.msSaveBlob) {
                    csvURL = navigator.msSaveBlob(fileData, file.userFileName);
                } else {
                    csvURL = window.URL.createObjectURL(fileData);
                }
                var tempLink = document.createElement('a');
                tempLink.href = csvURL;
                tempLink.setAttribute('download', file.userFileName);
                document.body.appendChild(tempLink);
                tempLink.click();

            },
                error => {
                    if (error.error instanceof Error) {
                        console.log('An error occurred:', error.error.message);
                    } else {
                        console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                    }
                });
    }

    // Delete File 
    deleteFile(file) {
        this.openConfirmationDialogDelFile(file);
    }

    // Delete File Dialog
    openConfirmationDialogDelFile(file) {
        this.dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRefDelFile.componentInstance.confirmMessage = "Are you sure you want to delete this file?"

        this.dialogRefDelFile.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                setTimeout(() => {
                    this.spinnerService.hide();
                }, 500);

                let path = file.permanentFilePath + '\\' + file.sysFileName;
                this.cogbytesService.deleteFile(file.expID, path).map(res => {

                }).subscribe(
                    response => { this.UpdateFileList(); });

                //this.spinnerService.hide();
                //this.filesUploaded.emit(null);

                //this.UpdateFileList();
            }
            this.dialogRefDelFile = null;
        });
    }

    UpdateFileList() {
        this.cogbytesService.getUploadFiles(this.uploadID).subscribe(data => { this.uploadFileList = data });
    }

    DeleteUpload() {
        this.openConfirmationDialogDelUpload();
    }

    // Delete Upload Dialog
    openConfirmationDialogDelUpload() {
        this.dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRefDelFile.componentInstance.confirmMessage = "Are you sure you want to delete all of the above features and files?"

        this.dialogRefDelFile.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                this.cogbytesService.deleteUpload(this.uploadID).map(res => {

                }).subscribe(result => {
                    this.filesUploaded.emit(null);
                    setTimeout(() => {
                        this.spinnerService.hide();
                    }, 500);
                });
            }
            this.dialogRefDelFile = null;
        });
    }
    //remove() {
    //    this.parentRef.removeUploadComponent(this.uploadKey);
    //}

}
