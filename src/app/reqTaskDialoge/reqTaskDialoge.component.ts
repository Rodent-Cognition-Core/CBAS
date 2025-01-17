import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { SharedModule } from '../shared/shared.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

    selector: 'app-reqTaskDialoge',
    templateUrl: './reqTaskDialoge.component.html',
    styleUrls: ['./reqTaskDialoge.component.scss'],
    providers: [RequestService]

})
export class ReqTaskDialogeComponent implements OnInit {

    faQuestionCircle = faQuestionCircle;
    // Defining Models Parameters

    reqScheduleModel: string;
           
    private _request = new Request();

    // FormControl Parameters

    name: FormControl;
    task: FormControl;
    email: FormControl;
    

    constructor(public thisDialogRef: MatDialogRef<ReqTaskDialogeComponent>,
         
        private requestService: RequestService,
        private fb: FormBuilder) {

        this.name = fb.control('', [Validators.required]);
        this.task = fb.control('', [Validators.required]);
        this.email = fb.control('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
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
        this._request.taskName = this.task.value;
        this._request.scheduleName = this.reqScheduleModel;

        // Submiting the request to server
        this.requestService.addTask(this._request).subscribe( this.thisDialogRef.close() );
       
    }


    getErrorMessage()
    {

        return this.name.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageTask() {

        return this.task.hasError('required') ? FIELDISREQUIRED :
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
