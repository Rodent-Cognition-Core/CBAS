import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';
import { SharedModule } from '../shared/shared.module';



@Component({

    selector: 'app-reqPubSubMethodDialoge',
    templateUrl: './reqPubSubMethodDialoge.component.html',
    styleUrls: ['./reqPubSubMethodDialoge.component.scss'],
    providers: [RequestService, PubScreenService]

})
export class ReqPubSubMethodDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqNameMethod: string;
    reqEmailMethod: string;
    reqMethod: string;
    reqDOIMethod: string;
    reqNewSubMethod: string;
   
    methodList: any;

    private _request = new Request();

    // FormControl Parameters

    name = new FormControl('', [Validators.required]);
    email = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    method = new FormControl('', [Validators.required]);
    newSubMethod = new FormControl('', [Validators.required]);
    doi = new FormControl('', [Validators.required]);

    constructor(public thisDialogRef: MatDialogRef<ReqPubSubMethodDialogeComponent>,

        private requestService: RequestService, private pubScreenService: PubScreenService) { }

    ngOnInit() {
        this.pubScreenService.getMethod().subscribe(data => { this.methodList = data; });

    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.reqNameMethod;
        this._request.email = this.reqEmailMethod;
        this._request.doi = this.reqDOIMethod;
        this._request.method = this.reqMethod;
        this._request.subMethod = this.reqNewSubMethod;

        // Submiting the request to server
        this.requestService.addPubSubMethod(this._request).subscribe( this.thisDialogRef.close()); 
       
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

    getErrorMessageMethod() {

        return this.method.hasError('required') ? 'You must select a value' :
            '';

    }

    getErrorMessageNewSubMethod() {
        return this.newSubMethod.hasError('required') ? 'You must enter a value' :
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
