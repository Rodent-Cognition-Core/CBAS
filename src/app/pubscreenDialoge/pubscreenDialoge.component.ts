import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PISiteService } from '../services/piSite.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
//import { UploadService } from '../services/upload.service';
import { SharedModule } from '../shared/shared.module';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { IdentityService } from '../services/identity.service';
import { Subject } from 'rxjs/Subject';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
import { take, takeUntil } from 'rxjs/operators';

@Component({

    selector: 'app-pubscreenDialoge',
    templateUrl: './pubscreenDialoge.component.html',
    styleUrls: ['./pubscreenDialoge.component.scss'],
    providers: [TaskAnalysisService,  PISiteService]

})
export class PubscreenDialogeComponent implements OnInit {

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
    //paperTypeModel2: any;
    referenceModel: any;
    sourceOptionModel: any;
    bioAddingOptionModel: any;
    bioDoiKeyModel: any;
    taskOtherModel: string;
    specieOtherModel: string;
    strainOtherModel: string;
    diseaseOtherModel: string;
    cellOtherModel: string;
    methodOtherModel: string;
    neurotransmitterOtherModel: string;

    isEditMode: boolean;
    publicationId: number;

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
    paperInfo: any;
    

    private form: FormGroup;



    //onbj variable from Models
    _pubscreen = new Pubscreen();
    _pubSCreenSearch = new Pubscreen();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(
        public thisDialogRef: MatDialogRef<PubscreenDialogeComponent>,
        // private pagerService: PagerService,
        private spinnerService: Ng4LoadingSpinnerService,
        public dialog: MatDialog,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,) {

        this.resetFormVals();
    }

