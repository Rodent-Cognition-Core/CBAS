import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Subject } from 'rxjs/Subject';
import { take, takeUntil } from 'rxjs/operators';
import { ManageUserService } from '../services/manageuser.service';
import { PagerService } from '../services/pager.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';


@Component({
    selector: 'app-pubScreenQueue',
    templateUrl: './pubScreenQueue.component.html',
    styleUrls: ['./pubScreenQueue.component.scss']
})
export class PubScreenQueueComponent implements OnInit {

    pubmedQueue: any;

    isAdmin: boolean;
    isUser: boolean;

    dialogRef: MatDialogRef<DeleteConfirmDialogComponent>;
    private _onDestroy = new Subject<void>();

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        private spinnerService: Ng4LoadingSpinnerService,) { }

    ngOnInit() {

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");

        this.pubScreenService.getPubmedQueue().subscribe(
            data => {
                this.pubmedQueue = data;
                console.log(this.pubmedQueue);
            }
        );
    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

 
    acceptPub(row) {
        this.openAcceptDialog(row.pubmedID, row.doi);
    }

    rejectPub(row) {
        this.openRejectDialog(row.pubmedID);
    }

    openAcceptDialog(pubmedID, doi) {
        this.dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Are you sure you wish to accept the paper into Pubscreen?"

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                console.log(pubmedID);

                this.pubScreenService.addQueuePaper(pubmedID, doi).subscribe(data => {
                    alert('Publication successfully added to Pubscreen!')
                    this.pubScreenService.getPubmedQueue().subscribe(
                        data => {
                            this.pubmedQueue = data;
                            console.log(this.pubmedQueue);
                            this.spinnerService.hide();
                        }
                    );
                });
            }
            this.dialogRef = null;
        });
    }

    openRejectDialog(pubmedID) {
        this.dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Are you sure you wish to reject the paper from Pubscreen?"

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                console.log(pubmedID);

                this.pubScreenService.rejectQueuePaper(pubmedID).subscribe(result => {
                    alert('Publication rejected!')
                    this.pubScreenService.getPubmedQueue().subscribe(
                        data => {
                            this.pubmedQueue = data;
                            console.log(this.pubmedQueue);
                            this.spinnerService.hide();
                        }
                    );
                });
            }
            this.dialogRef = null;
        });
    }



}


