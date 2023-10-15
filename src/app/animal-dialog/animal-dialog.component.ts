import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Animal } from '../models/animal';
import { AnimalService } from '../services/animal.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
    selector: 'app-animal-dialog',
    templateUrl: './animal-dialog.component.html',
    styleUrls: ['./animal-dialog.component.scss'],
    providers: [AnimalService]
})
export class AnimalDialogComponent implements OnInit {

    _animal = new Animal();
    experimentId: number;
    userAnimalIdModel: string;
    genderModel: string;
    strainModel: any;
    genotypeModel: any;
    isTaken: boolean;

    GenoList: any;
    StrainList: any;
    userAnimalLoadVal: any;


    constructor(public thisDialogRef: MatDialogRef<AnimalDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any, private animalService: AnimalService, private spinnerService: Ng4LoadingSpinnerService) { }

    ngOnInit() {

        this.animalService.getAllStraine().subscribe(data => { this.StrainList = data; });
        //this.animalService.getAllGeno(this.strainModel).subscribe(data => { this.GenoList = data; });

        // if it is the edit mode
        if (this.data.animalObj != null) {
            this.userAnimalLoadVal = this.data.animalObj.userAnimalID;
            this.userAnimalIdModel = this.data.animalObj.userAnimalID;
            this.genderModel = this.data.animalObj.sex;
            this.strainModel = this.data.animalObj.sid;
            this.getGenoList(this.data.animalObj.sid);
            this.genotypeModel = this.data.animalObj.gid;
            
           
        }

        


    }

    // Getting Selected Strain ID and Pass it to get Genolist
    selectedStrainChange(selectedStrainVal) {

        this.genotypeModel = [];

        this.getGenoList(selectedStrainVal);

    }

    getGenoList(selected_StrainValue): any {

        this.animalService.getAllGeno(selected_StrainValue).subscribe(data => { this.GenoList = data; });
    }

    onCloseCancel(): void {

        this.thisDialogRef.close('Cancel');
    }

    userAnimalId = new FormControl('', [Validators.required]);
    gender = new FormControl('', [Validators.required]);
    strain = new FormControl('', [Validators.required]);
    genotype = new FormControl('', [Validators.required]);

    getErrorMessageId() {
        return this.userAnimalId.hasError('required') ? 'Animal ID is required' : '';
    }
    getErrorMessageIdTaken() {
        return this.userAnimalId.hasError('taken') ? 'This Animal Id already exists in this experiment!' : '';
    }

    getErrorMessageGender() {
        return this.gender.hasError('required') ? 'Animal sex is required' : '';
    }
    getErrorMessageStrain() {
        return this.strain.hasError('required') ? 'Strain is required' : '';
    }
    getErrorMessageGenotype() {
        return this.genotype.hasError('required') ? 'Genotype is required' : '';
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
        this._animal.UserAnimalID = this.userAnimalIdModel;
        this._animal.Sex = this.genderModel;
        this._animal.SID = this.strainModel;
        this._animal.GID = this.genotypeModel;

        if (this.data.animalObj == null) {   // Insert Mode: Insert Animal
            this.isTaken = false;

            this.animalService.createAnimal(this._animal).map(res => {
                if (res == "Taken") {
                    this.isTaken = true;
                    this.userAnimalId.setErrors({ 'taken': true });
                } else {
                    this.thisDialogRef.close(true);
                }
            }).subscribe();

        } else {  // Edit Mode: Edit Animal

            // Check If UserAnimalID has been edited
            if (this.userAnimalLoadVal.trim().toUpperCase() == this.userAnimalIdModel.trim().toUpperCase()) {

                this._animal.AnimalID = this.data.animalObj.animalID;
                this.animalService.updateAniaml(this._animal).map(res => {
                    // close it so we can see the loading
                    this.thisDialogRef.close(true);

                }).subscribe();

            }
            else {

                // Check If edited UserAnimalID exist in Table Animal
                this.animalService.IsUserAnimalIDExist(this.userAnimalIdModel.trim(), this.data.experimentId).map(res => {
                    if (res == "Not Exist") {
                        alert("ERROR! The edited Animal Id does not Exist in Database");

                    } else {
                        // Edit UserAnimalId based what exists in tbl Animal in Database
                        this.animalService.EditUserAnimalID(this.userAnimalIdModel.trim(), this.data.animalObj.animalID, this.data.experimentId).map(res => {
                            // close it so we can see the loading
                            if (res == "Successful") {
                                this.thisDialogRef.close(false);
                            }
                            else {
                                alert("Edit cannot be performed!")
                            }

                        }).subscribe();
                    }
                }).subscribe();
            }




        }



    }




}
