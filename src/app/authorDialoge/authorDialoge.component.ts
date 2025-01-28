import { Component, OnInit, Inject, NgModule } from '@angular/core';
import {
    MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef,
    MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA
} from '@angular/material/legacy-dialog';
// import { MatFormFieldModule } from '@angular/material/form-field';
import { UntypedFormControl, Validators, /* ReactiveFormsModule, FormGroup,*/ UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
// import { Request } from '../models/request';
import { PubScreenService } from '../services/pubScreen.service';
import { FIELDISREQUIRED } from '../shared/messages';



@Component({

  selector: 'app-authordialoge',
  templateUrl: './authorDialoge.component.html',
  styleUrls: ['./authorDialoge.component.scss'],
  providers: [PubScreenService]

  })
export class AuthorDialogeComponent implements OnInit {

  // Defining Models Parameters
  authorAffiliationModel: any;

  // FormControl Parameters
  authorName: UntypedFormControl;
  authorLastName: UntypedFormControl;

  constructor(public thisDialogRef: MatDialogRef<AuthorDialogeComponent>,

    private pubScreenService: PubScreenService,
    private fb: UntypedFormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.authorName = fb.control('', [Validators.required]);
    this.authorLastName = fb.control('', [Validators.required]);
  }

  ngOnInit() {

  }

  onCloseCancel(): void {


    this.thisDialogRef.close('Cancel');

  }

  onCloseSubmit(): void {

    // Submiting the request to server
    this.pubScreenService.addAuthor(this.authorName.value, this.authorLastName.value, this.authorAffiliationModel)
      .subscribe(this.thisDialogRef.close());

  }


  getErrorMessageName() {

    return this.authorName.hasError('required') ? FIELDISREQUIRED :
      '';
  }

  getErrorMessageLastName() {

    return this.authorLastName.hasError('required') ? FIELDISREQUIRED :
      '';
  }

  setDisabledVal() {

    if (this.authorName.hasError('required') ||
            this.authorLastName.hasError('required')

    ) {
      return true;
    }

    return false;
  }


}
