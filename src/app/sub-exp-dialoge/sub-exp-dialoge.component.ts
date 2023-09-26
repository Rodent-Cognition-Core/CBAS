import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { SubExperiment } from '../models/subexperiment';
import { SubExpDialogeService } from '../services/subexpdialoge.service';
import { FIELDISREQUIRED, SUBEXPERIEMENTWITHSAMECONDITIONS, SUBEXPERIMENTNAMETAKEN } from '../shared/messages';

@Component({
    selector: 'app-sub-exp-dialoge',
    templateUrl: './sub-exp-dialoge.component.html',
    styleUrls: ['./sub-exp-dialoge.component.scss'],
    providers: [SubExpDialogeService,]
})

export class SubExpDialogeComponent implements OnInit {

    subExpList: any;
    isTakenSubExpName: boolean;
    isTakenAge: boolean;
    isTakenAgeSubExp: boolean;
    taskID: any;
    
    // ngModel vars
    subExpNameModel: any;
    ageInMonthModel: any;
    interventionModel: string = "0";
    isDrugModel: string;
    drugModel: string;
    drugQuantityModel: string
    drugUnitModel: string;
    intDesModel: string;
    selectedImageVal: any;
    imageDesModel: any;
    housingModel: string;
    lightCycleModel: string;
    
    // Lists
    ageList: any;
    imageList: any;
    selectedImageResult: any; 

    // formControl vars
    ageInMonth = new FormControl('', [Validators.required]);
    subExp = new FormControl('', [Validators.required]);
    intervention = new FormControl('', [Validators.required]);
    isDrug = new FormControl('', [Validators.required]);
    drug = new FormControl('', [Validators.required]);
    //drugQuantity = new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    drugQuantity = new FormControl('', [Validators.required, Validators.pattern(/^[-+]?[0-9]*\.?[0-9]+$/)]);
    drugUnit = new FormControl('', [Validators.required]);
    intDes = new FormControl('', [Validators.required]);
    imageInfo = new FormControl('', [Validators.required]);
    imageDescription = new FormControl('', [Validators.required]);
    housing = new FormControl('', [Validators.required]);
    lightCycle = new FormControl('', [Validators.required]);
    

    private _subexperiment = new SubExperiment();


    constructor(public thisDialogRef: MatDialogRef<SubExpDialogeComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any, private subexpDialogeService: SubExpDialogeService, ) { }

    ngOnInit() {

        this.taskID = this.data.expObj.taskID;
        //console.log(this.taskID)

        this.subexpDialogeService.getAllAge().subscribe(data => { this.ageList = data; });

        this.subexpDialogeService.getAllImages().subscribe(data => { this.imageList = data;  });

        //console.log(this.data.subexperimentObj);
        if (this.data.subexperimentObj != null) {
            this.subExpNameModel = this.data.subexperimentObj.subExpName;
            this.ageInMonthModel = this.data.subexperimentObj.ageID;
            this.interventionModel = this.data.subexperimentObj.isIntervention == true ? "1" : "0";
            this.isDrugModel = this.data.subexperimentObj.isDrug == true ? "1" : "0";
            this.drugModel = this.data.subexperimentObj.drugName;
            this.drugQuantityModel = this.data.subexperimentObj.drugQuantity;
            this.drugUnitModel = this.data.subexperimentObj.drugUnit;
            this.intDesModel = this.data.subexperimentObj.interventionDescription;
            this.selectedImageVal = this.data.subexperimentObj.imageIds;
            this.imageDesModel = this.data.subexperimentObj.imageDescription;
            this.housingModel = this.data.subexperimentObj.housing;
            this.lightCycleModel = this.data.subexperimentObj.lightCycle;

            //this.selectedImageResult = this.data.subexperimentObj.imageIds.length;
            this.getSelectedImage(this.selectedImageVal);
            
        }
    }

   
    //onAgeInputChange(ageValue: string) {

    //    if (Number(ageValue)) {

    //        if (
    //            (Number(ageValue) === parseInt(ageValue, 10)) // to check if it is integer
    //            && (Number(ageValue) >= 1)
    //            && (Number(ageValue) <= 36)
    //        ) {
    //            // input is an int between 1 and 36, validation pass!
    //            this.ageInMonth.setErrors({ 'range': null });
    //        }

    //        else {
    //            this.ageInMonth.setErrors({ 'range': true });
    //        }

    //    } else {
    //        this.ageInMonth.setErrors({ 'range': true });
    //    }

    //}



