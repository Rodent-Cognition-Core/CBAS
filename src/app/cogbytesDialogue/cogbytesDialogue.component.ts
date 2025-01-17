import {
    Component, OnInit, Inject, NgModule,
    ViewChild, ViewContainerRef, ComponentFactoryResolver, ComponentRef
} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PISiteService } from '../services/piSite.service';
import { NgxSpinnerService } from 'ngx-spinner';
//import { UploadService } from '../services/upload.service';
import { SharedModule } from '../shared/shared.module';
import { CogbytesService } from '../services/cogbytes.service';
import { Pubscreen } from '../models/pubscreen';
import { Cogbytes } from '../models/cogbytes'
import { CogbytesUploadComponent } from '../cogbytesUpload/cogbytesUpload.component'
import { IdentityService } from '../services/identity.service';
import { Subject ,  ReplaySubject } from 'rxjs';
import { CogbytesAuthorDialogueComponent } from '../cogbytesAuthorDialogue/cogbytesAuthorDialogue.component';
import { take, takeUntil } from 'rxjs/operators';
import { CogbytesPIDialogeComponent } from '../cogbytesPIDialoge/cogbytesPIDialoge.component'
//import { CogbytesService } from '../services/cogbytes.service';

@Component({

    selector: 'app-cogbytesDialogue',
    templateUrl: './cogbytesDialogue.component.html',
    styleUrls: ['./cogbytesDialogue.component.scss'],
    providers: [TaskAnalysisService,  PISiteService]

})
export class CogbytesDialogueComponent implements OnInit {

    //Models Variables for adding Publication
    keywordsModel: any;
    doiModel: any;
    authorMultiSelect: any;
    isEditMode: boolean;
    descriptionModel: any;
    additionalNotesModel: any;
    linkModel: any;
    piMultiSelect: any;

    // Definiing List Variables 
    authorList: any;
    piList: any;
    searchResultList: any;
    yearList: any;
    paperInfoFromDoiList: any;
    paperInfo: any;

    repID: any;

    private form: FormGroup;

    //Form Validation Variables for adding publications
    author: FormControl;
    pi: FormControl;
    title: FormControl;
    date: FormControl;
    privacyStatus: FormControl;



    //onbj variable from Models
    _cogbytes = new Cogbytes();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public piMultiFilterCtrl: FormControl = new FormControl();
    public filteredPIList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(
        public thisDialogRef: MatDialogRef<CogbytesDialogueComponent>,
        // private pagerService: PagerService,
        private spinnerService: NgxSpinnerService,
        public dialog: MatDialog,
        private cogbytesService: CogbytesService,
        //private cogbytesService: CogbytesService,
        public dialogAuthor: MatDialog,
        public fb: FormBuilder,

        //private resolver: ComponentFactoryResolver,
        
        @Inject(MAT_DIALOG_DATA) public data: any,) {

        this.resetFormVals();
        this.author = fb.control('', [Validators.required])
        this.pi = fb.control('', [Validators.required])
        this.title = fb.control('', [Validators.required])
        this.date = fb.control('', [Validators.required])
        this.privacyStatus = fb.control('', [Validators.required])
    }

    ngOnInit() {

        this.isEditMode = false;

        this.resetFormVals();
        this.GetAuthorList();
        this.GetPIList();

        //console.log(this.data);

        // if it is an Edit model
        if (this.data.repObj != null) {

            this.isEditMode = true;
            this.repID = this.data.repObj.id;
            this.title.setValue(this.data.repObj.title);
            this.date.setValue(new Date(this.data.repObj.date));
            this.keywordsModel = this.data.repObj.keywords;
            this.doiModel = this.data.repObj.doi;
            this.linkModel = this.data.repObj.link;
            this.privacyStatus.setValue(this.data.repObj.privacyStatus ? "true" : "false");
            this.descriptionModel = this.data.repObj.description;
            this.additionalNotesModel = this.data.repObj.additionalNotes;
            this.author.setValue(this.data.repObj.authourID);
            this.pi.setValue(this.data.repObj.piid);
        }



    }

    // Function Definition to open a dialog for adding new Author to the system
    openDialogAuthor(): void {

        let dialogref = this.dialogAuthor.open(CogbytesAuthorDialogueComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe(result => {

            this.GetAuthorList();

        });
    }

    openDialogPI(): void { //PI Dialog Component must be implemented!

        let dialogref = this.dialogAuthor.open(CogbytesPIDialogeComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe(result => {

            this.GetPIList();
        });
    }

    //ngOnDestroy() {
    //    this._onDestroy.next();
    //    this._onDestroy.complete();
    //}

    GetAuthorList() {

        this.cogbytesService.getAuthor().subscribe(data => {
            this.authorList = data;

            // load the initial AuthorList
            this.filteredAutorList.next(this.authorList.slice());

            this.authorMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterAuthor();
                });

        });

        return this.authorList;
    }


