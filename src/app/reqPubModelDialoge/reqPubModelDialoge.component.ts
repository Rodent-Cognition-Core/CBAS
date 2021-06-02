import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';
import { SharedModule } from '../shared/shared.module';



@Component({

    selector: 'app-reqPubModelDialoge',
    templateUrl: './reqPubModelDialoge.component.html',
    styleUrls: ['./reqPubModelDialoge.component.scss'],
    providers: [RequestService, PubScreenService]

})
export class ReqPubModelDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqNameModel: string;
    reqEmailModel: string;
    reqRodentModel: string;
    reqDOIModel: string;
    reqNewSubModel: string;
   
    modelList: any;

    private _request = new Request();

    // FormControl Parameters

    name = new FormControl('', [Validators.required]);
    email = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    rodentModel = new FormControl('', [Validators.required]);
    newSubModel = new FormControl('', [Validators.required]);
    doi = new FormControl('', [Validators.required]);

    constructor(public thisDialogRef: MatDialogRef<ReqPubModelDialogeComponent>,

        private requestService: RequestService, private pubScreenService: PubScreenService) { }

    ngOnInit() {
        this.pubScreenService.getDisease().subscribe(data => { this.modelList = data; });

    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.reqNameModel;
        this._request.email = this.reqEmailModel;
        this._request.doi = this.reqDOIModel;
        this._request.model = this.reqRodentModel;
        this._request.subModel = this.reqNewSubModel;

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
