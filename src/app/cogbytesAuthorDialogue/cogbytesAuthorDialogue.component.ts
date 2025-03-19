import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, FormBuilder } from '@angular/forms';
import { CogbytesService } from '../services/cogbytes.service';



@Component({

    selector: 'app-cogbytesAuthorDialogue',
    templateUrl: './cogbytesAuthorDialogue.component.html',
    styleUrls: ['./cogbytesAuthorDialogue.component.scss'],
    providers: [CogbytesService]

})
export class CogbytesAuthorDialogueComponent implements OnInit {

    // Defining Models Parameters
    authorAffiliationModel: any;

    // FormControl Parameters
    authorName: FormControl;
    authorLastName: FormControl;
    
    constructor(public thisDialogRef: MatDialogRef<CogbytesAuthorDialogueComponent>,
         
        private cogbytesService: CogbytesService,
        private fb: FormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.authorName = fb.control('', [Validators.required])
        this.authorLastName = fb.control('', [Validators.required])
    }

    ngOnInit() {
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // Submiting the request to server
        this.cogbytesService.addAuthor(this.authorName.value, this.authorLastName.value, this.authorAffiliationModel).subscribe((result: any) => {
            if (result == 0) {
                alert("Author already in database!");
            }
            else {
                alert("Author successfully added!");
            }
            this.thisDialogRef.close()
        });
       
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
