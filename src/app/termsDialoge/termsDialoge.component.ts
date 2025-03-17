import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';



@Component({

    selector: 'app-termsDialoge',
    templateUrl: './termsDialoge.component.html',
    styleUrls: ['./termsDialoge.component.scss'],
    

})
export class TermsDialogeComponent implements OnInit {

    
    constructor(public thisDialogRef: MatDialogRef<TermsDialogeComponent>,
                @Inject(MAT_DIALOG_DATA) public data: any
         
        ) { }

    ngOnInit() {
      
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

            
}
