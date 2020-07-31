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
    selector: 'app-shared-pubscreen',
    templateUrl: './shared-pubscreen.component.html',
    styleUrls: ['./shared-pubscreen.component.scss']
})
export class SharedPubscreenComponent implements OnInit {

    //Models Variables for adding Publication
    authorModel: any;
    titleModel: any;
    abstractModel: any;
    yearModel: any;
    keywordsModel: any;
    doiModel: any;
    paperTypeModel: any;
    cognitiveTaskModel: any;
    specieModel: any;
    sexModel: any;
    strainModel: any;
    diseaseModel: any;
    regionModel: any;
    subRegionModel: any;
    cellTypeModel: any;
    addingOptionModel: any;
    methodModel: any;
    neurotransmitterModel: any;
    authorMultiSelect: any;
    doiKeyModel: any;
    PubMedKeyModel: any;
    authorModel2: any;
    paperTypeModel2: any;
    referenceModel: any;
    sourceOptionModel: any;
    bioAddingOptionModel: any;
    bioDoiKeyModel: any;


    //Models Variables for searching publication
    yearSearchModel: any

    // Definiing List Variables 
    paperTypeList: any;
    taskList: any;
    specieList: any;
    sexList: any;
    strainList: any;
    diseaseList: any;
    regionSubregionList: any
    regionList: any;
    subRegionList: any;
    cellTypeList: any;
    methodList: any;
    neurotransmitterList: any;
    authorList: any;
    authorList2: any;
    searchResultList: any;
    yearList: any;
    paperInfoFromDoiList: any;

    //Form Validation Variables for adding publications
    author = new FormControl('', [Validators.required]);
    title = new FormControl('', [Validators.required]);
    doi = new FormControl('', [Validators.required]);
    doiKey = new FormControl('', [Validators.required]);
    paperType = new FormControl('', [Validators.required]);
    cognitiveTask = new FormControl('', [Validators.required]);
    //specie = new FormControl('', [Validators.required]);
    //sex = new FormControl('', [Validators.required]);
    addingOption = new FormControl('', [Validators.required]);
    year = new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    pubMedKey = new FormControl('', [Validators.required]);
    sourceOption = new FormControl('', [Validators.required]);
    bioAddingOption = new FormControl('', [Validators.required]);
    doiKeyBio = new FormControl('', [Validators.required]);


    //onbj variable from Models
    _pubscreen = new Pubscreen();
    _pubSCreenSearch = new Pubscreen();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(
        // private pagerService: PagerService,
        public dialog: MatDialog,
        private pubScreenService: PubScreenService, public dialogAuthor: MatDialog) {

        this.resetFormVals();
    }

    ngOnInit() {

        //this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe(data => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe(data => { this.taskList = data; });
        this.pubScreenService.getSpecie().subscribe(data => { this.specieList = data; });
        this.pubScreenService.getSex().subscribe(data => { this.sexList = data; });
        this.pubScreenService.getStrain().subscribe(data => { this.strainList = data; });
        this.pubScreenService.getDisease().subscribe(data => { this.diseaseList = data; });
        this.pubScreenService.getRegion().subscribe(data => { this.regionList = data; });
        this.pubScreenService.getCellType().subscribe(data => { this.cellTypeList = data; });
        this.pubScreenService.getMethod().subscribe(data => { this.methodList = data; });
        this.pubScreenService.getNeurotransmitter().subscribe(data => { this.neurotransmitterList = data; });

        //this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; console.log(this.yearList); });
        this.getAllYears();


    }

