import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
//import { NgModel } from '@angular/forms';
import { Animal } from '../models/animal';
import { AnimalService } from '../services/animal.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { map, /*catchError*/ } from 'rxjs/operators'
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

    userAnimalId: FormControl;
    gender: FormControl;
    strain: FormControl;
    genotype: FormControl;

    GenoList: any;
    StrainList: any;
    userAnimalLoadVal: any;


    constructor(public thisDialogRef: MatDialogRef<AnimalDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any, private animalService: AnimalService, private spinnerService: NgxSpinnerService,
        private fb: FormBuilder
    ) {
        this.userAnimalId = fb.control('', [Validators.required])
        this.gender = fb.control('', [Validators.required])
        this.strain = fb.control('', [Validators.required])
        this.genotype = fb.control('', [Validators.required])
        this.experimentId = 0;
        this.isTaken = false;
        this._animal = { AnimalID: 0, ExpID: 0, Genotype: '', GID: 0, Sex: '', SID: 0, Strain: '', UserAnimalID: '' }
    }

    ngOnInit() {

        this.animalService.getAllStraine().subscribe(data => { this.StrainList = data; });
        //this.animalService.getAllGeno(this.strainModel).subscribe(data => { this.GenoList = data; });

        // if it is the edit mode
        if (this.data.animalObj != null) {
            this.userAnimalLoadVal = this.data.animalObj.userAnimalID;
            this.userAnimalId.setValue(this.data.animalObj.userAnimalID);
            this.gender.setValue(this.data.animalObj.sex);
            this.strain.setValue(this.data.animalObj.sid);
            this.getGenoList(this.data.animalObj.sid);
            this.genotype.setValue(this.data.animalObj.gid);
            
           
        }

        


    }

    // Getting Selected Strain ID and Pass it to get Genolist
    selectedStrainChange(selectedStrainVal: number) {

        this.genotype.setValue([]);

        this.getGenoList(selectedStrainVal);

    }

    getGenoList(selected_StrainValue: number): any {

        this.animalService.getAllGeno(selected_StrainValue).subscribe(data => { this.GenoList = data; });
    }

    onCloseCancel(): void {

        this.thisDialogRef.close('Cancel');
    }

    getErrorMessageId() {
        return this.userAnimalId.hasError('required') ? FIELDISREQUIRED : '';
    }
    getErrorMessageIdTaken() {
        return this.userAnimalId.hasError('taken') ? ANIMALIDTAKEN : '';
    }

    getErrorMessageGender() {
        return this.gender.hasError('required') ? FIELDISREQUIRED : '';
    }
    getErrorMessageStrain() {
        return this.strain.hasError('required') ? FIELDISREQUIRED : '';
    }
    getErrorMessageGenotype() {
        return this.genotype.hasError('required') ? FIELDISREQUIRED : '';
    }


    setDisabledVal() {

        if (this.userAnimalId.hasError('required') ||

            this.gender.hasError('required') ||
            this.strain.hasError('required') ||
            this.genotype.hasError('required')
        ) {
            return true;
        }
        return false;
    }


    onCloseSubmit(): void {

        this._animal.ExpID = this.data.experimentId;
        this._animal.UserAnimalID = this.userAnimalId.value;
        this._animal.Sex = this.gender.value;
        this._animal.SID = this.strain.value;
        this._animal.GID = this.genotype.value;

        if (this.data.animalObj == null) {   // Insert Mode: Insert Animal
            this.isTaken = false;

            this.animalService.createAnimal(this._animal).pipe(map(res => {
                if (res == TAKEN) {
                    this.isTaken = true;
                    this.userAnimalId.setErrors({ 'taken': true });
                } else {
                    this.thisDialogRef.close(true);
                }
            })
            ).subscribe();

        } else {  // Edit Mode: Edit Animal

            // Check If UserAnimalID has been edited
            if (this.userAnimalLoadVal.trim().toUpperCase() == this.userAnimalId.value.trim().toUpperCase()) {

                this._animal.AnimalID = this.data.animalObj.animalID;
                this.animalService.updateAniaml(this._animal).pipe(map((res: {id: string}) => {
                    // close it so we can see the loading
                    this.thisDialogRef.close(true);

                })
             ).subscribe();

            }
            else {

                // Check If edited UserAnimalID exist in Table Animal
                this.animalService.IsUserAnimalIDExist(this.userAnimalId.value.trim(), this.data.experimentId).pipe(map((res) => {
                    if (!res) {
                        alert("ERROR: " + ANIMALIDDOESNOTEXIST);

                    } else {
                        // Edit UserAnimalId based what exists in tbl Animal in Database
                        this.animalService.EditUserAnimalID(this.userAnimalId.value.trim(), this.data.animalObj.animalID, this.data.experimentId).pipe(map((res) => {
                            // close it so we can see the loading
                            if (res) {
                                this.thisDialogRef.close(false);
                            }
                            else {
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
