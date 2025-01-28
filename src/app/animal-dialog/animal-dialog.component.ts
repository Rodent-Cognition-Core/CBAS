import { Component, OnInit, Inject/* , NgModule*/ } from '@angular/core';
import { /* MatDialog,*/ MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
import { Animal } from '../models/animal';
import { AnimalService } from '../services/animal.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { map, /* catchError*/ } from 'rxjs/operators';
import { ANIMALIDDOESNOTEXIST, ANIMALIDTAKEN, CANNOTSAVEEDITS, FIELDISREQUIRED, NOTEXIST, SUCCESSFUL, TAKEN } from '../shared/messages';

@Component({
  selector: 'app-animal-dialog',
  templateUrl: './animal-dialog.component.html',
  styleUrls: ['./animal-dialog.component.scss'],
  providers: [AnimalService]
})
export class AnimalDialogComponent implements OnInit {

  _animal: Animal;
  experimentId: number;
  isTaken: boolean;

  userAnimalID: UntypedFormControl;
  gender: UntypedFormControl;
  strain: UntypedFormControl;
  genotype: UntypedFormControl;

  genoList: any;
  strainList: any;
  userAnimalLoadVal: any;


  constructor(public thisDialogRef: MatDialogRef<AnimalDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private animalService: AnimalService, private spinnerService: NgxSpinnerService,
    private fb: UntypedFormBuilder
  ) {
    this.userAnimalID = fb.control('', [Validators.required]);
    this.gender = fb.control('', [Validators.required]);
    this.strain = fb.control('', [Validators.required]);
    this.genotype = fb.control('', [Validators.required]);
    this.experimentId = 0;
    this.isTaken = false;
    this._animal = { animalID: 0, expID: 0, genotype: '', gID: 0, sex: '', sID: 0, strain: '', userAnimalID: '' };
  }

  ngOnInit() {

    this.animalService.getAllStraine().subscribe(data => {
      this.strainList = data;
    });
    // this.animalService.getAllGeno(this.strainModel).subscribe(data => { this.genoList = data; });

    // if it is the edit mode
    if (this.data.animalObj != null) {
      this.userAnimalLoadVal = this.data.animalObj.userAnimalID;
      this.userAnimalID.setValue(this.data.animalObj.userAnimalID);
      this.gender.setValue(this.data.animalObj.sex);
      this.strain.setValue(this.data.animalObj.sID);
      this.getgenoList(this.data.animalObj.sID);
      this.genotype.setValue(this.data.animalObj.gID);


    }




  }

  // Getting Selected strain ID and Pass it to get genoList
  selectedstrainChange(selectedstrainVal: number) {

    this.genotype.setValue([]);

    this.getgenoList(selectedstrainVal);

  }

  getgenoList(selectedstrainValue: number): any {

    this.animalService.getAllGeno(selectedstrainValue).subscribe(data => {
      this.genoList = data;
    });
  }

  onCloseCancel(): void {

    this.thisDialogRef.close('Cancel');
  }

  getErrorMessageId() {
    return this.userAnimalID.hasError('required') ? FIELDISREQUIRED : '';
  }
  getErrorMessageIdTaken() {
    return this.userAnimalID.hasError('taken') ? ANIMALIDTAKEN : '';
  }

  getErrorMessageGender() {
    return this.gender.hasError('required') ? FIELDISREQUIRED : '';
  }
  getErrorMessagestrain() {
    return this.strain.hasError('required') ? FIELDISREQUIRED : '';
  }
  getErrorMessagegenotype() {
    return this.genotype.hasError('required') ? FIELDISREQUIRED : '';
  }


  setDisabledVal() {

    if (this.userAnimalID.hasError('required') ||

            this.gender.hasError('required') ||
            this.strain.hasError('required') ||
            this.genotype.hasError('required')
    ) {
      return true;
    }
    return false;
  }


  onCloseSubmit(): void {

    this._animal.expID = this.data.experimentId;
    this._animal.userAnimalID = this.userAnimalID.value;
    this._animal.sex = this.gender.value;
    this._animal.sID = this.strain.value;
    this._animal.gID = this.genotype.value;

    if (this.data.animalObj === null) {   // Insert Mode: Insert Animal
      this.isTaken = false;

      this.animalService.createAnimal(this._animal).pipe(map(res => {
        if (res === TAKEN) {
          this.isTaken = true;
          this.userAnimalID.setErrors({ 'taken': true });
        } else {
          this.thisDialogRef.close(true);
        }
      })
      ).subscribe();

    } else {  // Edit Mode: Edit Animal

      // Check If userAnimalID has been edited
      if (this.userAnimalLoadVal.trim().toUpperCase() === this.userAnimalID.value.trim().toUpperCase()) {

        this._animal.animalID = this.data.animalObj.animalID;
        this.animalService.updateAniaml(this._animal).pipe(map((res: {id: string}) => {
          // close it so we can see the loading
          this.thisDialogRef.close(true);

        })
        ).subscribe();

      } else {

        // Check If edited userAnimalID exist in Table Animal
        this.animalService.isUserAnimalIDExist(this.userAnimalID.value.trim(), this.data.experimentId).pipe(map((res) => {
          if (!res) {
            alert('ERROR: ' + ANIMALIDDOESNOTEXIST);

          } else {
            // Edit userAnimalID based what exists in tbl Animal in Database
            this.animalService.editUserAnimalID(this.userAnimalID.value.trim(),
              this.data.animalObj.animalID, this.data.experimentId).pipe(map((res2) => {
              // close it so we can see the loading
              if (res2) {
                this.thisDialogRef.close(false);
              } else {
                alert(CANNOTSAVEEDITS);
              }

            })
            ).subscribe();
          }
        })
        ).subscribe();
      }




    }



  }




}
