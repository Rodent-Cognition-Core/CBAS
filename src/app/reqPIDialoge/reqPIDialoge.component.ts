import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { SharedModule } from '../shared/shared.module';



@Component({

    selector: 'app-reqPIDialoge',
    templateUrl: './reqPIDialoge.component.html',
    styleUrls: ['./reqPIDialoge.component.scss'],
    providers: [RequestService]

})
export class ReqPIDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqNameModel: string;
    reqEmailModel: string;
    reqPINameModel: string;
    reqPIEmailModel: string;
    reqInsNameModel: string;

           
    private _request = new Request();

    // FormControl Parameters

    name = new FormControl('', [Validators.required]);
    email = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    emailPI = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    piName = new FormControl('', [Validators.required]);
    institution = new FormControl('', [Validators.required]);

    constructor(public thisDialogRef: MatDialogRef<ReqPIDialogeComponent>,
         
        private requestService: RequestService, ) { }

    ngOnInit() {
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.reqNameModel;
        this._request.email = this.reqEmailModel;
        this._request.piFullName = this.reqPINameModel;
        this._request.piEmail = this.reqPIEmailModel;
        this._request.piInstitution = this.reqInsNameModel;
        

        // Submiting the request to server
        this.requestService.addPI(this._request).subscribe( this.thisDialogRef.close()); 
       
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

    getErrorMessagePIName() {
        return this.piName.hasError('required') ? 'You must enter a value' :
            '';
    }

    getErrorMessageIns() {
        return this.institution.hasError('required') ? 'You must enter a value' :
            '';
    }

    getErrorMessagePIEmail() {

        return this.emailPI.hasError('required') ? 'You must enter a value' :
            '';

    }

    getErrorMessagePIEmailValid() {

        return this.emailPI.hasError('pattern') ? 'Enter Valid Email Address' :
            '';
    }



   
    setDisabledVal()
    {

        if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.emailPI.hasError('required') ||
            this.emailPI.hasError('pattern') ||
            this.piName.hasError('required') ||
            this.institution.hasError('required')
            
        )
        {
            return true;
        }

        return false;
    }

        
}
