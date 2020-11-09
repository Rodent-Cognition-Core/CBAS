import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
//import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { CogbytesDialogueComponent } from '../cogbytesDialogue/cogbytesDialogue.component';
import { CogbytesUpload } from '../models/cogbytesUpload'
import { CogbytesService } from '../services/cogbytes.service'
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';


@Component({
    selector: 'app-cogbytes',
    templateUrl: './cogbytes.component.html',
    styleUrls: ['./cogbytes.component.scss']
})


export class CogbytesComponent implements OnInit {

    public uploadKey: number;
    panelOpenState: boolean;

    public repModel: any;


    // Definiing List Variables
    repList: any;

    _cogbytesUpload = new CogbytesUpload();


    isAdmin: boolean;
    isUser: boolean;

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    dialogRef: MatDialogRef<DeleteConfirmDialogComponent>;

    constructor(
        public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        //public dialogAuthor: MatDialog,
        private cogbytesService: CogbytesService,
        private spinnerService: Ng4LoadingSpinnerService,
    )
    {
        this.resetFormVals();
    }

    ngOnInit() {
        this.panelOpenState = false;


        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");

        if (this.isAdmin || this.isUser) {

            this.cogbytesService.getRepositories().subscribe(data => { this.repList = data; });
        }
    }


    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    resetFormVals() {

    }

    GetRepositories() {
        this.cogbytesService.getRepositories().subscribe(data => { this.repList = data; });
        return this.repList;
    }

    //// Opening Dialog for adding a new repository.
    openDialogAddRepository(): void {
        let dialogref = this.dialog.open(CogbytesDialogueComponent, {
            height: '850px',
            width: '1200px',
            data: {
                repObj: null,
            }

        });

        dialogref.afterClosed().subscribe(result => {
            console.log('the dialog was closed');
            //this.DialogResult = result;
            //this.GetAuthorList();
        });
    }

    //// Opening Dialog for editing an existing repository.
    openDialogEditRepository(): void {
        let dialogref = this.dialog.open(CogbytesDialogueComponent, {
            height: '850px',
            width: '1200px',
            data: {
                repObj: this.repList[this.repList.map(function (x) { return x.title }).indexOf(this.repModel)],
            }

        });

        dialogref.afterClosed().subscribe(result => {
            console.log('the dialog was closed');
            this.repModel = null;
            //this.DialogResult = result;
            //this.GetAuthorList();
        });
    }

    remove() {

    }

    getRepID() : number {
        return this.repList[this.repList.map(function (x) { return x.title }).indexOf(this.repModel)].id;
    }

    //// Edit publication
    //openDialogEditPublication(Publication): void {
    //    let dialogref = this.dialog.open(CogbytesDialogueComponent, {
    //        height: '850px',
    //        width: '1200px',
    //        data: { publicationObj: Publication }

    //    });

    //    dialogref.afterClosed().subscribe(result => {
    //        console.log('the dialog was closed');
    //        this.search();
    //    });
    //}
  

}


