import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IdentityService } from '../services/identity.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PASSWORDSUCCESSFULLYCHANGED } from '../shared/messages';

@Component({
    selector: 'app-password-dialog',
    templateUrl: './password-dialog.component.html',
    styleUrls: ['./password-dialog.component.scss'],
    providers: [IdentityService]
})
export class PasswordDialogComponent implements OnInit {

    // Parameters Initialization
    model: any = {};
    showSuccess = false;
    errorMessages: any[] = [];


    constructor(public thisDialogRef: MatDialogRef<PasswordDialogComponent>,

        private identityService: IdentityService,
        private snackBar: MatSnackBar,
        @Inject(MAT_DIALOG_DATA) public data: any) { }

    ngOnInit() {




    }


    onCloseCancel(): void {

        this.thisDialogRef.close('Cancel');
    }


    onCloseSubmit(): void {

        this.identityService.changePassword(this.model.currentPassword, this.model.newPassword).subscribe(
            (res: any) => {
                // IdentityResult.
                if (res == 'Invalid Email!') {
                    this.errorMessages.push({ description: "Invalid Email!" });
                } else {
                    if (res.succeeded) {
                        //this.showSuccess = true;
                        this.snackBar.open(PASSWORDSUCCESSFULLYCHANGED, "", {
                            duration: 2000,
                            horizontalPosition: 'right',
                            verticalPosition: 'top',

                        });
                        this.thisDialogRef.close(true);

                    } else {
                        this.errorMessages = res.errors;
                    }
                }

            },
            (error: any) => {
                const errMsg = (error.message) ? error.message :
                    error.status ? `${error.status} - ${error.statusText}` : "Server error";
                //console.log(errMsg);
                this.errorMessages.push({ description: "Server error. Try later." });
            });

    }

    clearMessages(): void {
        this.errorMessages = [];
    }



}




