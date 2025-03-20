import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, UntypedFormBuilder } from '@angular/forms';
import { PubScreenService } from '../services/pubScreen.service';
import { FIELDISREQUIRED } from '../shared/messages';



@Component({

    selector: 'app-author-dialoge',
    templateUrl: './authorDialoge.component.html',
    styleUrls: ['./authorDialoge.component.scss'],
    providers: [PubScreenService]

})
export class AuthorDialogeComponent {

    // Defining Models Parameters
    authorAffiliationModel: any;

    // UntypedFormControl Parameters
    authorName: UntypedFormControl
    authorLastName: UntypedFormControl
    
    constructor(public thisDialogRef: MatDialogRef<AuthorDialogeComponent>,
         
        private pubScreenService: PubScreenService,
        private fb: UntypedFormBuilder,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {
        this.authorName = fb.control('', [Validators.required])
        this.authorLastName = fb.control('', [Validators.required])
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {

        // Submiting the request to server
        this.pubScreenService.addAuthor(this.authorName.value, this.authorLastName.value, this.authorAffiliationModel).subscribe( this.thisDialogRef.close() );
       
    }


    getErrorMessageName()
    {

        return this.authorName.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageLastName() {

        return this.authorLastName.hasError('required') ? FIELDISREQUIRED :
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
