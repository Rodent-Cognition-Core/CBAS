import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ReqTaskDialogeComponent } from '../reqTaskDialoge/reqTaskDialoge.component';
import { ReqPIDialogeComponent } from '../reqPIDialoge/reqPIDialoge.component';
import { ReqAgeDialogeComponent } from '../reqAgeDialoge/reqAgeDialoge.component';
import { ReqMouseLineDialogeComponent } from '../reqMouseLineDialoge/reqMouseLineDialoge.component';
import { ReqGeneralDialogeComponent } from '../reqGeneralDialoge/reqGeneralDialoge.component';

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

        let dialogref = this.dialogTask.open(ReqTaskDialogeComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe();
    }

    // Function Definition to open a dialog for adding new PI to the system
    openDialogPI(): void {

        let dialogref = this.dialogPI.open(ReqPIDialogeComponent, {
            height: '550px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe();
    }

    // Function Definition to open a dialog for adding a new age to the system 
    openDialogAge(): void {

        let dialogref = this.dialogAge.open(ReqAgeDialogeComponent, {
            height: '400px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe();
    }


    //Function Definition to open a dialog for adding new mouse line to the system
    openDialogMouseLine(): void {

        let dialogref = this.dialogML.open(ReqMouseLineDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

    //Function Definition to open a dialog for adding new mouse line to the system
    openDialogGeneral(): void {

        let dialogref = this.dialogML.open(ReqGeneralDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

}
