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
        
        this.experimentId = 0;
        this.isTaken = false;
        this.isTimeSeries = data.isTimeSeries;
        this._animal = { AnimalID: 0, ExpID: 0, Genotype: '', GID: 0, Sex: '', SID: 0, Strain: '', UserAnimalID: '' }

        if (this.isTimeSeries) {
            this.strain = fb.control('');
            this.strainTimeSeries = fb.control('', [Validators.required]);
            this.genotype = fb.control('');
            this.genotypeTimeSeries = fb.control('', [Validators.required]);
        } else {
            this.strain = fb.control('', [Validators.required]);
            this.strainTimeSeries = fb.control('');
            this.genotype = fb.control('', [Validators.required]);
            this.genotypeTimeSeries = fb.control('');
        }
    }

    ngOnInit() {

        this.animalService.getAllStraine().subscribe(data => { 
            this.StrainList = data; 
            
            // if it is the edit mode
            if (this.data.animalObj != null) {
                if (this.isTimeSeries) {
                    this.strainTimeSeries.setValue(this.data.animalObj.strain);
                    this.genotypeTimeSeries.setValue(this.data.animalObj.genotype);

                    const foundStrain = this.StrainList.find((s: any) => s.strain === this.data.animalObj.strain);
                    if (foundStrain) {
                        this.getGenoList(foundStrain.id, true);
                    }
                } else {
                    this.strain.setValue(this.data.animalObj.sid);
                    this.getGenoList(this.data.animalObj.sid, true);
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
        this.getGenoList(selectedStrainVal, false);
    }
    
    onStrainSelection(event: any) {
        const strainName = event.option.value;
        const found = this.StrainList.find((s: any) => s.strain === strainName);
        if (found) {
            this.getGenoList(found.id, false);
        } else {
            this.GenoList = [];
        }
    }
    
    onStrainInput(strainVal: string) {
        const found = this.StrainList.find((s: any) => s.strain === strainVal);
        if (found) {
            this.getGenoList(found.id, false);
        } else {
            this.GenoList = [];
        }
    }

    getGenoList(selected_StrainValue: number, isInit: boolean = false): any {
        if (selected_StrainValue === -1) {
            this.GenoList = [];
            return;
        }
        this.animalService.getAllGeno(selected_StrainValue).subscribe(data => { 
            this.GenoList = data; 
            
            if (isInit && this.data.animalObj != null) {
                if (!this.isTimeSeries) {
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
        if (this.isTimeSeries) {
            return this.strainTimeSeries.hasError('required') ? FIELDISREQUIRED : '';
        }
        return this.strain.hasError('required') ? FIELDISREQUIRED : '';
    }
    getErrorMessageGenotype() {
        if (this.isTimeSeries) {
            return this.genotypeTimeSeries.hasError('required') ? FIELDISREQUIRED : '';
        }
        return this.genotype.hasError('required') ? FIELDISREQUIRED : '';
    }


    setDisabledVal() {

        if (this.userAnimalId.hasError('required') ||

            this.gender.hasError('required')
        ) {
            return true;
        }

        if (this.isTimeSeries) {
            if (this.strainTimeSeries.hasError('required') ||
                this.genotypeTimeSeries.hasError('required')) {
                return true;
            }
        } else {
            if (this.strain.hasError('required') ||
                this.genotype.hasError('required')) {
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
            this._animal.Strain = this.strainTimeSeries.value;
            this._animal.Genotype = this.genotypeTimeSeries.value;
        } else {
            this._animal.SID = this.strain.value;
            this._animal.GID = this.genotype.value;
             const selectedStrain = this.StrainList.find((s: any) => s.id === this.strain.value);
             this._animal.Strain = selectedStrain ? selectedStrain.strain : '';
             const selectedGeno = this.GenoList.find((g: any) => g.id === this.genotype.value);
             this._animal.Genotype = selectedGeno ? selectedGeno.genotype : '';
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
