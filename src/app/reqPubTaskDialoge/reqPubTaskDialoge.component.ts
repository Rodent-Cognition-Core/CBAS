import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, UntypedFormBuilder } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

    selector: 'app-req-pubtask-dialoge',
    templateUrl: './reqPubTaskDialoge.component.html',
    styleUrls: ['./reqPubTaskDialoge.component.scss'],
    providers: [RequestService, PubScreenService]

})
export class ReqPubTaskDialogeComponent implements OnInit {
   
    taskCategoryList: any;

    private _request: Request;

    // UntypedFormControl Parameters

    name: UntypedFormControl;
    email: UntypedFormControl;
    taskCategory: UntypedFormControl;
    newCategory: UntypedFormControl;
    newTask: UntypedFormControl;
    doi: UntypedFormControl;

    constructor(public thisDialogRef: MatDialogRef<ReqPubTaskDialogeComponent>,

        private requestService: RequestService, private pubScreenService: PubScreenService,
        private fb: UntypedFormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: any) {

        this.name = fb.control('', [Validators.required]);
        this.email = fb.control('', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]);
        this.taskCategory = fb.control('', [Validators.required]);
        this.newCategory = fb.control('', [Validators.required]);
        this.newTask = fb.control('', [Validators.required]);
        this.doi = fb.control('', [Validators.required]);
        this._request = {
            age: '', controlSuggestion: '', doi: '', email: '', fullName: '', generalRequest: '', geneticModification: '', genotype: '', ID: 0,
            method: '', model: '', mouseStrain: '', piEmail: '', piFullName: '', piInstitution: '', scheduleName: '', strainReference: '', subMethod: '',
            subModel: '', taskCategory: '', taskName: '', type: ''
        }
    }

    ngOnInit() {
        this.pubScreenService.getTask().subscribe((data : any) => { this.taskCategoryList = data; });

    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // building request object
        this._request.fullName = this.name.value;
        this._request.email = this.email.value;
        this._request.doi = this.doi.value;
        if (this.taskCategory.value != 'None') {
            this._request.taskCategory = this.taskCategory.value;
        }
        else {
            this._request.taskCategory = "NEW: " + this.newCategory.value;
        }
        this._request.taskName = this.newTask.value;

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
            (this.newCategory.hasError('required') && this.taskCategory.value == 'None')
            
            
        )
        {
            return true;
        }

        return false;
    }

        
}
