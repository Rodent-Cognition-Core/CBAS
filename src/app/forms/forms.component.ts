import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
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
export class FormsComponent {

    constructor(public dialogTask: MatDialog, public dialogPI: MatDialog,
        public dialogAge: MatDialog, public dialogML: MatDialog) {


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

        let _dialogref = this.dialogML.open(ReqMouseLineDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

    //Function Definition to open a dialog for adding new Pubscreen task to the system
    openDialogPubTask(): void {

        let _dialogref = this.dialogML.open(ReqPubTaskDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

    //Function Definition to open a dialog for adding new Pubscreen task to the system
    openDialogPubModel(): void {

        let _dialogref = this.dialogML.open(ReqPubModelDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

    openDialogPubSubMethod(): void {

        let _dialogref = this.dialogML.open(ReqPubSubMethodDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

    //Function Definition to open a dialog for adding new mouse line to the system
    openDialogGeneral(): void {

        let _dialogref = this.dialogML.open(ReqGeneralDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

}
