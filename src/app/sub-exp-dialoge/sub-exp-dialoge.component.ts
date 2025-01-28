import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
import { SubExperiment } from '../models/subexperiment';
import { SubExpDialogeService } from '../services/subexpdialoge.service';
import { FIELDISREQUIRED, SUBEXPERIEMENTWITHSAMECONDITIONS, SUBEXPERIMENTNAMETAKEN } from '../shared/messages';

@Component({
  selector: 'app-sub-exp-dialoge',
  templateUrl: './sub-exp-dialoge.component.html',
  styleUrls: ['./sub-exp-dialoge.component.scss'],
  providers: [SubExpDialogeService, ]
})

export class SubExpDialogeComponent implements OnInit {

  subExpList: any;
  isTakenSubExpName: boolean;
  isTakenAge: boolean;
  isTakenAgeSubExp: boolean;
  taskID: any;

  // Lists
  ageList: any;
  imageList: any;
  selectedImageResult: any;

  // formControl vars
  ageInMonth: UntypedFormControl;
  subExp: UntypedFormControl;
  intervention: UntypedFormControl;
  isDrug: UntypedFormControl;
  drug: UntypedFormControl;
  // drugQuantity = new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
  drugQuantity: UntypedFormControl;
  drugUnit: UntypedFormControl;
  intDes: UntypedFormControl;
  imageInfo: UntypedFormControl;
  imageDescription: UntypedFormControl;
  housing: UntypedFormControl;
  lightCycle: UntypedFormControl;


  private _subexperiment: SubExperiment;


  constructor(public thisDialogRef: MatDialogRef<SubExpDialogeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private subexpDialogeService: SubExpDialogeService,
    private fb: UntypedFormBuilder) {

    this.isTakenSubExpName = false;
    this.isTakenAge = false;
    this.isTakenAgeSubExp = false;
    this.ageInMonth = fb.control('', [Validators.required]);
    this.subExp = fb.control('', [Validators.required]);
    this.intervention = fb.control('', [Validators.required]);
    this.isDrug = fb.control('', [Validators.required]);
    this.drug = fb.control('', [Validators.required]);
    this.drugQuantity = fb.control('', [Validators.required, Validators.pattern(/^[-+]?[0-9]*\.?[0-9]+$/)]);
    this.drugUnit = fb.control('', [Validators.required]);
    this.intDes = fb.control('', [Validators.required]);
    this.imageInfo = fb.control('', [Validators.required]);
    this.imageDescription = fb.control('', [Validators.required]);
    this.housing = fb.control('', [Validators.required]);
    this.lightCycle = fb.control('', [Validators.required]);
    this._subexperiment = {
      ageID: 0, ageInMonth: '', drugName: '', drugQuantity: '', drugUnit: '', errorMessage: '', expID: 0, housing: '', imageDescription: '',
      imageIds: [], imageInfo: '', interventionDescription: '', isDrug: false, isIntervention: false, isPostProcessingPass: false,
      lightCycle: '', subExpID: 0, subExpName: ''
    };
  }

  ngOnInit() {

    this.taskID = this.data.expObj.taskID;
    // console.log(this.taskID)

    this.subexpDialogeService.getAllAge().subscribe((data: any) => {
      this.ageList = data;
    });

    this.subexpDialogeService.getAllImages().subscribe((data: any) => {
      this.imageList = data;
    });

    // console.log(this.data.subexperimentObj);
    if (this.data.subexperimentObj != null) {
      this.subExp.setValue(this.data.subexperimentObj.subExpName);
      this.ageInMonth.setValue(this.data.subexperimentObj.ageID);
      this.intervention.setValue(this.data.subexperimentObj.isIntervention === true ? '1' : '0');
      this.isDrug.setValue(this.data.subexperimentObj.isDrug === true ? '1' : '0');
      this.drug.setValue(this.data.subexperimentObj.drugName);
      this.drugQuantity.setValue(this.data.subexperimentObj.drugQuantity);
      this.drugUnit.setValue(this.data.subexperimentObj.drugUnit);
      this.intDes.setValue(this.data.subexperimentObj.interventionDescription);
      this.imageInfo.setValue(this.data.subexperimentObj.imageIds);
      this.imageDescription.setValue(this.data.subexperimentObj.imageDescription);
      this.housing.setValue(this.data.subexperimentObj.housing);
      this.lightCycle.setValue(this.data.subexperimentObj.lightCycle);

      // this.selectedImageResult = this.data.subexperimentObj.imageIds.length;
      this.getSelectedImage(this.imageInfo.value);

    }
  }


  // onAgeInputChange(ageValue: string) {

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

