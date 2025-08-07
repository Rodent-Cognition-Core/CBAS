import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, UntypedFormBuilder } from '@angular/forms';
//import { NgModel } from '@angular/forms';
import { Animal } from '../models/animal';
import { AnimalService } from '../services/animal.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { forkJoin } from 'rxjs';
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

    userAnimalId: UntypedFormControl;
    gender: UntypedFormControl;
    strain: UntypedFormControl;
    genotype: UntypedFormControl;

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
        if (this.data.animalObj == null) {
            // Insert Mode for one or more animals
            this.isTaken = false;
            const animalIds = this.userAnimalId.value.split(',')
                .map((id: string) => id.trim())
                .filter(Boolean);

            if (animalIds.length === 0) {
                return; // Exit if no valid IDs are provided
            }

            const creationObservables = animalIds.map((id: string) => {
                const newAnimal: Animal = {
                    AnimalID: 0,
                    ExpID: this.data.experimentId,
                    UserAnimalID: id,
                    Sex: this.gender.value,
                    SID: this.strain.value,
                    GID: this.genotype.value,
                    Genotype: '',
                    Strain: ''
                };
                return this.animalService.createAnimal(newAnimal);
            });

            this.spinnerService.show();
            forkJoin<any[]>(creationObservables).subscribe(
                (results: any[]) => {
                    this.spinnerService.hide();
                    const takenIds = animalIds.filter((_id: string, index: number) => results[index] === TAKEN);

                    if (takenIds.length > 0) {
                        this.isTaken = true;
                        this.userAnimalId.setErrors({ 'taken': true });
                    } else {
                        this.thisDialogRef.close(true);
                    }
                },
                (err: any) => {
                    this.spinnerService.hide();
                    console.error('Error creating animals:', err);
                }
            );
        } else {
            // Edit Mode for a single animal
            const updatedAnimal: Animal = {
                AnimalID: this.data.animalObj.animalID,
                ExpID: this.data.experimentId,
                UserAnimalID: this.userAnimalId.value,
                Sex: this.gender.value,
                SID: this.strain.value,
                GID: this.genotype.value,
                Genotype: '',
                Strain: ''
            };

            const idHasChanged = this.userAnimalLoadVal.trim().toUpperCase() !== this.userAnimalId.value.trim().toUpperCase();

            const performUpdate = () => {
                this.animalService.updateAniaml(updatedAnimal).subscribe(
                    () => {
                        this.spinnerService.hide();
                        this.thisDialogRef.close(true);
                    },
                    (err) => {
                        this.spinnerService.hide();
                        console.error('Error updating animal:', err);
                        alert(CANNOTSAVEEDITS);
                    }
                );
            };

            this.spinnerService.show();
            if (idHasChanged) {
                this.animalService.IsUserAnimalIDExist(updatedAnimal.UserAnimalID, updatedAnimal.ExpID).subscribe(isTaken => {
                    if (isTaken) {
                        this.userAnimalId.setErrors({ 'taken': true });
                        this.spinnerService.hide();
                    } else {
                        performUpdate();
                    }
                });
            } else {
                performUpdate();
            }
        }
    }
}