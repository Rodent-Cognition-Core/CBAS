import {
    Component, OnInit, Inject, NgModule,
    ViewChild, ViewContainerRef, ComponentFactoryResolver, ComponentRef
} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PISiteService } from '../services/piSite.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
//import { UploadService } from '../services/upload.service';
import { SharedModule } from '../shared/shared.module';
import { CogbytesService } from '../services/cogbytes.service';
import { Pubscreen } from '../models/pubscreen';
import { Cogbytes } from '../models/cogbytes'
import { CogbytesUploadComponent } from '../cogbytesUpload/cogbytesUpload.component'
import { IdentityService } from '../services/identity.service';
import { Subject } from 'rxjs/Subject';
import { ReplaySubject } from 'rxjs/ReplaySubject';
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
    authorModel: any;
    titleModel: any;
    dateModel: any;
    keywordsModel: any;
    doiModel: any;
    authorMultiSelect: any;
    isEditMode: boolean;
    privacyStatusModel: any;
    descriptionModel: any;
    additionalNotesModel: any;
    linkModel: any;
    piModel: any;
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



    //onbj variable from Models
    _cogbytes = new Cogbytes();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public piMultiFilterCtrl: FormControl = new FormControl();
    public filteredPIList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    // container for child components (cogbytesUpload)
    //@ViewChild('uploadContainer', { read: ViewContainerRef }) entry: ViewContainerRef;
    //child_unique_key: number = 0;
    //componentReferences = Array<ComponentRef<CogbytesUploadComponent>>();

    constructor(
        public thisDialogRef: MatDialogRef<CogbytesDialogueComponent>,
        // private pagerService: PagerService,
        private spinnerService: Ng4LoadingSpinnerService,
        public dialog: MatDialog,
        private cogbytesService: CogbytesService,
        //private cogbytesService: CogbytesService,
        public dialogAuthor: MatDialog,

        //private resolver: ComponentFactoryResolver,
        
        @Inject(MAT_DIALOG_DATA) public data: any,) {

        this.resetFormVals();
    }

    ngOnInit() {

        this.isEditMode = false;

        this.resetFormVals();
        this.GetAuthorList();
        this.GetPIList();
        //this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; console.log(this.yearList); });
        //this.getAllYears();

        console.log(this.data);
        // if it is an Edit model

        if (this.data.repObj != null) {

            this.isEditMode = true;
            this.repID = this.data.repObj.id;
            this.titleModel = this.data.repObj.title;
            this.dateModel = new Date(this.data.repObj.date);
            this.keywordsModel = this.data.repObj.keywords;
            this.doiModel = this.data.repObj.doi;
            this.linkModel = this.data.repObj.link;
            this.privacyStatusModel = this.data.repObj.privacyStatus;
            this.descriptionModel = this.data.repObj.description;
            this.additionalNotesModel = this.data.repObj.additionalNotes;
            this.authorModel = this.data.repObj.authourID;
            this.piModel = this.data.repObj.piid;
        }



    }

    //ngAfterViewInit() {
    //    console.log("ngAfterViewInit");
    //}

    //createUploadComponent() {
    //    const factory = this.resolver.resolveComponentFactory(CogbytesUploadComponent);
    //    const componentRef = this.entry.createComponent(factory);
    //    let childComponent = componentRef.instance;
    //    childComponent.uploadKey = ++this.child_unique_key;
    //    childComponent.parentRef = this;

    //    this.componentReferences.push(componentRef);
    //}

    //removeUploadComponent(key: number) {

    //    if (this.entry.length < 1) return;

    //    let componentRef = this.componentReferences.filter(
    //        x => x.instance.uploadKey == key
    //    )[0];

    //    let uploadIndex: number = this.entry.indexOf(componentRef as any);

    //    this.entry.remove(uploadIndex);

    //    this.componentReferences = this.componentReferences.filter(
    //        x => x.instance.uploadKey !== key
    //    );

    //}


    // Function Definition to open a dialog for adding new cognitive task to the system
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

        //let dialogref = this.dialogAuthor.open(AuthorDialogeComponent, {
        let dialogref = this.dialogAuthor.open(CogbytesPIDialogeComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe(result => {

            //this.GetAuthorList();
        });
    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    GetAuthorList() {

        this.cogbytesService.getAuthor().subscribe(data => {
            this.authorList = data;

            // load the initial expList
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
            //this.filteredPIList.next(this.piList.slice());

            //this.piMultiFilterCtrl.valueChanges
            //    .pipe(takeUntil(this._onDestroy))
            //    .subscribe(() => {
            //        this.filterPI();
            //    });

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
            this.piList.filter(x => x.lastName.toLowerCase().indexOf(searchPI) > -1)
        );
    }

    //Form Validation Variables for adding publications
    author = new FormControl('', [Validators.required]);
    title = new FormControl('', [Validators.required]);
    date = new FormControl('', [Validators.required]);
    privacyStatus = new FormControl('', [Validators.required]);

    // Handling Error for the required fields
    getErrorMessageAuthor() {

        return this.author.hasError('required') ? 'You must enter a value' : '';
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

        //if (this.authorModel === null && this.author.hasError('required')) {

        //    return true;
        //}


        //else if (
        if (
            this.title.hasError('required') ||
            this.author.hasError('required') ||
            this.privacyStatus.hasError('required') ||
            this.date.hasError('required') ||
            ((this.titleModel == null || this.titleModel == "") && this.title.hasError('required'))

        ) {

            return true;
        }

        else {

            return false;
        }

    }


    AddRepository() {

        this._cogbytes.authourID = this.authorModel;
        this._cogbytes.title = this.titleModel;
        this._cogbytes.keywords = this.keywordsModel;
        this._cogbytes.doi = this.doiModel;
        this._cogbytes.piID = this.piModel;
        this._cogbytes.link = this.linkModel;
        this._cogbytes.privacyStatus = this.privacyStatusModel == "true" ? true : false; 
        this._cogbytes.description = this.descriptionModel;
        this._cogbytes.additionalNotes = this.additionalNotesModel;
        this._cogbytes.date = this.dateModel.toISOString().split('T')[0];

        let today = new Date();
        this._cogbytes.dateRepositoryCreated = today.toISOString().split('T')[0];

        // ADD LINK TO COGBYTES DATABASE HERE

        this.cogbytesService.addRepository(this._cogbytes).subscribe(data => {

            if (data === null) {
                alert("Failed to add repository to Cogbytes");
            }
            else {
                this.thisDialogRef.close();
                alert("Repository was successfully added to the system!");
            }
        });
    }

    EditRepository() {

        this._cogbytes.authourID = this.authorModel;
        this._cogbytes.title = this.titleModel;
        this._cogbytes.keywords = this.keywordsModel;
        this._cogbytes.doi = this.doiModel;
        this._cogbytes.piID = this.piModel;
        this._cogbytes.link = this.linkModel;
        this._cogbytes.privacyStatus = this.privacyStatusModel == "true" ? true : false;
        this._cogbytes.description = this.descriptionModel;
        this._cogbytes.additionalNotes = this.additionalNotesModel;
        this._cogbytes.date = this.dateModel.toISOString().split('T')[0];

        let today = new Date();
        //this._cogbytes.dateRepositoryCreated = today.toISOString().split('T')[0];

        // ADD LINK TO COGBYTES DATABASE HERE

        this.cogbytesService.editRepository(this.repID, this._cogbytes).subscribe(data => {

            if (data === null) {
                alert("Failed to edit repository in Cogbytes");
            }
            else {
                this.thisDialogRef.close();
                alert("Repository was successfully edited to the system!");
            }
        });
    }

/* **********************************************************************
    // Adding a new publication to DB by cliking on Submit button
   AddEditPublication() {

        if (!this.yearModel.match(/^(19|20)\d{2}$/)) {
            alert('year is not valid!');
            return;
        }

        if (this.authorModel !== null && this.authorModel.length !== 0) {
            this._pubscreen.authourID = this.authorModel;
            console.log(this.authorModel)
        }
        else {

            this._pubscreen.author = this.authorList2;
            this._pubscreen.authorString = this.authorModel2;
            console.log(this._pubscreen.author)
            console.log(this.authorList2);

        }

        if (this.paperTypeModel !== null) {
            this._pubscreen.paperType = this.paperTypeModel;
        }
        else {
            this._pubscreen.paperType = this.paperTypeModel2;
        }
        if (this.referenceModel !== null) {
            this._pubscreen.reference = this.referenceModel;
        }

        this._pubscreen.title = this.titleModel;
        this._pubscreen.abstract = this.abstractModel;
        this._pubscreen.keywords = this.keywordsModel;
        this._pubscreen.doi = this.doiModel;
        this._pubscreen.year = this.yearModel;
        this._pubscreen.taskID = this.cognitiveTaskModel;
        this._pubscreen.specieID = this.specieModel;
        this._pubscreen.sexID = this.sexModel;
        this._pubscreen.strainID = this.strainModel;
        this._pubscreen.diseaseID = this.diseaseModel;
        this._pubscreen.regionID = this.regionModel;
        this._pubscreen.subRegionID = this.subRegionModel;
        this._pubscreen.cellTypeID = this.cellTypeModel;
        this._pubscreen.methodID = this.methodModel;
        this._pubscreen.transmitterID = this.neurotransmitterModel;
        this._pubscreen.taskOther = this.taskOtherModel;
        this._pubscreen.specieOther = this.specieOtherModel;
        this._pubscreen.strainOther = this.strainOtherModel;
        this._pubscreen.diseaseOther = this.diseaseOtherModel;
        this._pubscreen.celltypeOther = this.cellOtherModel;
        this._pubscreen.methodOther = this.methodOtherModel;
        this._pubscreen.neurotransOther = this.neurotransmitterOtherModel;

                    

        switch (this.sourceOptionModel) {

            case 1: {
                this._pubscreen.source = 'pubMed';
                break;
            }
            case 2: {
                this._pubscreen.source = 'BioRxiv';
                break;
            }
            case 3: {
                this._pubscreen.source = 'None';
                break;
            }

        }

        if (this.isEditMode) {
            this.pubScreenService.EditPublication(this.publicationId, this._pubscreen).subscribe(data => {

                if (data === true) {
                    alert("Publication was successfully edited!");
                    this.thisDialogRef.close();
                } else {
                    alert("Error in editing publication! Please try again, if this happens again contact admin.")
                }
 
            });

        } else {
            this.pubScreenService.addPublication(this._pubscreen).subscribe(data => {

                if (data === null) {
                    alert("Publication with the same DOI exists in the database!");
                } else {
                    this.thisDialogRef.close();
                    alert("Publication was successfully added to the system!");
                }
                this.resetFormVals();

            });
        }

    }

*****************************/

    resetFormVals() {

        this.titleModel = '';
        this.keywordsModel = '';
        this.doiModel = '';
        this.dateModel = '';
        this.authorModel = [];
        this.descriptionModel = '';
        this.additionalNotesModel = '';
        this.linkModel = '';
        this.piModel = [];
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
