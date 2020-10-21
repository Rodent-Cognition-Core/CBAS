import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { CogbytesDialogueComponent } from '../cogbytesDialogue/cogbytesDialogue.component'
import { CogbytesUpload } from '../models/cogbytesUpload'
import { CogbytesService } from '../services/cogbytes.service'
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';


@Component({

    selector: 'app-cogbytesUpload',
    templateUrl: './cogbytesUpload.component.html',
    styleUrls: ['./cogbytesUpload.component.scss'],
    providers: [CogbytesService],
})

export class CogbytesUploadComponent implements OnInit {

    public uploadKey: number;
    public parentRef: CogbytesDialogueComponent;

    //public nameModel: any;
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
    public taskList: any;
    public speciesList: any;
    public sexList: any;
    public strainList: any;
    public genosList: any;
    public ageList: any;

    _cogbytesUpload = new CogbytesUpload();


    constructor(
        public thisDialogRef: MatDialogRef<CogbytesUploadComponent>,
        //private spinnerService: Ng4LoadingSpinnerService,
        public dialog: MatDialog,
        private cogbytesService: CogbytesService,
        @Inject(MAT_DIALOG_DATA) public data: any,
    )
    {
        this.resetFormVals();
    }
        
    
    ngOnInit() {
        this.cogbytesService.getTask().subscribe(data => { this.taskList = data; });
        this.cogbytesService.getSpecies().subscribe(data => { this.speciesList = data; });
        this.cogbytesService.getSex().subscribe(data => { this.sexList = data; });
        this.cogbytesService.getStrain().subscribe(data => { this.strainList = data; });
        this.cogbytesService.getGenos().subscribe(data => { this.genosList = data; });
        this.cogbytesService.getAges().subscribe(data => { this.ageList = data; });
    }

    //name = new FormControl('', [Validators.required]);
    //date = new FormControl('', [Validators.required]);

    fileType = new FormControl('', [Validators.required]);
    cognitiveTask = new FormControl('', [Validators.required]);
    intervention = new FormControl('', [Validators.required]);

    getErrorMessageFileType() {
        return this.fileType.hasError('required') ? 'You must enter a value' : '';
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

    resetFormVals() {

        this.fileTypeModel = '';
        this.descriptionModel = '';
        this.additionalNotesModel = '';
        this.cognitiveTaskModel = [];
        this.speciesModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.genotypeModel = [];
        this.ageModel = [];
        this.housingModel = [];
        this.lightModel = [];
        this.intDesModel = '';
    }


    remove() {
        this.parentRef.removeUploadComponent(this.uploadKey);
    }

}