    // Function Definition to open a dialog for adding new cognitive task to the system
    openDialogAuthor(): void {

        let dialogref = this.dialogAuthor.open(AuthorDialogeComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe(result => {

            this.GetAuthorList();
        });
    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    GetAuthorList() {

        this.resetFormVals();

        this.pubScreenService.getAuthor().subscribe(data => {
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

    // Getting list of all years  in database ???
    getAllYears() {
        return this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; console.log(this.yearList); });
    }


    // handling multi filtered Author list
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


    selectedRegionChange(SelectedRegion) {

        this.pubScreenService.getRegionSubRegion().subscribe(data => {
            this.regionSubregionList = data;
            //console.log(this.regionSubregionList);
            var filtered = this.regionSubregionList.filter(function (item) {
                return SelectedRegion.indexOf(item.rid) !== -1;
            });

            //console.log(filtered);
            this.subRegionList = JSON.parse(JSON.stringify(filtered));
        });

        console.log(this.subRegionList);
    }

    // Handling Error for the required fields
    getErrorMessageAuthor() {

        return this.author.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageTitle() {

        return this.title.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageYear() {
        return this.year.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageYearVal() {
        return this.year.hasError('pattern') ? 'You must enter a valid value' : '';
    }

    getErrorMessageDOI() {
        return this.doi.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessagePaperType() {

        return this.paperType.hasError('required') ? 'You must enter a value' : '';

    }

    getErrorMessageTask() {
        return this.cognitiveTask.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessagePaperOption() {
        return this.addingOption.hasError('required') ? 'You must select one of the options' : '';
    }

    getErrorMessageDOIKey() {
        return this.doiKey.hasError('required') ? 'You must enter the value' : '';
    }

    getErrorMessagePubMedKey() {
        return this.pubMedKey.hasError('required') ? 'You must enter the value' : '';
    }

    getErrorMessagePaperSource() {
        return this.sourceOption.hasError('required') ? 'You must enter the value' : '';
    }

    getErrorMessagePaperOptionBio() {
        return this.bioAddingOption.hasError('required') ? 'You must enter the value' : '';
    }

    getErrorMessageDOIKeyBio() {
        return this.doiKeyBio.hasError('required') ? 'You must enter the value' : '';
    }

    setDisabledVal() {

        if (this.authorModel == null && this.author.hasError('required')) {

            return true;
        }

        if (this.paperTypeModel == null && this.paperType.hasError('required')) {
            return true;
        }

        if (this.sourceOptionModel == 1 && this.addingOption.hasError('required')) {
            return true;

        }

        if (this.sourceOptionModel == 2 && this.bioAddingOption.hasError('required')) {
            return true;

        }

        if (this.addingOptionModel == 1 && this.doiKey.hasError('required')) {
            return true;

        }

        if (this.addingOptionModel == 2 && this.pubMedKey.hasError('required')) {
            return true;

        }

        if (this.bioAddingOptionModel == 1 && this.doiKeyBio.hasError('required')) {
            return true;

        }

        else if (this.title.hasError('required') ||
            this.doi.hasError('required') ||
            this.cognitiveTask.hasError('required') ||
            this.year.hasError('required') ||
            this.year.hasError('pattern') ||
            this.sourceOption.hasError('required')
            

        ) {

            return true;
        }

        else {

            return false;
        }

    }

    setDisabledAddDOI() {
        if (this.doiKey.hasError('required')) {
            return true;
        }
        return false;
    }

    setDisabledAddPubMedID() {
        if (this.pubMedKey.hasError('required')) {
            return true;
        }
        return false;
    }

    setDisabledAddDOIBio() {
        if (this.doiKeyBio.hasError('required')) {
            return true;
        }
        return false;
    }

    // Adding DOI's paper to get some paper's info from PubMed
    addDOI(doi) {

        this.pubScreenService.getPaparInfoFromDOI(doi).subscribe(data => {

            console.log(data);
            console.log(data.result);

            if (data.result == null) {
                alert("DOI is not valid or has not been found!");

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.keywordsModel = data.result.keywords;
                this.doiModel = data.result.doi;
                this.paperTypeModel2 = data.result.paperType;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                this.paperTypeModel = data.result.paperType;
                
            }

        });

    }

    // Adding pubmed key to get paper information from pubMed
    addPubMedID(PubMedKey) {

        this.pubScreenService.getPaparInfoFromPubmedKey(PubMedKey).subscribe(data => {

            console.log(data);
            console.log(data.result);

            if (data.result == null) {
                alert("Pubmed Key is not valid or has not been found!");

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.keywordsModel = data.result.keywords;
                this.doiModel = data.result.doi;
                this.paperTypeModel2 = data.result.paperType;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                this.paperTypeModel = data.result.paperType;
            }

        });

    }

    addDOIBio(doi) {

        this.pubScreenService.getPaparInfoFromDOIBio(doi).subscribe(data => {

            console.log(data);
            console.log(data.result);

            if (data.result == null) {
                alert("DOI is not valid or has not been found!");

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.keywordsModel = data.result.keywords;
                this.doiModel = data.result.doi;
                this.paperTypeModel2 = data.result.paperType;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                this.paperTypeModel = data.result.paperType;

            }

        });

    }

    // Adding a new publication to DB by cliking on Submit button
    AddPublication() {

        if (this.authorModel != null && this.authorModel.length != 0) {
            this._pubscreen.authourID = this.authorModel;
            console.log(this.authorModel)
        }
        else {

            this._pubscreen.author = this.authorList2;
            this._pubscreen.authorString = this.authorModel2;
            console.log(this._pubscreen.author)
            console.log(this.authorList2);

        }

        if (this.paperTypeModel != null) {
            this._pubscreen.paperType = this.paperTypeModel;
        }
        else {
            this._pubscreen.paperType = this.paperTypeModel2;
        }
        if (this.referenceModel != null) {
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

        switch (this.sourceOptionModel) {

            case 1 : {
                this._pubscreen.source = 'pubMed';
                break;
            }
            case 2: {
                this._pubscreen.source = 'BioRxiv';
                break;
            }

        }
         

        this.pubScreenService.addPublication(this._pubscreen).subscribe(data => {

            if (data == null) {
                alert("Publication with the same DOI exists in the database!")
            }
            this.resetFormVals();

        });



    }

    resetFormVals() {

        this.titleModel = '';
        this.abstractModel = '';
        this.keywordsModel = '';
        this.doiModel = '';
        this.yearModel = '';
        this.yearSearchModel = [];
        this.authorModel = [];
        this.paperTypeModel = '';
        this.cognitiveTaskModel = [];
        this.specieModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.diseaseModel = [];
        this.regionModel = [];
        this.subRegionModel = [];
        this.cellTypeModel = [];
        this.methodModel = [];
        this.neurotransmitterModel = [];
        this.doiKeyModel = '';
        this.authorModel2 = '';
        this.paperTypeModel2 = '';
        this.referenceModel = '';
        this.PubMedKeyModel = '';
        this.bioDoiKeyModel = '';

    }

    // Checking the errorMessage for searching publication
    setDisabledValSearch() {

        if (this.cognitiveTask.hasError('required') ||
            this.year.hasError('pattern')

        ) {
            return true;
        }
        return false;


    }

    

}
