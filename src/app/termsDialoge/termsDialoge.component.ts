import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';



@Component({

    selector: 'app-terms-dialoge',
    templateUrl: './termsDialoge.component.html',
    styleUrls: ['./termsDialoge.component.scss'],
    

})
export class TermsDialogeComponent {

    
    constructor(public thisDialogRef: MatDialogRef<TermsDialogeComponent>,
                @Inject(MAT_DIALOG_DATA) public data: any
         
        ) { }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

            
}
