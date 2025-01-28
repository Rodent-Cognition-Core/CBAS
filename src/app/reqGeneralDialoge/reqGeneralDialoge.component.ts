import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
// import { MatFormFieldModule } from '@angular/material/form-field';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
// import { SharedModule } from '../shared/shared.module';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

  selector: 'app-reqgeneraldialoge',
  templateUrl: './reqGeneralDialoge.component.html',
  styleUrls: ['./reqGeneralDialoge.component.scss'],
  providers: [RequestService]

})
export class ReqGeneralDialogeComponent implements OnInit {

  // FormControl Parameters

  name: UntypedFormControl;
  email: UntypedFormControl;
  request: UntypedFormControl;

  private _request: Request;


  constructor(public thisDialogRef: MatDialogRef<ReqGeneralDialogeComponent>,

    private requestService: RequestService,
    private fb: UntypedFormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.name = fb.control('', [Validators.required]);
    this.email = fb.control('', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]);
    this.request = fb.control('', [Validators.required]);
    this._request = {
      age: '', controlSuggestion: '', doi: '', email: '', fullName: '', generalRequest: '', geneticModification: '', genotype: '', id: 0,
      method: '', model: '', mouseStrain: '', piEmail: '', piFullName: '', piInstitution: '', scheduleName: '', strainReference: '',
      subMethod: '', subModel: '', taskCategory: '', taskName: '', type: ''
    };
  }

  ngOnInit() {

  }

  onCloseCancel(): void {


    this.thisDialogRef.close('Cancel');

  }

  onCloseSubmit(): void {

    // building request object
    this._request.fullName = this.name.value;
    this._request.email = this.email.value;
    this._request.generalRequest = this.request.value;



    // Submiting the request to server
    this.requestService.addGeneral(this._request).subscribe( this.thisDialogRef.close());

  }


  getErrorMessage() {

    return this.name.hasError('required') ? FIELDISREQUIRED :
      '';
  }

  getErrorMessageEmail() {

    return this.email.hasError('required') ? FIELDISREQUIRED :
      '';

  }

  getErrorMessageEmailValid() {

    return this.email.hasError('pattern') ? INVALIDEMAILADDRESS :
      '';
  }

  getErrorMessageRequest() {
    return this.request.hasError('required') ? FIELDISREQUIRED :
      '';
  }




  setDisabledVal() {

    if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.request.hasError('required')


    ) {
      return true;
    }

    return false;
  }


}
