import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, UntypedFormBuilder } from '@angular/forms';
//import { NgModel } from '@angular/forms';
import { Animal } from '../models/animal';
import { AnimalService } from '../services/animal.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { map, /*catchError*/ } from 'rxjs/operators'
import { ANIMALIDDOESNOTEXIST, ANIMALIDTAKEN, CANNOTSAVEEDITS, FIELDISREQUIRED, TAKEN } from '../shared/messages';

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
    isTimeSeries: boolean;

    userAnimalId: UntypedFormControl;
    gender: UntypedFormControl;
    strain: UntypedFormControl;
    strainTimeSeries: UntypedFormControl;
    genotype: UntypedFormControl;
    genotypeTimeSeries: UntypedFormControl;

    GenoList: any;
    StrainList: any;
    userAnimalLoadVal: any;


    constructor(public thisDialogRef: MatDialogRef<AnimalDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any, private animalService: AnimalService, private spinnerService: NgxSpinnerService,
        private fb: UntypedFormBuilder
    ) {
        this.userAnimalId = fb.control('', [Validators.required])
        this.gender = fb.control('', [Validators.required])
        this.strain = fb.control('', [Validators.required])
        this.strainTimeSeries = fb.control('', [Validators.required])
        this.genotype = fb.control('', [Validators.required])
        this.genotypeTimeSeries = fb.control('', [Validators.required])
        this.experimentId = 0;
        this.isTaken = false;
        this.isTimeSeries = data.isTimeSeries;
        this._animal = { AnimalID: 0, ExpID: 0, Genotype: '', GID: 0, Sex: '', SID: 0, Strain: '', UserAnimalID: '' }
    }

    ngOnInit() {

        this.animalService.getAllStraine().subscribe(data => { 
            this.StrainList = data; 
            if (this.isTimeSeries) {
                this.StrainList.push({ id: -1, strain: 'Other', link: '' });
            }

            // if it is the edit mode
            if (this.data.animalObj != null) {
                if (this.isTimeSeries) {
                    const foundStrain = this.StrainList.find((s: any) => s.strain === this.data.animalObj.strain);
                    if (foundStrain) {
                        this.strain.setValue(foundStrain.id);
                        this.getGenoList(foundStrain.id);
                    } else {
                        this.strain.setValue(-1);
                        this.strainTimeSeries.setValue(this.data.animalObj.strain);
                        this.getGenoList(-1);
                    }
                } else {
                    this.strain.setValue(this.data.animalObj.sid);
                    this.getGenoList(this.data.animalObj.sid);
                }
            }
        });

        // if it is the edit mode
        if (this.data.animalObj != null) {
            this.userAnimalLoadVal = this.data.animalObj.userAnimalID;
            this.userAnimalId.setValue(this.data.animalObj.userAnimalID);
            this.gender.setValue(this.data.animalObj.sex);
        }

    }

    // Getting Selected Strain ID and Pass it to get Genolist
    selectedStrainChange(selectedStrainVal: number) {

        this.genotype.setValue([]);

        this.getGenoList(selectedStrainVal);

    }

    getGenoList(selected_StrainValue: number): any {
        if (selected_StrainValue === -1) {
            this.GenoList = [];
            if (this.isTimeSeries) {
                this.GenoList.push({ id: -1, genotype: 'Other', link: '', description: '' });
                if (this.data.animalObj != null) {
                    const foundGeno = this.GenoList.find((g : any) => g.genotype === this.data.animalObj.genotype);
                    if (foundGeno) {
                        this.genotype.setValue(foundGeno.id);
                    } else {
                        this.genotype.setValue(-1);
                        this.genotypeTimeSeries.setValue(this.data.animalObj.genotype);
                    }
                }
            }
            return;
        }
        this.animalService.getAllGeno(selected_StrainValue).subscribe(data => { 
            this.GenoList = data; 
            if (this.isTimeSeries) {
                this.GenoList.push({ id: -1, genotype: 'Other', link: '', description: '' });
            }

            if (this.data.animalObj != null) {
                if (this.isTimeSeries) {
                    const foundGeno = this.GenoList.find((g : any) => g.genotype === this.data.animalObj.genotype);
                    if (foundGeno) {
                        this.genotype.setValue(foundGeno.id);
                    } else {
                        this.genotype.setValue(-1);
                        this.genotypeTimeSeries.setValue(this.data.animalObj.genotype);
                    }
                } else {
                    this.genotype.setValue(this.data.animalObj.gid);
                }
            }
        });
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

        if (this.isTimeSeries) {
            if (this.strain.value === -1 && this.strainTimeSeries.hasError('required')) {
                return true;
            }
            if (this.genotype.value === -1 && this.genotypeTimeSeries.hasError('required')) {
                return true;
            }
        }

        return false;
    }


    onCloseSubmit(): void {

        this._animal.ExpID = this.data.experimentId;
        this._animal.UserAnimalID = this.userAnimalId.value;
        this._animal.Sex = this.gender.value;
        if (this.isTimeSeries) {
            if (this.strain.value === -1) {
                this._animal.Strain = this.strainTimeSeries.value;
            } else {
                const selectedStrain = this.StrainList.find((s: any) => s.id === this.strain.value);
                this._animal.Strain = selectedStrain ? selectedStrain.strain : '';
            }

            if (this.genotype.value === -1) {
                this._animal.Genotype = this.genotypeTimeSeries.value;
            } else {
                const selectedGeno = this.GenoList.find((g: any) => g.id === this.genotype.value);
                this._animal.Genotype = selectedGeno ? selectedGeno.genotype : '';
            }
        } else {
            this._animal.SID = this.strain.value;
            this._animal.GID = this.genotype.value;
        }

        if (this.data.animalObj == null) {   // Insert Mode: Insert Animal
            this.isTaken = false;

            if (this.isTimeSeries) {
                this.animalService.createAnimalTimeSeries(this._animal).pipe(map(res => {
                if (res == TAKEN) {
                    this.isTaken = true;
                    this.userAnimalId.setErrors({ 'taken': true });
                } else {
                    this.thisDialogRef.close(true);
                }
            })
            ).subscribe();
            } else {
                this.animalService.createAnimal(this._animal).pipe(map(res => {
                if (res == TAKEN) {
                    this.isTaken = true;
                    this.userAnimalId.setErrors({ 'taken': true });
                } else {
                    this.thisDialogRef.close(true);
                }
            })
            ).subscribe();
            }

        } else {  // Edit Mode: Edit Animal

            if (this.isTimeSeries) 
                {
                    if (this.userAnimalLoadVal.trim().toUpperCase() == this.userAnimalId.value.trim().toUpperCase()) 
                        {
                          this._animal.AnimalID = this.data.animalObj.animalID;
                            this.animalService.updateAnimalTimeSeries(this._animal).pipe(map((_res: {id: string}) => {
                            // close it so we can see the loading
                            this.thisDialogRef.close(true);

                            })
                            ).subscribe();  
                        }
                        else {
                            this.animalService.IsUserAnimalTimeSeriesIDExist(this.userAnimalId.value.trim(), this.data.experimentId).pipe(map((res) => {
                            if (!res) {
                                alert("ERROR: " + ANIMALIDDOESNOTEXIST);

                            } else {
                                // Edit UserAnimalId based what exists in tbl Animal in Database
                            this.animalService.EditUserAnimalTimeSeriesID(this.userAnimalId.value.trim(), this.data.animalObj.animalID, this.data.experimentId).pipe(map((res) => {
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
                } else {
                   if (this.userAnimalLoadVal.trim().toUpperCase() == this.userAnimalId.value.trim().toUpperCase()) 
                        {
                          this._animal.AnimalID = this.data.animalObj.animalID;
                            this.animalService.updateAnimal(this._animal).pipe(map((_res: {id: string}) => {
                            // close it so we can see the loading
                            this.thisDialogRef.close(true);

                            })
                            ).subscribe();  
                        }
                        else {
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




}
