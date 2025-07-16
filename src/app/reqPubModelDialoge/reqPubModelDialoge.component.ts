import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, UntypedFormBuilder } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';



@Component({

    selector: 'app-req-pubmodel-dialoge',
    templateUrl: './reqPubModelDialoge.component.html',
    styleUrls: ['./reqPubModelDialoge.component.scss'],
    providers: [RequestService, PubScreenService]

})
export class ReqPubModelDialogeComponent implements OnInit {
   
    modelList: any;

    private _request: Request;

    // UntypedFormControl Parameters

    name: UntypedFormControl;
    email: UntypedFormControl;
    rodentModel: UntypedFormControl;
    newSubModel: UntypedFormControl;
    doi: UntypedFormControl;

    constructor(public thisDialogRef: MatDialogRef<ReqPubModelDialogeComponent>,

        private requestService: RequestService, private pubScreenService: PubScreenService,
        private fb: UntypedFormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: any) {

        this.name = fb.control('', [Validators.required]);
        this.email = fb.control('', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]);
        this.rodentModel = fb.control('', [Validators.required]);
        this.newSubModel = fb.control('', [Validators.required]);
        this.doi = fb.control('', [Validators.required]);
        this._request = {
            controlSuggestion: '', doi: '', email: '', fullName: '', generalRequest: '', geneticModification: '', genotype: '', ID: 0,
            method: '', model: '', mouseStrain: '', piEmail: '', piFullName: '', piInstitution: '', scheduleName: '', strainReference: '', subMethod: '',
            subModel: '', taskCategory: '', taskName: '', type: ''
        }
    }

    ngOnInit() {
        this.pubScreenService.getDisease().subscribe((data : any) => { this.modelList = data; });

    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.name.value;
        this._request.email = this.email.value;
        this._request.doi = this.doi.value;
        this._request.model = this.rodentModel.value;
        this._request.subModel = this.newSubModel.value;

        // Submiting the request to server
        this.requestService.addPubSubModel(this._request).subscribe( this.thisDialogRef.close()); 
       
    }


    getErrorMessage()
    {

        return this.name.hasError('required') ? 'You must enter a value' :
            '';
    }

    getErrorMessageEmail()
    {

        return this.email.hasError('required') ? 'You must enter a value' :
            '';

    }
    
    getErrorMessageEmailValid()
    {

        return this.email.hasError('pattern') ? 'Enter Valid Email Address' :
            '';
    }

    getErrorMessageModel() {

        return this.rodentModel.hasError('required') ? 'You must select a value' :
            '';

    }

    getErrorMessageNewSubModel() {
        return this.newSubModel.hasError('required') ? 'You must enter a value' :
            '';
    }

    getErrorMessageDOI() {
        return this.doi.hasError('required') ? 'You must enter a value' :
            '';
    }

    

   
    setDisabledVal()
    {

        if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.newSubModel.hasError('required') ||
            this.doi.hasError('required') ||
            this.rodentModel.hasError('required')
        )
        {
            return true;
        }

        return false;
    }

        
}
