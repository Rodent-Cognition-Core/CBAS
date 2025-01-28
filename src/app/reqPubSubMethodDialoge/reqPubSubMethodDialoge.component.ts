import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
import { Request } from '../models/request';
import { RequestService } from '../services/request.service';
import { PubScreenService } from '../services/pubScreen.service';
// import { SharedModule } from '../shared/shared.module';
import { FIELDISREQUIRED, INVALIDEMAILADDRESS } from '../shared/messages';



@Component({

  selector: 'app-reqpubsubmethoddialoge',
  templateUrl: './reqPubSubMethodDialoge.component.html',
  styleUrls: ['./reqPubSubMethodDialoge.component.scss'],
  providers: [RequestService, PubScreenService]

})
export class ReqPubSubMethodDialogeComponent implements OnInit {

  methodList: any;

  // FormControl Parameters

  name: UntypedFormControl;
  email: UntypedFormControl;
  method: UntypedFormControl;
  newSubMethod: UntypedFormControl;
  doi: UntypedFormControl;

  private _request: Request;

  constructor(public thisDialogRef: MatDialogRef<ReqPubSubMethodDialogeComponent>,

    private requestService: RequestService, private pubScreenService: PubScreenService,
    private fb: UntypedFormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.name = fb.control('', [Validators.required]);
    this.email = fb.control('', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]);
    this.method = fb.control('', [Validators.required]);
    this.newSubMethod = fb.control('', [Validators.required]);
    this.doi = fb.control('', [Validators.required]);
    this._request = {
      age: '', controlSuggestion: '', doi: '', email: '', fullName: '', generalRequest: '', geneticModification: '', genotype: '', id: 0,
      method: '', model: '', mouseStrain: '', piEmail: '', piFullName: '', piInstitution: '', scheduleName: '', strainReference: '',
      subMethod: '', subModel: '', taskCategory: '', taskName: '', type: ''
    };
  }

  ngOnInit() {
    this.pubScreenService.getMethod().subscribe((data: any) => {
      this.methodList = data;
    });

  }

  onCloseCancel(): void {


    this.thisDialogRef.close('Cancel');

  }

  onCloseSubmit(): void {

    // building request object
    this._request.fullName = this.name.value;
    this._request.email = this.email.value;
    this._request.doi = this.doi.value;
    this._request.method = this.method.value;
    this._request.subMethod = this.newSubMethod.value;

    // Submiting the request to server
    this.requestService.addPubSubMethod(this._request).subscribe( this.thisDialogRef.close());

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

  getErrorMessageMethod() {

    return this.method.hasError('required') ? FIELDISREQUIRED :
      '';

  }

  getErrorMessageNewSubMethod() {
    return this.newSubMethod.hasError('required') ? FIELDISREQUIRED :
      '';
  }

  getErrorMessageDOI() {
    return this.doi.hasError('required') ? FIELDISREQUIRED :
      '';
  }




  setDisabledVal() {

    if (this.name.hasError('required') ||
            this.email.hasError('required') ||
            this.email.hasError('pattern') ||
            this.newSubMethod.hasError('required') ||
            this.doi.hasError('required') ||
            this.method.hasError('required')
    ) {
      return true;
    }

    return false;
  }


}
