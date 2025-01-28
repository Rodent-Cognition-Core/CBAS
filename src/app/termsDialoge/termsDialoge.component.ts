import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
// import { Request } from '../models/request';
// import { RequestService } from '../services/request.service';
// import { SharedModule } from '../shared/shared.module';



@Component({

  selector: 'app-termsdialoge',
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
