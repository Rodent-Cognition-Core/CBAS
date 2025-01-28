import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { SharedModule } from '../shared/shared.module';
import { ReqTaskDialogeComponent } from '../reqTaskDialoge/reqTaskDialoge.component';
import { ReqPIDialogeComponent } from '../reqPIDialoge/reqPIDialoge.component';
import { ReqAgeDialogeComponent } from '../reqAgeDialoge/reqAgeDialoge.component';
import { ReqMouseLineDialogeComponent } from '../reqMouseLineDialoge/reqMouseLineDialoge.component';
import { ReqGeneralDialogeComponent } from '../reqGeneralDialoge/reqGeneralDialoge.component';
import { ReqPubTaskDialogeComponent } from '../reqPubTaskDialoge/reqPubTaskDialoge.component';
import { ReqPubModelDialogeComponent } from '../reqPubModelDialoge/reqPubModelDialoge.component';
import { ReqPubSubMethodDialogeComponent } from '../reqPubSubMethodDialoge/reqPubSubMethodDialoge.component';


@Component({
  selector: 'app-forms',
  templateUrl: './forms.component.html',
  styleUrls: ['./forms.component.scss']
})
export class FormsComponent implements OnInit {

  constructor(public dialogTask: MatDialog, public dialogPI: MatDialog,
    public dialogAge: MatDialog, public dialogML: MatDialog) {


  }

  ngOnInit() {
  }

  // Function Definition to open a dialog for adding new cognitive task to the system
  openDialogTask(): void {

    const dialogref = this.dialogTask.open(ReqTaskDialogeComponent, {
      height: '500px',
      width: '700px',
      data: {}

    });

    dialogref.afterClosed().subscribe();
  }

  // Function Definition to open a dialog for adding new PI to the system
  openDialogPI(): void {

    const dialogref = this.dialogPI.open(ReqPIDialogeComponent, {
      height: '550px',
      width: '700px',
      data: {}

    });

    dialogref.afterClosed().subscribe();
  }

  // Function Definition to open a dialog for adding a new age to the system
  openDialogAge(): void {

    const dialogref = this.dialogAge.open(ReqAgeDialogeComponent, {
      height: '400px',
      width: '700px',
      data: {}

    });

    dialogref.afterClosed().subscribe();
  }


  // Function Definition to open a dialog for adding new mouse line to the system
  openDialogMouseLine(): void {

    const dialogref = this.dialogML.open(ReqMouseLineDialogeComponent, {
      height: '600px',
      width: '700px',
      data: {}

    });

  }

  // Function Definition to open a dialog for adding new Pubscreen task to the system
  openDialogPubTask(): void {

    const dialogref = this.dialogML.open(ReqPubTaskDialogeComponent, {
      height: '600px',
      width: '700px',
      data: {}

    });

  }

  // Function Definition to open a dialog for adding new Pubscreen task to the system
  openDialogPubModel(): void {

    const dialogref = this.dialogML.open(ReqPubModelDialogeComponent, {
      height: '600px',
      width: '700px',
      data: {}

    });

  }

  openDialogPubSubMethod(): void {

    const dialogref = this.dialogML.open(ReqPubSubMethodDialogeComponent, {
      height: '600px',
      width: '700px',
      data: {}

    });

  }

  // Function Definition to open a dialog for adding new mouse line to the system
  openDialogGeneral(): void {

    const dialogref = this.dialogML.open(ReqGeneralDialogeComponent, {
      height: '600px',
      width: '700px',
      data: {}

    });

  }

}
