import { Component, OnInit, Inject, NgModule, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { CogbytesUpload } from '../models/cogbytesUpload';
import { CogbytesService } from '../services/cogbytes.service';
import { NgxSpinnerService } from 'ngx-spinner';
import {
    DropzoneComponent, DropzoneDirective,
    DropzoneConfigInterface
} from 'ngx-dropzone-wrapper';
import { ViewChild } from '@angular/core'
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component'
import { OAuthService } from 'angular-oauth2-oidc';
import { map } from 'rxjs/operators';
import { CONFIRMDELETE, FAILEDTOADDUPLOADDUETOMISSINGFEATURES, FAILEDTOADDUPLOADDUETOSERVER, FEATUREEDITFAILED, FEATUREEDITSUCCESSFULL, FEATURESUPLOADFAILED, FEATURESUPLOADSUCESS, INVALIDNUMBERICALVALUE, FIELDISREQUIRED, UPLOADSUCCESS } from '../shared/messages';

//declare global {
//    interface Navigator {
//        msSaveBlob: (blobOrBase64: Blob | string, filename: string) => void
//    }
//}

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

    readonly DATASET = [1, 2, 3, 4, 5];

    isUploadAdded = false;
    uploadID: number;

    //public parentRef: CogbytesDialogueComponent;

    //public dateModel: any;
    public descriptionModel: any;
    public additionalNotesModel: any;
    public speciesModel: any;
    public sexModel: any;
    public strainModel: any;
    public genotypeModel: any;
    public ageModel: any;
    public housingModel: any;
    public lightModel: any;
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

    name: FormControl;
    //date = new FormControl('', [Validators.required]);

    fileType: FormControl;
    cognitiveTask: FormControl;
    intervention: FormControl;
    numSubjects: FormControl;

    _cogbytesUpload: CogbytesUpload;

    // DropZone
    @ViewChild(DropzoneComponent, { static: false }) componentRef!: DropzoneComponent
    @ViewChild(DropzoneDirective, { static: false }) directiveRef!: DropzoneDirective

    //fileToUpload: File = null;
    public type: string = 'component';
    proceedUpload: boolean = true;
    //public disabled: boolean = false;

    uploadErrorServer: string = "";
    uploadErrorFileType: string = "";

    uploadConfirmShowed: boolean = false;

    //DropZone
    public config: DropzoneConfigInterface = {
        clickable: true,
        maxFiles: 5000,
        maxFilesize: 10000,
        autoReset: undefined,
        errorReset: undefined,
        cancelReset: undefined,
        timeout: 36000000,
        headers: {}
    };
    

    constructor(
        private oAuthService: OAuthService,
        //public thisDialogRef: MatDialogRef<CogbytesUploadComponent>,
        private spinnerService: NgxSpinnerService,
        public dialog: MatDialog,
        private cogbytesService: CogbytesService,
        private fb: FormBuilder,
        public dialogRefDelFile: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any

    )
    {
        this.repID = 0;
        this.isEditMode = false;
        this.uploadID = 0;
        this._cogbytesUpload = {
            additionalNotes: '', ageID: [], dateUpload: '', description: '', fileTypeId: 0, genoID: [],
            housing: '', id: 0, imageDescription: '', imageIds: '', interventionDescription: '', isIntervention: false,
            lightCycle: '', name: '', numSubjects: 0, repId: 0, sexID: [], specieID: [], strainID: [], taskBattery: '', taskID: []
        }

        this.name = fb.control('', [Validators.required]);
        this.fileType = fb.control('', [Validators.required]);
        this.cognitiveTask = fb.control('', [Validators.required]);
        this.intervention = fb.control('', [Validators.required]);
        this.numSubjects = fb.control('', [Validators.pattern('[0-9]*')]);
        this.config.headers = { 'Authorization': 'Bearer ' + this.oAuthService.getAccessToken() };

        this.resetFormVals();
    }
        
    
    ngOnInit() {

        this.cogbytesService.getFileTypes().subscribe((data : any) => { this.fileTypeList = data; });
        this.cogbytesService.getTask().subscribe((data: any) => { this.taskList = data; });
        this.cogbytesService.getSpecies().subscribe((data: any) => { this.speciesList = data; });
        this.cogbytesService.getSex().subscribe((data: any) => { this.sexList = data; });
        this.cogbytesService.getStrain().subscribe((data: any) => { this.strainList = data; });
        this.cogbytesService.getGenos().subscribe((data: any) => { this.genosList = data; });
        this.cogbytesService.getAges().subscribe((data: any) => { this.ageList = data; });

        if (this.isEditMode) {

            this.isUploadAdded = true;

            this.uploadID = this.uploadObj.id;
            this.name.setValue(this.uploadObj.name);
            this.fileType.setValue(this.uploadObj.fileTypeID);
            this.descriptionModel = this.uploadObj.description;
            this.additionalNotesModel = this.uploadObj.additionalNotes;
            this.cognitiveTask.setValue(this.uploadObj.taskID);
            this.speciesModel = this.uploadObj.specieID;
            this.sexModel = this.uploadObj.sexID;
            this.strainModel = this.uploadObj.strainID;
            this.genotypeModel = this.uploadObj.genoID;
            this.ageModel = this.uploadObj.ageID;
            this.housingModel = this.uploadObj.housing;
            this.lightModel = this.uploadObj.lightCycle;
            this.intervention.setValue(this.uploadObj.isIntervention ? "true" : "false");
            this.intDesModel = this.uploadObj.interventionDescription;
            this.imgDesModel = this.uploadObj.imageDescription;
            this.taskBatteryModel = this.uploadObj.taskBattery;
            if (this.uploadObj.numSubjects != null) {
                this.numSubjects.setValue(this.uploadObj.numSubjects.toString());
            }
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

    getErrorMessageFileType() {
        return this.fileType.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageName() {
        return this.name.hasError('required') ? FIELDISREQUIRED : '';
    }
    getErrorMessageTask() {
        return this.cognitiveTask.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageIntervention() {
        return this.intervention.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageNumSubjects() {
        return this.numSubjects.hasError('pattern') ? INVALIDNUMBERICALVALUE: '';
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

        if (this.fileType.value >= 1 && this.fileType.value <= 5) {
            if (this.cognitiveTask.hasError('required') || this.intervention.hasError('required') || this.numSubjects.hasError('pattern')) {
                return true;
            }
        }

        if (this.isUploadAdded && !this.isEditMode) return true;

        return false;
    }

    resetFormVals() {

        this.fileType.setValue(null);
        this.name.setValue('');
        this.descriptionModel = '';
        this.additionalNotesModel = '';
        this.cognitiveTask.setValue([]);
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
        this.intervention.setValue(null);

        if (!this.isEditMode) this.isUploadAdded = false;
    }

    AddUpload() {

        this._cogbytesUpload.repId = this.repID;
        this._cogbytesUpload.fileTypeId = this.fileType.value;
        this._cogbytesUpload.name = this.name.value;
        let today = new Date();
        this._cogbytesUpload.dateUpload = today.toISOString().split('T')[0];
        this._cogbytesUpload.description = this.descriptionModel;
        this._cogbytesUpload.additionalNotes = this.additionalNotesModel;
        this._cogbytesUpload.isIntervention = this.intervention.value == "true" ? true : false;
        this._cogbytesUpload.interventionDescription = this.intDesModel;
        // IMAGE IDS TO BE IMPLEMENTED LATERthis._cogbytesUpload.imageIds =
        this._cogbytesUpload.imageDescription = this.imgDesModel;
        this._cogbytesUpload.housing = this.housingModel;
        this._cogbytesUpload.lightCycle = this.lightModel;
        this._cogbytesUpload.taskBattery = this.taskBatteryModel;

        this._cogbytesUpload.taskID = this.cognitiveTask.value;
        this._cogbytesUpload.specieID = this.speciesModel;
        this._cogbytesUpload.sexID = this.sexModel;
        this._cogbytesUpload.strainID = this.strainModel;
        this._cogbytesUpload.genoID = this.genotypeModel;
        this._cogbytesUpload.ageID = this.ageModel;

        this._cogbytesUpload.numSubjects = parseInt(this.numSubjects.value);

        this.cogbytesService.addUpload(this._cogbytesUpload).subscribe((data: any) => {

            if (data === null) {
                alert(FEATURESUPLOADFAILED);
            }
            else {
                alert(FEATURESUPLOADSUCESS);
                this.isUploadAdded = true;
                this.uploadID = data;
            }
        })
    }

    EditUpload() {

        this._cogbytesUpload.repId = this.repID;
        this._cogbytesUpload.fileTypeId = this.fileType.value;
        this._cogbytesUpload.name = this.name.value;
        let today = new Date();
        this._cogbytesUpload.dateUpload = today.toISOString().split('T')[0];
        this._cogbytesUpload.description = this.descriptionModel;
        this._cogbytesUpload.additionalNotes = this.additionalNotesModel;
        this._cogbytesUpload.isIntervention = this.intervention.value == "true" ? true : false;
        this._cogbytesUpload.interventionDescription = this.intDesModel;
        // IMAGE IDS TO BE IMPLEMENTED LATERthis._cogbytesUpload.imageIds =
        this._cogbytesUpload.imageDescription = this.imgDesModel;
        this._cogbytesUpload.housing = this.housingModel;
        this._cogbytesUpload.lightCycle = this.lightModel;
        this._cogbytesUpload.taskBattery = this.taskBatteryModel;

        this._cogbytesUpload.taskID = this.cognitiveTask.value;
        this._cogbytesUpload.specieID = this.speciesModel;
        this._cogbytesUpload.sexID = this.sexModel;
        this._cogbytesUpload.strainID = this.strainModel;
        this._cogbytesUpload.genoID = this.genotypeModel;
        this._cogbytesUpload.ageID = this.ageModel;

        this._cogbytesUpload.numSubjects = parseInt(this.numSubjects.value);

        this.cogbytesService.editUpload(this.uploadObj.id, this._cogbytesUpload).subscribe(data => {

            if (data === null) {
                alert(FEATUREEDITFAILED);
            }
            else {
                alert(FEATUREEDITSUCCESSFULL);
            }
        })
    }
    public resetDropzoneUploads() {

        if (this.type === 'directive') {
            this.directiveRef.reset();
        } else {
            if (this.componentRef?.directiveRef?.reset) {
                this.componentRef.directiveRef.reset();
            }
        }

    }

    public onUploadError(args: any) {

        if (!this.isUploadAdded) {
            this.uploadErrorServer = FAILEDTOADDUPLOADDUETOMISSINGFEATURES;
        }
        else {
            this.uploadErrorServer = FAILEDTOADDUPLOADDUETOSERVER;
        }
        
        this.resetDropzoneUploads();
        console.log('onUploadError:', args);
    }

    onSending(data : any): void {

        this.proceedUpload = true;
        this.spinnerService.show();

        const formData = data[2];
        formData.append('uploadID', this.uploadID);
        

    }

    public onUploadSuccess(args: any) {

        this.resetDropzoneUploads();

        alert(UPLOADSUCCESS);

        this.proceedUpload = false;


    }

    onAddedFile(data : any): void {

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
                var csvURL = '';
                //if (navigator.msSaveBlob) {
                //    csvURL = navigator.msSaveBlob(fileData, file.userFileName);
                //} else {
                //    csvURL = window.URL.createObjectURL(fileData);
                //}
                csvURL = window.URL.createObjectURL(fileData);
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
    deleteFile(file : any) {
        this.openConfirmationDialogDelFile(file);
    }

    // Delete File Dialog
    openConfirmationDialogDelFile(file : any) {
        const dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRefDelFile.componentInstance.confirmMessage = CONFIRMDELETE

        dialogRefDelFile.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                setTimeout(() => {
                    this.spinnerService.hide();
                }, 500);

                let path = file.permanentFilePath + '\\' + file.sysFileName;
                this.cogbytesService.deleteFile(file.expID, path).pipe(map((res : any) => {

                })).subscribe(
                    (response : any) => { this.UpdateFileList(); });

                //this.spinnerService.hide();
                //this.filesUploaded.emit(null);

                //this.UpdateFileList();
            }
            //this.dialogRefDelFile = null;
            dialogRefDelFile.close();
        });
    }

    UpdateFileList() {
        this.cogbytesService.getUploadFiles(this.uploadID).subscribe((data : any) => { this.uploadFileList = data });
    }

    DeleteUpload() {
        this.openConfirmationDialogDelUpload();
    }

    // Delete Upload Dialog
    openConfirmationDialogDelUpload() {
        const dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRefDelFile.componentInstance.confirmMessage = CONFIRMDELETE;

        dialogRefDelFile.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                this.cogbytesService.deleteUpload(this.uploadID).map((res : any) => {

                }).subscribe((result : any) => {
                    this.filesUploaded.emit(null);
                    setTimeout(() => {
                        this.spinnerService.hide();
                    }, 500);
                });
            }
            //this.dialogRefDelFile = null;
            dialogRefDelFile.close();
        });
    }
    //remove() {
    //    this.parentRef.removeUploadComponent(this.uploadKey);
    //}

}
