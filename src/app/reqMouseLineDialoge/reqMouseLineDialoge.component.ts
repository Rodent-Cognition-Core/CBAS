import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
//import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
//import { SharedModule } from '../shared/shared.module';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

    selector: 'app-reqMouseLineDialoge',
    templateUrl: './reqMouseLineDialoge.component.html',
    styleUrls: ['./reqMouseLineDialoge.component.scss'],
    providers: [RequestService]

})
export class ReqMouseLineDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqGenoModel: string;
    refModel: string
    controlModel: string
   
   // DEfining obj model parameters        
    private _request: Request;

    // FormControl Parameters

    name: FormControl;
    email: FormControl;
    strain: FormControl;
    geneticModi: FormControl;
    //control = new FormControl('', [Validators.required]);
    

    // List of GeneticModification for Mouse line
    public geneticModification: any[] = [
           
        { name: 'Transgenic' },
        { name: 'WildType' },
        { name: 'KnockIn' },
        { name: 'KnockOut' }
        
    ];
    

    constructor(public thisDialogRef: MatDialogRef<ReqMouseLineDialogeComponent>,
         
        private requestService: RequestService,
        private fb: FormBuilder) {

        this.reqGenoModel = '';
        this.refModel = '';
        this.controlModel = '';
        this.name = fb.control('', [Validators.required]);
        this.email = fb.control('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
        this.strain = fb.control('', [Validators.required]);
        this.geneticModi = fb.control('', [Validators.required])
        this._request = {
            age: '', controlSuggestion: '', doi: '', email: '', fullName: '', generalRequest: '', geneticModification: '', genotype: '', ID: 0,
            method: '', model: '', mouseStrain: '', piEmail: '', piFullName: '', piInstitution: '', scheduleName: '', strainReference: '', subMethod: '',
            subModel: '', taskCategory: '', taskName: '', type: ''
        }
    }

    ngOnInit() {
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.name.value;
        this._request.email = this.email.value;
        this._request.mouseStrain = this.strain.value;
        this._request.genotype = this.reqGenoModel;
        this._request.geneticModification = this.geneticModi.value;
        this._request.strainReference = this.refModel;
        this._request.controlSuggestion = this.controlModel;

        
        // Submiting the request to server
        this.requestService.addMouseLine(this._request).subscribe( this.thisDialogRef.close()); 
       
    }


    getErrorMessage()
    {

        return this.name.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageEmail()
    {

        return this.email.hasError('required') ? FIELDISREQUIRED :
            '';

    }
    
    getErrorMessageEmailValid()
    {

        return this.email.hasError('pattern') ? INVALIDEMAILADDRESS :
            '';
    }

    getErrorMessageStrain() {
        return this.strain.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageGM() {

        return this.geneticModi.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    //getErrorMessageControl() {
    //    return this.control.hasError('required') ? 'You must enter a value' :
    //        '';
    //}

      
    setDisabledVal()
    {

        if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.strain.hasError('required') ||
            this.geneticModi.hasError('required')
                                   
        )

        {
            return true;
        }

        return false;
    }

        
}
