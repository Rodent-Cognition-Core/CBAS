import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';
import { SharedModule } from '../shared/shared.module';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

    selector: 'app-reqPubTaskDialoge',
    templateUrl: './reqPubTaskDialoge.component.html',
    styleUrls: ['./reqPubTaskDialoge.component.scss'],
    providers: [RequestService, PubScreenService]

})
export class ReqPubTaskDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqNameModel: string;
    reqEmailModel: string;
    reqTaskCategoryModel: string;
    reqNewCategoryModel: string;
    reqDOIModel: string;
    reqNewTaskModel: string;
   
    taskCategoryList: any;

    private _request = new Request();

    // FormControl Parameters

    name = new FormControl('', [Validators.required]);
    email = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    taskCategory = new FormControl('', [Validators.required]);
    newCategory = new FormControl('', [Validators.required]);
    newTask = new FormControl('', [Validators.required]);
    doi = new FormControl('', [Validators.required]);

    constructor(public thisDialogRef: MatDialogRef<ReqPubTaskDialogeComponent>,

        private requestService: RequestService, private pubScreenService: PubScreenService) { }

    ngOnInit() {
        this.pubScreenService.getTask().subscribe(data => { this.taskCategoryList = data; });

    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.reqNameModel;
        this._request.email = this.reqEmailModel;
        this._request.doi = this.reqDOIModel;
        if (this.reqTaskCategoryModel != 'None') {
            this._request.taskCategory = this.reqTaskCategoryModel;
        }
        else {
            this._request.taskCategory = "NEW: " + this.reqNewCategoryModel;
        }
        this._request.taskName = this.reqNewTaskModel;

        // Submiting the request to server
        this.requestService.addPubTask(this._request).subscribe( this.thisDialogRef.close()); 
       
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

    getErrorMessageTaskCategory() {

        return this.taskCategory.hasError('required') ? FIELDISREQUIRED :
            '';

    }
    getErrorMessageNewCategory() {

        return this.newCategory.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    getErrorMessageNewTask() {
        return this.newTask.hasError('required') ? FIELDISREQUIRED :
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
            this.newTask.hasError('required') ||
            this.doi.hasError('required') ||
            this.taskCategory.hasError('required') ||
            (this.newCategory.hasError('required') && this.reqTaskCategoryModel == 'None')
            
            
        )
        {
            return true;
        }

        return false;
    }

        
}