  // }



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
    return this.ageInMonth.hasError('takenAge') ? SUBEXPERIEMENTWITHSAMECONDITIONS :
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

  setDisabledVal() {

    if (this.subExp.hasError('required') ||
            this.ageInMonth.hasError('required') ||
            this.subExp.hasError('takenSubExpName') ||
            // this.ageInMonth.hasError('takenAge') ||
            this.ageInMonth.hasError('required') ||
            this.intervention.hasError('required') ||
            this.housing.hasError('required') ||
            this.lightCycle.hasError('required')
            || ((this.taskID === 3 || this.taskID === 4) && this.imageInfo.hasError('required'))
        || (((this.taskID === 3 && this.imageInfo.value.length > 2) || (this.taskID === 4 && this.imageInfo.value.length > 3)) &&
            this.imageDescription.hasError('required'))


    ) {
      return true;
    }

    return false;
  }

  getSelectedImage(selectedImageVal: any) {

    if (selectedImageVal != null) {
      return this.selectedImageResult = selectedImageVal.length;
    } else {
      return 0;
    }


  }

  onCloseCancel(): void {

    // console.log(this.drugModel);
    // console.log(this.isDrugModel);
    this.thisDialogRef.close('Cancel');

  }


  onCloseSubmit(): void {

    if (this.isDrug.value === '0') {

      this.drug.setValue('');
      this.drugQuantity.setValue('');
      this.drugUnit.setValue('');
    }

    if (this.isDrug.value === '1' && this.intervention.value === '1') {

      this.intDes.setValue('');

    }

    if (this.intervention.value === '0') {

      this.isDrug.setValue('0');
      this.drug.setValue('');
      this.drugQuantity.setValue('');
      this.drugUnit.setValue('');
      this.intDes.setValue('');

    }

    this._subexperiment.subExpName = this.subExp.value;
    this._subexperiment.ageID = this.ageInMonth.value;
    this._subexperiment.expID = this.data.expObj.expID;
    this._subexperiment.isIntervention = this.intervention.value === '1' ? true : false;
    this._subexperiment.isDrug = this.isDrug.value === '1' ? true : false;
    this._subexperiment.drugName = this.drug.value;
    this._subexperiment.drugQuantity = this.drugQuantity.value;
    this._subexperiment.drugUnit = this.drugUnit.value;
    this._subexperiment.interventionDescription = this.intDes.value;
    this._subexperiment.imageIds = this.imageInfo.value;
    this._subexperiment.imageDescription = this.imageDescription.value;
    this._subexperiment.housing = this.housing.value;
    this._subexperiment.lightCycle = this.lightCycle.value;

    // console.log(this._subexperiment);
    if (this.data.subexperimentObj == null) {
      // Insert Mode: Insert sub Experiment
      this.isTakenSubExpName = false;
      this.isTakenAge = false;


      this.subexpDialogeService.createSubExp(this._subexperiment).map((res: any) => {

        // console.log('create');
        // console.log(res);

        if (res === 'TakenSubExpName') {
          this.isTakenSubExpName = true;
          this.subExp.setErrors({ 'takenSubExpName': true });
        } else if (res === 'takenSubExp') {
          this.isTakenAge = true;
          this.ageInMonth.setErrors({ 'takenAge': true });
        } else {
          this.thisDialogRef.close();
        }

      }).subscribe();

    } else { // Edit Mode: edit sub experiment

      // check if number of iamges for PAL or PD are equal to 3 and 2 respectively, clean the value of imageDescription
      if ((this.taskID === 3 && this._subexperiment.imageIds.length === 2) ||
            (this.taskID === 4 && this._subexperiment.imageIds.length === 3) ||
            (this.taskID === 11 && this._subexperiment.imageIds.length <= 6) ||
            (this.taskID === 12 && this._subexperiment.imageIds.length <= 4)) {
        this._subexperiment.imageDescription = '';
        // console.log(123456);

      }

      this.isTakenSubExpName = false;
      this.isTakenAge = false;

      this._subexperiment.subExpID = this.data.subexperimentObj.subExpID;
      this.subexpDialogeService.updateSubExp(this._subexperiment).map((res: any) => {

        // console.log('update');
        // console.log(res);

        if (res === 'TakenSubExpName') {
          this.isTakenSubExpName = true;
          this.subExp.setErrors({ 'takenSubExpName': true });
        } else if (res === 'takenSubExp') {
          this.isTakenAge = true;
          this.ageInMonth.setErrors({ 'takenAge': true });
        } else {
          this.thisDialogRef.close();
        }

      }).subscribe();

    }


  }



}