    //// handling multi filtered Author list
    private filterAuthor() {
        if (!this.authorList) {
            return;
        }

        // get the search keyword
        let searchAuthor = this.authorMultiFilterCtrl.value;

        if (!searchAuthor) {
            this.filteredAutorList.next(this.authorList.slice());
            return;
        } else {
            searchAuthor = searchAuthor.toLowerCase();
        }

        // filter the Author
        this.filteredAutorList.next(
            this.authorList.filter(x => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
        );
    }


    GetPIList() {

        this.cogbytesService.getPI().subscribe(data => {
            this.piList = data;

            // load the initial expList
            this.filteredPIList.next(this.piList.slice());

            this.piMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterPI();
                });

        });

        return this.piList;
    }

    //// handling multi filtered PI list
    private filterPI() {
        if (!this.piList) {
            return;
        }

        // get the search keyword
        let searchPI = this.piMultiFilterCtrl.value;

        if (!searchPI) {
            this.filteredPIList.next(this.piList.slice());
            return;
        } else {
            searchPI = searchPI.toLowerCase();
        }

        // filter the PI
        this.filteredPIList.next(
            this.piList.filter(x => x.piFullName.toLowerCase().indexOf(searchPI) > -1)
        );
    }

    // Handling Error for the required fields
    getErrorMessageAuthor() {

        return this.author.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessagePi() {
        return this.pi.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageTitle() {

        return this.title.hasError('required') ? 'You must enter a value' : '';
        
    }

    getErrorMessageDate() {
        return this.date.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessagePrivacyStatus() {
        return this.privacyStatus.hasError('required') ? 'You must select a value' : '';
    }

    setDisabledVal() {
       
        if (
            this.title.hasError('required') ||
            this.author.hasError('required') ||
            this.pi.hasError('required') ||
            this.privacyStatus.hasError('required') ||
            this.date.hasError('required') ||
            ((this.title.value == null || this.title.value == "") && this.title.hasError('required'))

        ) {

            return true;
        }

        else {

            return false;
        }

    }


    AddRepository() {

        this.spinnerService.show();

        this._cogbytes.authourID = this.author.value;
        this._cogbytes.title = this.title.value;
        this._cogbytes.keywords = this.keywordsModel;
        this._cogbytes.doi = this.doiModel;
        this._cogbytes.piID = this.pi.value;
        this._cogbytes.link = this.linkModel;
        this._cogbytes.privacyStatus = this.privacyStatus.value == "true" ? true : false; 
        this._cogbytes.description = this.descriptionModel;
        this._cogbytes.additionalNotes = this.additionalNotesModel;
        this._cogbytes.date = this.date.value.toISOString().split('T')[0];

        let today = new Date();
        this._cogbytes.dateRepositoryCreated = today.toISOString().split('T')[0];

        // ADD LINK TO COGBYTES DATABASE HERE

        this.cogbytesService.addRepository(this._cogbytes).subscribe(data => {

            this.thisDialogRef.close();
            setTimeout(() => {
                this.spinnerService.hide();


            }, 500);


        });
    }

    EditRepository() {

        this.spinnerService.show();

        this._cogbytes.authourID = this.author.value;
        this._cogbytes.title = this.title.value;
        this._cogbytes.keywords = this.keywordsModel;
        this._cogbytes.doi = this.doiModel;
        this._cogbytes.piID = this.pi.value;
        this._cogbytes.link = this.linkModel;
        this._cogbytes.privacyStatus = this.privacyStatus.value == "true" ? true : false;
        this._cogbytes.description = this.descriptionModel;
        this._cogbytes.additionalNotes = this.additionalNotesModel;
        this._cogbytes.date = this.date.value.toISOString().split('T')[0];

        let today = new Date();
        //this._cogbytes.dateRepositoryCreated = today.toISOString().split('T')[0];

        // ADD LINK TO COGBYTES DATABASE HERE

        this.cogbytesService.editRepository(this.repID, this._cogbytes).subscribe(data => {

            this.thisDialogRef.close('Cancel');
            setTimeout(() => {
                this.spinnerService.hide();


            }, 500);

        });
    }

    resetFormVals() {

        this.title.setValue('');
        this.keywordsModel = '';
        this.doiModel = '';
        this.date.setValue('');
        this.author.setValue([]);
        this.descriptionModel = '';
        this.additionalNotesModel = '';
        this.linkModel = '';
        this.pi.setValue([]);
    }


    processList(data, item, propertyName) {

        const ret = data.filter(row => (row[propertyName] === item));
        if (ret.length > 0) {
            data.splice(data.findIndex(row => (row[propertyName] === item)), 1);
            data.push(ret[0])
        }
        
        return data
    }
    
}
