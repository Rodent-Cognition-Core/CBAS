import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject ,  Subject } from 'rxjs';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
    selector: 'app-pubScreenEdit',
    templateUrl: './pubScreenEdit.component.html',
    styleUrls: ['./pubScreenEdit.component.scss']
})
export class PubScreenEditComponent implements OnInit {

    
    paperInfo: any;
   
    paperLinkGuid: string;
    isAdmin: boolean;
    isFullDataAccess: boolean;
    isLoaded: boolean;


    _pubSCreenSearch = new Pubscreen();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService,
        private route: ActivatedRoute,
        private spinnerService: Ng4LoadingSpinnerService,
        public dialogAuthor: MatDialog) {

        this.isLoaded = false;

        this.route.queryParams.subscribe(params => {
            this.paperLinkGuid = params['paperlinkguid'].split(" ")[0];

            this.GetDataByLinkGuid(this.paperLinkGuid);
        });

    }

    ngOnInit() {

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
    }

    GetDataByLinkGuid(paperLinkGuid) {
        this.spinnerService.show();

        this.pubScreenService.getDataByLinkGuid(paperLinkGuid).subscribe(data => {

            this.paperInfo = data;
            console.log(this.paperInfo)

            this.isLoaded = true;



        });

        this.spinnerService.hide();

    }

    // Edit publication
    openDialogEditPublication(Publication): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: { publicationObj: Publication, isPublic: Boolean }

        });

        dialogref.afterClosed().subscribe(result => {
            console.log('the dialog was closed');
            //this.search();
        });
    }

    

}
