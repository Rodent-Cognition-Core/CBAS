import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Subject } from 'rxjs/Subject';
import { take, takeUntil } from 'rxjs/operators';
import { ManageUserService } from '../services/manageuser.service';
//import { PagerService } from '../services/pager.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';


@Component({
    selector: 'app-pubScreen',
    templateUrl: './pubScreen.component.html',
    styleUrls: ['./pubScreen.component.scss']
})
export class PubScreenComponent implements OnInit {

   

    constructor(
        // private pagerService: PagerService,
        public dialog: MatDialog,
        private pubScreenService: PubScreenService, public dialogAuthor: MatDialog) {

       
    }

    ngOnInit() {

        


    }

    

    

}
