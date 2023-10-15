import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
//import { Request } from '../models/request';
import { PubScreenService } from '../services/pubScreen.service';
import { SharedModule } from '../shared/shared.module';



@Component({

    selector: 'app-authorDialoge',
    templateUrl: './authorDialoge.component.html',
    styleUrls: ['./authorDialoge.component.scss'],
    providers: [PubScreenService]

})
export class AuthorDialogeComponent implements OnInit {

    // Defining Models Parameters
    authorNameModel: any;
    authorLastNameModel: any;
    authorAffiliationModel: any;

    // FormControl Parameters
    authorName = new FormControl('', [Validators.required]);
    authorLastName = new FormControl('', [Validators.required]);
    
    constructor(public thisDialogRef: MatDialogRef<AuthorDialogeComponent>,
         
        private pubScreenService: PubScreenService, ) { }

    ngOnInit() {
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // Submiting the request to server
        this.pubScreenService.addAuthor(this.authorNameModel, this.authorLastNameModel, this.authorAffiliationModel).subscribe( this.thisDialogRef.close() );
       
    }


    getErrorMessageName()
    {

        return this.authorName.hasError('required') ? 'You must enter a value' :
            '';
    }

    getErrorMessageLastName() {

        return this.authorLastName.hasError('required') ? 'You must enter a value' :
            '';
    }

    setDisabledVal()
    {

        if (this.authorName.hasError('required') ||
            this.authorLastName.hasError('required') 
                     
        )
        {
            return true;
        }

        return false;
    }

        
}