    ngOnInit() {

        this.isEditMode = false;

        this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe(data => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe(data => { this.taskList = data; this.processList(this.taskList, "None", "task"); this.processList(this.taskList, "Other", "task");  });
        this.pubScreenService.getSpecie().subscribe(data => { this.specieList = data; this.processList(this.specieList, "Other", "species"); });
        this.pubScreenService.getSex().subscribe(data => { this.sexList = data; });
        this.pubScreenService.getStrain().subscribe(data => { this.strainList = data; this.processList(this.strainList, "Other", "strain"); });
        this.pubScreenService.getDisease().subscribe(data => { this.diseaseList = data; this.processList(this.diseaseList, "Other", "diseaseModel"); });
        this.pubScreenService.getRegion().subscribe(data => { this.regionList = data; });
        this.pubScreenService.getCellType().subscribe(data => { this.cellTypeList = data; this.processList(this.cellTypeList, "Other", "cellType"); });
        this.pubScreenService.getMethod().subscribe(data => { this.methodList = data; this.processList(this.methodList, "Other", "method"); });
        this.pubScreenService.getNeurotransmitter().subscribe(data => { this.neurotransmitterList = data; this.processList(this.neurotransmitterList, "Other", "neuroTransmitter"); });

        //this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; console.log(this.yearList); });
        this.getAllYears();

        console.log(this.data);
        // if it is an Edit model
        if (this.data.publicationObj != null) {

            this.isEditMode = true;
            this.publicationId = this.data.publicationObj.id;

            this.spinnerService.show();

            this.pubScreenService.getPaperInfo(this.publicationId).subscribe(data => {
                this.paperInfo = data;

                this.doiModel = this.paperInfo.doi;
                this.keywordsModel = this.paperInfo.keywords;
                this.titleModel = this.paperInfo.title;
                this.abstractModel = this.paperInfo.abstract;
                this.yearModel = this.paperInfo.year;
                this.referenceModel = this.paperInfo.reference;

                this.authorModel = this.paperInfo.authourID;
                this.cellTypeModel = this.paperInfo.cellTypeID;
                this.diseaseModel = this.paperInfo.diseaseID;
                this.methodModel = this.paperInfo.methodID;
                this.paperTypeModel = this.paperInfo.paperType;
                this.regionModel = this.paperInfo.regionID;
                this.sexModel = this.paperInfo.sexID;
                this.specieModel = this.paperInfo.specieID;
                this.strainModel = this.paperInfo.strainID;

                this.pubScreenService.getRegionSubRegion().subscribe(dataSubRegion => {
                    this.selectedRegionChange(this.regionModel)
                    this.subRegionModel = this.paperInfo.subRegionID;
                });

                this.cognitiveTaskModel = this.paperInfo.taskID;
                this.neurotransmitterModel = this.paperInfo.transmitterID;

                this.sourceOptionModel = 3;

                this.setDisabledVal();

                this.spinnerService.hide();
            });




 
        }

    }

    //ngAfterViewInit() {
    //    console.log("ngAfterViewInit");
    //}


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

        //this.resetFormVals();

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
            console.log(this.regionSubregionList);
            var filtered = this.regionSubregionList.filter(function (item) {
                return SelectedRegion.indexOf(item.rid) !== -1;
            });

            //console.log(filtered);
            this.subRegionList = JSON.parse(JSON.stringify(filtered));
        });

        console.log(this.subRegionList);
    }


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
    year = new FormControl('', [Validators.required]);
    pubMedKey = new FormControl('', [Validators.required]);
    sourceOption = new FormControl('', [Validators.required]);
    bioAddingOption = new FormControl('', [Validators.required]);
    doiKeyBio = new FormControl('', [Validators.required]);

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

        if (this.authorModel === null && this.author.hasError('required')) {

            return true;
        }

        if (this.paperTypeModel === null && this.paperType.hasError('required')) {
            return true;
        }

        if (this.sourceOptionModel === 1 && this.addingOption.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.sourceOptionModel === 2 && this.bioAddingOption.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.sourceOptionModel === 3 && this.author.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.addingOptionModel === 1 && this.doiKey.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.addingOptionModel === 2 && this.pubMedKey.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.bioAddingOptionModel === 1 && this.doiKeyBio.hasError('required') && !this.isEditMode) {
            return true;

        }

        else if (
            
            ((this.titleModel === null || this.titleModel === "") && this.title.hasError('required')) ||
            ((this.doiModel === null || this.doiModel === "") && this.doi.hasError('required'))||
            ((this.cognitiveTaskModel === null || this.cognitiveTaskModel.length === 0) && this.cognitiveTask.hasError('required'))||
            ((this.yearModel === null || this.yearModel === "") && this.year.hasError('required')) ||
            ((this.sourceOptionModel === null || this.sourceOptionModel === "") && this.sourceOption.hasError('required')) ||
            (this.paperType.hasError('required'))

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

            //console.log(data);
            //console.log(data.result);

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
                //this.paperTypeModel2 = data.result.paperType;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                //this.paperTypeModel = data.result.paperType;

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
                //this.paperTypeModel2 = data.result.paperType;
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
                //this.paperTypeModel2 = data.result.paperType;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                this.paperTypeModel = data.result.paperType;

            }

        });

    }


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

        //if (this.paperTypeModel !== null) {
        //    this._pubscreen.paperType = this.paperTypeModel;
        //}
        //else {
        //    this._pubscreen.paperType = this.paperTypeModel2;
        //}

        if (this.referenceModel !== null) {
            this._pubscreen.reference = this.referenceModel;
        }

        this._pubscreen.title = this.titleModel;
        this._pubscreen.abstract = this.abstractModel;
        this._pubscreen.keywords = this.keywordsModel;
        this._pubscreen.doi = this.doiModel;
        this._pubscreen.year = this.yearModel;
        this._pubscreen.paperType = this.paperTypeModel;
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
        this.paperTypeModel = '';
        this.referenceModel = '';
        this.PubMedKeyModel = '';
        this.bioDoiKeyModel = '';

        this.taskOtherModel = '';
        this.specieOtherModel = '';
        this.strainOtherModel = '';
        this.diseaseOtherModel = '';
        this.cellOtherModel = '';
        this.methodOtherModel = '';
        this.neurotransmitterOtherModel = '';

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

    processList(data, item, propertyName) {

        const ret = data.filter(row => (row[propertyName] === item));
        if (ret.length > 0) {
            data.splice(data.findIndex(row => (row[propertyName] === item)), 1);
            data.push(ret[0])
        }
        
        return data
    }
    




}
