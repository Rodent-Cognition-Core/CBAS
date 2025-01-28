import { Component, OnInit, Inject, NgModule } from '@angular/core';
import {
    MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef,
    MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA
} from '@angular/material/legacy-dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
// import { Request } from '../models/request';
import { CogbytesService } from '../services/cogbytes.service';
// import { SharedModule } from '../shared/shared.module';
import { INVALIDEMAILADDRESS, PIALRADYEXISTS, PISUCCESSFULLYADDED, FIELDISREQUIRED } from '../shared/messages';



@Component({

  selector: 'app-cogbytespidialoge',
  templateUrl: './cogbytesPIDialoge.component.html',
  styleUrls: ['./cogbytesPIDialoge.component.scss'],
  providers: [CogbytesService]

  })
export class CogbytesPIDialogeComponent implements OnInit {


  // private _request = new Request();

  // FormControl Parameters

  emailPI: UntypedFormControl;
  piName: UntypedFormControl;
  institution: UntypedFormControl;

  constructor(public thisDialogRef: MatDialogRef<CogbytesPIDialogeComponent>,

    private cogbytesService: CogbytesService,
    private fb: UntypedFormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.emailPI = fb.control('', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]);
    this.piName = fb.control('', [Validators.required]);
    this.institution = fb.control('', [Validators.required]);
  }

  ngOnInit() {

  }

  onCloseCancel(): void {


    this.thisDialogRef.close('Cancel');

  }

  onCloseSubmit(): void {

    // Submiting the request to server
    this.cogbytesService.addPI(this.piName.value, this.institution.value, this.emailPI.value).subscribe((result: any) => {
      if (result === 0) {
        alert(PIALRADYEXISTS);
      } else {
        alert(PISUCCESSFULLYADDED);
      }
      this.thisDialogRef.close();
    });

  }


  getErrorMessagePIName() {
    return this.piName.hasError('required') ? FIELDISREQUIRED :
      '';
  }

  getErrorMessageIns() {
    return this.institution.hasError('required') ? FIELDISREQUIRED :
      '';
  }

  getErrorMessagePIEmail() {

    return this.emailPI.hasError('required') ? FIELDISREQUIRED :
      '';

  }

  getErrorMessagePIEmailValid() {

    return this.emailPI.hasError('pattern') ? INVALIDEMAILADDRESS :
      '';
  }




  setDisabledVal() {

    if (
      this.emailPI.hasError('required') ||
            this.emailPI.hasError('pattern') ||
            this.piName.hasError('required') ||
            this.institution.hasError('required')

    ) {
      return true;
    }

    return false;
  }


}
