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
    specie = new FormControl('', [Validators.required]);
    sex = new FormControl('', [Validators.required]);
    addingOption = new FormControl('', [Validators.required]);
    year = new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    pubMedKey = new FormControl('', [Validators.required]);

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

        this.GetAuthorList();
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

    // Getting list of all years  in database
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

        // filter the Experiment
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

    getErrorMessageSpecie() {
        return this.specie.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessageSex() {
        return this.sex.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessagePaperOption() {
        return this.addingOption.hasError('required') ? 'You must select one of the options': '';
    }

    getErrorMessageDOIKey() {
        return this.doiKey.hasError('required') ? 'You must enter the value' : ''; 
    }

    getErrorMessagePubMedKey() {
        return this.pubMedKey.hasError('required') ? 'You must enter the value' : '';
    }

    setDisabledVal() {

        if (this.title.hasError('required') ||
            this.author.hasError('required') ||
            this.doi.hasError('required') ||
            this.paperType.hasError('required') ||
            this.cognitiveTask.hasError('required') ||
            this.specie.hasError('required') ||
            this.sex.hasError('required') ||
            this.year.hasError('required') ||
            this.year.hasError('pattern') ||
            this.addingOption.hasError('required')
            //this.doiKey.hasError('required') 

        ) {
            return true;
        }
        return false;
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

    // Adding DOI's paper to get some paper's info
    addDOI(doi) {

        this.pubScreenService.getPaparInfoFromDOI(doi).subscribe(data => {

            this.paperInfoFromDoiList = data;

        });

    }

    // Adding a new publication to DB by cliking on Submit button
    AddPublication() {

        this._pubscreen.title = this.titleModel;
        this._pubscreen.abstract = this.abstractModel;
        this._pubscreen.keywords = this.keywordsModel;
        this._pubscreen.doi = this.doiModel;
        this._pubscreen.year = this.yearModel;
        this._pubscreen.authourID = this.authorModel;
        this._pubscreen.paperTypeID = this.paperTypeModel;
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

        this.pubScreenService.addPublication(this._pubscreen).subscribe(data => {

            this.resetFormVals();
            this.getAllYears();

            
                        
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

    // Function definition for searching publications based on search criteria
    search() {

        this._pubSCreenSearch.title = this.titleModel;
        this._pubSCreenSearch.abstract = this.abstractModel;
        this._pubSCreenSearch.keywords = this.keywordsModel;
        this._pubSCreenSearch.doi = this.doiModel;
        this._pubSCreenSearch.year = this.yearModel;
        this._pubSCreenSearch.years = this.yearSearchModel;
        this._pubSCreenSearch.authourID = this.authorModel;
        this._pubSCreenSearch.paperTypeID = this.paperTypeModel;
        this._pubSCreenSearch.taskID = this.cognitiveTaskModel;
        this._pubSCreenSearch.specieID = this.specieModel;
        this._pubSCreenSearch.sexID = this.sexModel;
        this._pubSCreenSearch.strainID = this.strainModel;
        this._pubSCreenSearch.diseaseID = this.diseaseModel;
        this._pubSCreenSearch.regionID = this.regionModel;
        this._pubSCreenSearch.subRegionID = this.subRegionModel;
        this._pubSCreenSearch.cellTypeID = this.cellTypeModel;
        this._pubSCreenSearch.methodID = this.methodModel;
        this._pubSCreenSearch.transmitterID = this.neurotransmitterModel;
        console.log(this._pubSCreenSearch);

        this.pubScreenService.searchPublication(this._pubSCreenSearch).subscribe(data => {

            this.searchResultList = data;
            console.log(this.searchResultList);
        });

    }













}