    getErrorMessage() {

        return this.subExp.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageTaken() {

        return this.subExp.hasError('takenSubExpName') ? SUBEXPERIMENTNAMETAKEN :
            '';

    }

    getErrorMessageAge() {
        return this.ageInMonth.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageIntervention() {
        return this.intervention.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageIsDrug() {
        return this.isDrug.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageDrug() {
        return this.drug.hasError('required') ? FIELDISREQUIRED : '';
    }


    getErrorMessageDrugQunatity() {
        return this.drugQuantity.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageDrugQunatityNumber() {
        return this.drugQuantity.hasError('pattern') ? FIELDISREQUIRED : '';
    }

    getErrorMessageDrugUnit() {
        return this.drugUnit.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageIntDesc() {
        return this.intDes.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageTakenAge() {
        return this.ageInMonth.hasError('takenAge') ? SUBEXPERIEMENTWITHSAMECONDITIONS:
            '';

    }

    getErrorMessageImage() {
        return this.imageInfo.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageImageDescription() {
        return this.imageDescription.hasError('required') ? FIELDISREQUIRED : '';

    }

    getErrorMessageHousing() {
        return this.housing.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageLightCycle() {
        return this.lightCycle.hasError('required') ? FIELDISREQUIRED :
            '';
    }


    //getErrorMessageAgeValidation() {

    //    return this.ageInMonth.hasError('range') ? 'Animal age should be a number between 1 and 36!' : '';
    //}

    //getErrorMessageAgeSubExpValidation() {
    //    return this.AgeSubExp.hasError('takenAgeExp') ? 'This sub-experiment name was already taken and a sub-experiment with the same age already exists! ' : '';
    //}

    setDisabledVal() {

        if (this.subExp.hasError('required') ||
            this.ageInMonth.hasError('required') ||
            this.subExp.hasError('takenSubExpName') ||
            //this.ageInMonth.hasError('takenAge') ||
            this.ageInMonth.hasError('required') ||
            this.intervention.hasError('required') ||
            this.housing.hasError('required') ||
            this.lightCycle.hasError('required')
            || ((this.taskID == 3 || this.taskID == 4) && this.imageInfo.hasError('required'))
            || (((this.taskID == 3 && this.selectedImageVal.length > 2) || (this.taskID == 4 && this.selectedImageVal.length > 3)) && this.imageDescription.hasError('required'))
            

        ) {
            return true;
        } 

        return false;
    }

    getSelectedImage(selectedImageVal) {

        if (selectedImageVal != null) {
            return this.selectedImageResult = selectedImageVal.length;
        } else {
            return 0;
        }
        

    }
    
    onCloseCancel(): void {

        //console.log(this.drugModel);
        //console.log(this.isDrugModel);
        this.thisDialogRef.close('Cancel');

    }


    onCloseSubmit(): void {

        if (this.isDrugModel == "0") {

            this.drugModel = '';
            this.drugQuantityModel = '';
            this.drugUnitModel = '';
        }

        if (this.isDrugModel == "1" && this.interventionModel == "1") {

            this.intDesModel = '';

        }

        if (this.interventionModel == "0") {

            this.isDrugModel = "0";
            this.drugModel = '';
            this.drugQuantityModel = '';
            this.drugUnitModel = '';
            this.intDesModel = '';

        }

        this._subexperiment.SubExpName = this.subExpNameModel;
        this._subexperiment.AgeID = this.ageInMonthModel;
        this._subexperiment.ExpID = this.data.expObj.expID;
        this._subexperiment.isIntervention = this.interventionModel == "1" ? true : false;
        this._subexperiment.isDrug = this.isDrugModel == "1" ? true : false;
        this._subexperiment.drugName = this.drugModel;
        this._subexperiment.drugQuantity = this.drugQuantityModel;
        this._subexperiment.drugUnit = this.drugUnitModel;
        this._subexperiment.interventionDescription = this.intDesModel;
        this._subexperiment.ImageIds = this.selectedImageVal;
        this._subexperiment.ImageDescription = this.imageDesModel;
        this._subexperiment.Housing = this.housingModel;
        this._subexperiment.LightCycle = this.lightCycleModel;

        //console.log(this._subexperiment);
        if (this.data.subexperimentObj == null) {
            // Insert Mode: Insert sub Experiment
            this.isTakenSubExpName = false;
            this.isTakenAge = false;


            this.subexpDialogeService.createSubExp(this._subexperiment).map(res => {

                //console.log('create');
                //console.log(res);

                if (res == "TakenSubExpName") {
                    this.isTakenSubExpName = true;
                    this.subExp.setErrors({ 'takenSubExpName': true });
                }
                else if (res == "takenSubExp") {
                    this.isTakenAge = true;
                    this.ageInMonth.setErrors({ 'takenAge': true });
                }
                else {
                    this.thisDialogRef.close();
                }

            }).subscribe();

        } else { // Edit Mode: edit sub experiment

            // check if number of iamges for PAL or PD are equal to 3 and 2 respectively, clean the value of imageDescription
            if ((this.taskID == 3 && this._subexperiment.ImageIds.length == 2) || (this.taskID == 4 && this._subexperiment.ImageIds.length == 3) ||
                (this.taskID == 11 && this._subexperiment.ImageIds.length <=6) || (this.taskID == 12 && this._subexperiment.ImageIds.length <=4))
            {
                this._subexperiment.ImageDescription = "";
                //console.log(123456);

            }
            
            this.isTakenSubExpName = false;
            this.isTakenAge = false;

            this._subexperiment.SubExpID = this.data.subexperimentObj.subExpID;
            this.subexpDialogeService.updateSubExp(this._subexperiment).map(res => {

                //console.log('update');
                //console.log(res);

                if (res == "TakenSubExpName") {
                    this.isTakenSubExpName = true;
                    this.subExp.setErrors({ 'takenSubExpName': true });
                }
                else if (res == "takenSubExp") {
                    this.isTakenAge = true;
                    this.ageInMonth.setErrors({ 'takenAge': true });
                }
                else {
                    this.thisDialogRef.close();
                }

            }).subscribe();

        }


    }



}
