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

const DATASETFILE = 1;

@Component({
    selector: 'app-cogbytes',
    templateUrl: './cogbytes.component.html',
    styleUrls: ['./cogbytes.component.scss']
})


export class CogbytesComponent implements OnInit {

    public uploadKey: number;
    panelOpenState: boolean;

    //public nameModel: any;
    //public dateModel: any;
    public fileTypeModel: any;
    public descriptionModel: any;
    public additionalNotesModel: any;


    // Definiing List Variables
    public fileTypeList: any;


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
        //private cogbytesService: CogbytesService,
        private spinnerService: Ng4LoadingSpinnerService,
    )
    {
        this.resetFormVals();
    }

    ngOnInit() {
        this.panelOpenState = false;


        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        //this.yearList = this.GetYear(1970).sort().reverse();  

    }

    fileType = new FormControl('', [Validators.required]);

    getErrorMessageFileType() {
        return this.fileType.hasError('required') ? 'You must enter a value' : '';
    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    resetFormVals() {

        this.fileTypeModel = '';
        this.descriptionModel = '';
        this.additionalNotesModel = '';
    }

    //// Opening Dialog for adding a new publication.
    openDialogAddRepository(Repository): void {
        let dialogref = this.dialog.open(CogbytesDialogueComponent, {
            height: '850px',
            width: '1200px',
            data: {
                repositoryObj: Repository,
                //sourceOptionModel: this.sourceOptionModel,

            }

        });

        dialogref.afterClosed().subscribe(result => {
            console.log('the dialog was closed');
            //this.DialogResult = result;
            //this.GetAuthorList();
        });
    }

    remove() {

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


