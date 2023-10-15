import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { SharedModule } from '../shared/shared.module';



@Component({

    selector: 'app-reqTaskDialoge',
    templateUrl: './reqTaskDialoge.component.html',
    styleUrls: ['./reqTaskDialoge.component.scss'],
    providers: [RequestService]

})
export class ReqTaskDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqNameModel: string;
    reqEmailModel: string;
    reqTaskModel: string;
    reqScheduleModel: string;
           
    private _request = new Request();

    // FormControl Parameters

    name = new FormControl('', [Validators.required]);
    task = new FormControl('', [Validators.required]);
    email = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    

    constructor(public thisDialogRef: MatDialogRef<ReqTaskDialogeComponent>,
         
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
        this._request.taskName = this.reqTaskModel;
        this._request.scheduleName = this.reqScheduleModel;

        // Submiting the request to server
        this.requestService.addTask(this._request).subscribe( this.thisDialogRef.close() );
       
    }


    getErrorMessage()
    {

        return this.name.hasError('required') ? 'You must enter a value' :
            '';
    }

    getErrorMessageTask() {

        return this.task.hasError('required') ? 'You must enter a value' :
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

   
    setDisabledVal()
    {

        if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.task.hasError('required')
            
        )
        {
            return true;
        }

        return false;
    }

        
}
