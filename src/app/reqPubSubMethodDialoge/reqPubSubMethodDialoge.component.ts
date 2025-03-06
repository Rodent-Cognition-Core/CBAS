import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
//import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';
//import { SharedModule } from '../shared/shared.module';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

    selector: 'app-reqPubSubMethodDialoge',
    templateUrl: './reqPubSubMethodDialoge.component.html',
    styleUrls: ['./reqPubSubMethodDialoge.component.scss'],
    providers: [RequestService, PubScreenService]

})
export class ReqPubSubMethodDialogeComponent implements OnInit {
   
    methodList: any;

    private _request: Request;

    // FormControl Parameters

    name: FormControl;
    email: FormControl;
    method: FormControl;
    newSubMethod: FormControl;
    doi: FormControl;

    constructor(public thisDialogRef: MatDialogRef<ReqPubSubMethodDialogeComponent>,

        private requestService: RequestService, private pubScreenService: PubScreenService,
        private fb: FormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: any) {

        this.name = fb.control('', [Validators.required]);
        this.email = fb.control('', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]);
        this.method = fb.control('', [Validators.required]);
        this.newSubMethod = fb.control('', [Validators.required]);
        this.doi = fb.control('', [Validators.required]);
        this._request = {
            age: '', controlSuggestion: '', doi: '', email: '', fullName: '', generalRequest: '', geneticModification: '', genotype: '', ID: 0,
            method: '', model: '', mouseStrain: '', piEmail: '', piFullName: '', piInstitution: '', scheduleName: '', strainReference: '', subMethod: '',
            subModel: '', taskCategory: '', taskName: '', type: ''
        }
    }

    ngOnInit() {
        this.pubScreenService.getMethod().subscribe((data : any) => { this.methodList = data; });

    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.name.value;
        this._request.email = this.email.value;
        this._request.doi = this.doi.value;
        this._request.method = this.method.value;
        this._request.subMethod = this.newSubMethod.value;

        // Submiting the request to server
        this.requestService.addPubSubMethod(this._request).subscribe( this.thisDialogRef.close()); 
       
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

    getErrorMessageMethod() {

        return this.method.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    getErrorMessageNewSubMethod() {
        return this.newSubMethod.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageDOI() {
        return this.doi.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    

   
    setDisabledVal()
    {

        if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.newSubMethod.hasError('required') ||
            this.doi.hasError('required') ||
            this.method.hasError('required')
        )
        {
            return true;
        }

        return false;
    }

        
}
