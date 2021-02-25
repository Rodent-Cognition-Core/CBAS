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

        //this.pubScreenService.getPaparInfoFromDOI("10.1038/s41398-020-01162-0").subscribe(data => {
        //    console.log(data);
        //});
        //this.pubScreenService.getPaparInfoFromDOI("10.1186/s13041-020-00716-z").subscribe(data => {
        //    console.log(data);
        ////});
        //this.pubScreenService.getPaparInfoFromDOI("10.1038/s41598-021-80960-y").subscribe(data => {
        //    console.log(data);
        //});
        //this.pubScreenService.getPaparInfoFromDOI("10.1097/FBP.0000000000000626").subscribe(data => {
        //    console.log(data);
        //});

        //this.pubScreenService.getPubmedQueue().subscribe(
        //    data => {
        //        this.pubmedQueue = data;
        //        console.log(this.pubmedQueue);
        //        this.pubmedQueue.forEach((pubmedItem) => {
        //            this.pubScreenService.getPaparInfoFromPubmedKey(pubmedItem.pubmedID).subscribe(data => {
        //                pubmedItem.paper = data;
        //            });
        //        })
        //        console.log(this.pubmedQueue);
        //    },
        //);
        this.pubScreenService.getPubmedQueue().subscribe(
            data => {
                this.pubmedQueue = data.result;
                console.log(this.pubmedQueue);
                console.log(this.pubmedQueue.length);
            }
        );
    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

 
    // Deleting publication
    delPub(pubRow) {
        this.openConfirmationDialog(pubRow.id);
    }

    // Deleting Experiment
    openConfirmationDialog(pubID) {
        this.dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Are you sure you want to delete?"


        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                console.log(pubID);

                this.pubScreenService.deletePublicationById(pubID).map(res => {


                    this.spinnerService.hide();


                    location.reload()

                }).subscribe();
            }
            this.dialogRef = null;
        });
    }




}


