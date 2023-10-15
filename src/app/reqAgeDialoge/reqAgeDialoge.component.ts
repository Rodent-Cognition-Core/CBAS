import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { SharedModule } from '../shared/shared.module';



@Component({

    selector: 'app-reqAgeDialoge',
    templateUrl: './reqAgeDialoge.component.html',
    styleUrls: ['./reqAgeDialoge.component.scss'],
    providers: [RequestService]

})
export class ReqAgeDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqNameModel: string;
    reqEmailModel: string;
    reqAgeModel: string;
   
           
    private _request = new Request();

    // FormControl Parameters

    name = new FormControl('', [Validators.required]);
    email = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    age = new FormControl('', [Validators.required]);
    

    constructor(public thisDialogRef: MatDialogRef<ReqAgeDialogeComponent>,
         
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
        this._request.age = this.reqAgeModel;
      
        

        // Submiting the request to server
        this.requestService.addAge(this._request).subscribe( this.thisDialogRef.close()); 
       
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

    getErrorMessageAge() {
        return this.age.hasError('required') ? 'You must enter a value' :
            '';
    }

    

   
    setDisabledVal()
    {

        if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.age.hasError('required') 
            
            
        )
        {
            return true;
        }

        return false;
    }

        
}
