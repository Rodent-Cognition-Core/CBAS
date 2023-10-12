import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
//import { Request } from '../models/request';
import { CogbytesService } from '../services/cogbytes.service';
import { SharedModule } from '../shared/shared.module';
import { INVALIDEMAILADDRESS, PIALRADYEXISTS, PISUCCESSFULLYADDED, REQUIREDTOENTERAVALUE } from '../shared/messages';
import { INVALID } from '@angular/forms/src/model';



@Component({

    selector: 'app-cogbytesPIDialoge',
    templateUrl: './cogbytesPIDialoge.component.html',
    styleUrls: ['./cogbytesPIDialoge.component.scss'],
    providers: [CogbytesService]

})
export class CogbytesPIDialogeComponent implements OnInit {

    // Defining Models Parameters

    reqPINameModel: string;
    reqPIEmailModel: string;
    reqInsNameModel: string;

           
    //private _request = new Request();

    // FormControl Parameters

    emailPI = new FormControl('', [Validators.required, Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]);
    piName = new FormControl('', [Validators.required]);
    institution = new FormControl('', [Validators.required]);

    constructor(public thisDialogRef: MatDialogRef<CogbytesPIDialogeComponent>,
         
        private cogbytesService: CogbytesService, ) { }

    ngOnInit() {
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {      

        // Submiting the request to server
        this.cogbytesService.addPI(this.reqPINameModel, this.reqInsNameModel, this.reqPIEmailModel).subscribe(result => {
            if (result == 0) {
                alert(PIALRADYEXISTS);
            }
            else {
                alert(PISUCCESSFULLYADDED);
            }
            this.thisDialogRef.close()
        }); 
       
    }


    getErrorMessagePIName() {
        return this.piName.hasError('required') ? REQUIREDTOENTERAVALUE :
            '';
    }

    getErrorMessageIns() {
        return this.institution.hasError('required') ? REQUIREDTOENTERAVALUE :
            '';
    }

    getErrorMessagePIEmail() {

        return this.emailPI.hasError('required') ? REQUIREDTOENTERAVALUE :
            '';

    }

    getErrorMessagePIEmailValid() {

        return this.emailPI.hasError('pattern') ? INVALIDEMAILADDRESS :
            '';
    }



   
    setDisabledVal()
    {

        if (
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
