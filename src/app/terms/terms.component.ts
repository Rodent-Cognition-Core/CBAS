import { Component, Optional } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog'

@Component({
  selector: 'app-terms',
  templateUrl: './terms.component.html',
  styleUrls: ['./terms.component.scss']
})
export class TermsComponent {

    constructor(@Optional() public dialogRef: MatDialogRef<TermsComponent>) { }

}
