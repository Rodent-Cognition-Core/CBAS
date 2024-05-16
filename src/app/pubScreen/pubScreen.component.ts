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
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { ReqGeneralDialogeComponent } from '../reqGeneralDialoge/reqGeneralDialoge.component';
import { CONFIRMDELETE, YEARNEEDSTOBEGREATER } from '../shared/messages';


@Component({
    selector: 'app-pubScreen',
    templateUrl: './pubScreen.component.html',
    styleUrls: ['./pubScreen.component.scss']
})
export class PubScreenComponent implements OnInit {

    pubCount: number;
    featureCount: number;

    authorMultiSelect: any;
    strainMultiSelect: any;
    subTaskMultiSelect: any;
    diseaseMultiSelect: any;
    subModelMultiSelect: any;
    regionMultiSelect: any;
    subRegionMultiSelect: any;
    cellTypeMultiSelect: any;
    methodMultiSelect: any;
    subMethodMultiSelect: any;
    neurotransmitterMultiSelect: any;

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
    subModel: any;
    regionModel: any;
    subRegionModel: any;
    cellTypeModel: any;
    addingOptionModel: any;
    methodModel: any;
    subMethodModel: any;
    neurotransmitterModel: any;
    yearFromSearchModel: any;
    yearToSearchModel: any;
    subTaskModel: any;
    panelOpenState = false;
    searchModel: any;
    // Definiing List Variables 
    paperTypeList: any;
    taskList: any;
    specieList: any;
    sexList: any;
    strainList: any;
    diseaseList: any;
    subModelList: any;
    regionList: any;
    subRegionList: any;
    cellTypeList: any;
    methodList: any;
    subMethodList: any;
    neurotransmitterList: any;
    authorList: any;
    authorList2: any;
    searchResultList: any;
    yearList: any;
    subTaskList: any;
    subSubTaskList: any;
    subStrainList: any;
    subSubRegionList: any;
    subSubModelList: any;
    subSubMethodList: any;
    paperInfoFromDoiList: any;
    checkYear: boolean;


    isAdmin: boolean;
    isUser: boolean;
    isFullDataAccess: boolean;

    //yearFrom = new FormControl('', []);
    yearTo = new FormControl('', []);

    _pubSCreenSearch = new Pubscreen();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public strainMultiFilterCtrl: FormControl = new FormControl();
    public filteredStrainList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subTaskMultiFilterCtrl: FormControl = new FormControl();
    public filteredSubTaskList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public diseaseMultiFilterCtrl: FormControl = new FormControl();
    public filteredDiseaseList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subModelMultiFilterCtrl: FormControl = new FormControl();
    public filteredSubModelList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public regionMultiFilterCtrl: FormControl = new FormControl();
    public filteredRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subRegionMultiFilterCtrl: FormControl = new FormControl();
    public filteredSubRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public cellTypeMultiFilterCtrl: FormControl = new FormControl();
    public filteredCellTypeList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public methodMultiFilterCtrl: FormControl = new FormControl();
    public filteredMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subMethodMultiFilterCtrl: FormControl = new FormControl();
    public filteredSubMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public neurotransmitterMultiFilterCtrl: FormControl = new FormControl();
    public filteredNeurotransmitterList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    dialogRef: MatDialogRef<DeleteConfirmDialogComponent>;

    dialogRefLink: MatDialogRef<NotificationDialogComponent>;
    showGeneratedLink: any;

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        private spinnerService: Ng4LoadingSpinnerService,
        public dialogML: MatDialog,    ) { }

    ngOnInit() {

        this.pubScreenService.getPubCount().subscribe(data => {
            this.pubCount = data['pubCount'];
            this.featureCount = data['featureCount'];
        }, error => {
            console.error('Error fetching pub count:', error);
        });

        this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe(data => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe(data => { this.taskList = data; });
        this.GetSubTaskList();
        // this.pubScreenService.getTaskSubTask().subscribe(data => { this.subTaskList = data; });
        this.pubScreenService.getSpecie().subscribe(data => { this.specieList = data; });
        this.pubScreenService.getSex().subscribe(data => { this.sexList = data; });
        this.GetStrainList();
        this.GetDiseaseList();
        this.GetSubModelList();
        this.GetRegionList();
        this.GetSubRegionList();
        this.GetCellTypeList();
        this.GetMethodList();
        this.GetSubMethodList();
        this.GetNeurotransmitterList();

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.yearList = this.GetYear(1970).sort().reverse();       

    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    // Function definition to get list of years
    GetYear(startYear) {
        var currentYear = new Date().getFullYear(), years = [];
        startYear = startYear || 1980;
        while (startYear <= currentYear) {
            years.push(startYear++);
        }
        return years;
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

    GetStrainList() {

        this.pubScreenService.getStrain().subscribe(data => {
            this.strainList = data;

            this.strainMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterStrain();
                });

        });

        return this.strainList;
    }

    GetSubTaskList() {

        this.pubScreenService.getTaskSubTask().subscribe(data => {
            this.subTaskList = data;

            this.subTaskMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterSubTask();
                });

        });

        return this.subTaskList;
    }

    GetDiseaseList() {

        this.pubScreenService.getDisease().subscribe(data => {
            this.diseaseList = data;
            this.filteredDiseaseList.next(this.diseaseList.slice());
            this.diseaseMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterDisease();
                });

        });

        return this.diseaseList;
    }

    GetSubModelList() {

        this.pubScreenService.getSubModels().subscribe(data => {
            this.subModelList = data;

            this.subModelMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterSubModel();
                });

        });

        return this.subModelList;
    }

    GetRegionList() {

        this.pubScreenService.getRegion().subscribe(data => {
            this.regionList = data;
            this.filteredRegionList.next(this.regionList.slice());
            this.regionMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterRegion();
                });

        });

        return this.regionList;
    }

    GetSubRegionList() {

        this.pubScreenService.getRegionSubRegion().subscribe(data => {
            this.subRegionList = data;

            this.subRegionMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterSubRegion();
                });

        });

        return this.subRegionList;
    }

    GetCellTypeList() {

        this.pubScreenService.getCellType().subscribe(data => {
            this.cellTypeList = data;
            this.filteredCellTypeList.next(this.cellTypeList.slice());
            this.cellTypeMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterCellType();
                });

        });

        return this.cellTypeList;
    }

    GetMethodList() {

        this.pubScreenService.getMethod().subscribe(data => {
            this.methodList = data;
            this.filteredMethodList.next(this.methodList.slice());
            this.methodMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterMethod();
                });

        });

        return this.methodList;
    }

    GetSubMethodList() {

        this.pubScreenService.getSubMethod().subscribe(data => {
            this.subMethodList = data;

            this.subMethodMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterSubMethod();
                });

        });

        return this.subMethodList;
    }

    GetNeurotransmitterList() {

        this.pubScreenService.getNeurotransmitter().subscribe(data => {
            this.neurotransmitterList = data;
            this.filteredNeurotransmitterList.next(this.neurotransmitterList.slice());
            this.neurotransmitterMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterNeurotransmitter();
                });

        });

        return this.neurotransmitterList;
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

    // handling multi filtered Strain list
    private filterStrain() {
        if (!this.subStrainList) {
            return;
        }

        // get the search keyword
        let searchStrain = this.strainMultiFilterCtrl.value;

        if (!searchStrain) {
            this.filteredStrainList.next(this.subStrainList.slice());
            return;
        } else {
            searchStrain = searchStrain.toLowerCase();
        }

        // filter the strain
        this.filteredStrainList.next(
            this.subStrainList.filter(x => x.strain.toLowerCase().indexOf(searchStrain) > -1)
        );
    }

    // handling multi filtered SubTask list
    private filterSubTask() {
        if (!this.subSubTaskList) {
            return;
        }

        // get the search keyword
        let searchSubTask = this.subTaskMultiFilterCtrl.value;

        if (!searchSubTask) {
            this.filteredSubTaskList.next(this.subSubTaskList.slice());
            return;
        } else {
            searchSubTask = searchSubTask.toLowerCase();
        }

        // filter the SubTask
        this.filteredSubTaskList.next(
            this.subSubTaskList.filter(x => x.subTask.toLowerCase().indexOf(searchSubTask) > -1)
        );
    }

    // handling multi filtered Disease list
    private filterDisease() {
        if (!this.diseaseList) {
            return;
        }

        // get the search keyword
        let searchDisease = this.diseaseMultiFilterCtrl.value;

        if (!searchDisease) {
            this.filteredDiseaseList.next(this.diseaseList.slice());
            return;
        } else {
            searchDisease = searchDisease.toLowerCase();
        }

        // filter the disease
        this.filteredDiseaseList.next(
            this.diseaseList.filter(x => x.diseaseModel.toLowerCase().indexOf(searchDisease) > -1)
        );
    }

    // handling multi filtered SubTask list
    private filterSubModel() {
        if (!this.subSubModelList) {
            return;
        }

        // get the search keyword
        let searchSubModel = this.subModelMultiFilterCtrl.value;

        if (!searchSubModel) {
            this.filteredSubModelList.next(this.subSubModelList.slice());
            return;
        } else {
            searchSubModel = searchSubModel.toLowerCase();
        }

        // filter the SubTask
        this.filteredSubModelList.next(
            this.subSubModelList.filter(x => x.subModel.toLowerCase().indexOf(searchSubModel) > -1)
        );
    }

    // handling multi filtered Disease list
    private filterRegion() {
        if (!this.regionList) {
            return;
        }

        // get the search keyword
        let searchRegion = this.regionMultiFilterCtrl.value;

        if (!searchRegion) {
            this.filteredRegionList.next(this.regionList.slice());
            return;
        } else {
            searchRegion = searchRegion.toLowerCase();
        }

        // filter the disease
        this.filteredRegionList.next(
            this.regionList.filter(x => x.brainRegion.toLowerCase().indexOf(searchRegion) > -1)
        );
    }

    // handling multi filtered Sub Region list
    private filterSubRegion() {
        if (!this.subSubRegionList) {
            return;
        }

        // get the search keyword
        let searchSubRegion = this.subRegionMultiFilterCtrl.value;

        if (!searchSubRegion) {
            this.filteredSubRegionList.next(this.subSubRegionList.slice());
            return;
        } else {
            searchSubRegion = searchSubRegion.toLowerCase();
        }

        // filter the SubTask
        this.filteredSubRegionList.next(
            this.subSubRegionList.filter(x => x.subRegion.toLowerCase().indexOf(searchSubRegion) > -1)
        );
    }

    // handling multi filtered Disease list
    private filterCellType() {
        if (!this.cellTypeList) {
            return;
        }

        // get the search keyword
        let searchCellType = this.cellTypeMultiFilterCtrl.value;

        if (!searchCellType) {
            this.filteredCellTypeList.next(this.cellTypeList.slice());
            return;
        } else {
            searchCellType = searchCellType.toLowerCase();
        }

        // filter the disease
        this.filteredCellTypeList.next(
            this.cellTypeList.filter(x => x.cellType.toLowerCase().indexOf(searchCellType) > -1)
        );
    }

    // handling multi filtered Disease list
    private filterMethod() {
        if (!this.methodList) {
            return;
        }

        // get the search keyword
        let searchMethod = this.methodMultiFilterCtrl.value;

        if (!searchMethod) {
            this.filteredMethodList.next(this.methodList.slice());
            return;
        } else {
            searchMethod = searchMethod.toLowerCase();
        }

        // filter the disease
        this.filteredMethodList.next(
            this.methodList.filter(x => x.method.toLowerCase().indexOf(searchMethod) > -1)
        );
    }

    // handling multi filtered SubTask list
    private filterSubMethod() {
        if (!this.subSubMethodList) {
            return;
        }

        // get the search keyword
        let searchSubMethod = this.subMethodMultiFilterCtrl.value;

        if (!searchSubMethod) {
            this.filteredSubMethodList.next(this.subSubMethodList.slice());
            return;
        } else {
            searchSubMethod = searchSubMethod.toLowerCase();
        }

        // filter the SubTask
        this.filteredSubMethodList.next(
            this.subSubMethodList.filter(x => x.subMethod.toLowerCase().indexOf(searchSubMethod) > -1)
        );
    }

    // handling multi filtered Disease list
    private filterNeurotransmitter() {
        if (!this.neurotransmitterList) {
            return;
        }

        // get the search keyword
        let searchNeurotransmitter = this.neurotransmitterMultiFilterCtrl.value;

        if (!searchNeurotransmitter) {
            this.filteredNeurotransmitterList.next(this.neurotransmitterList.slice());
            return;
        } else {
            searchNeurotransmitter = searchNeurotransmitter.toLowerCase();
        }

        // filter the disease
        this.filteredNeurotransmitterList.next(
            this.neurotransmitterList.filter(x => x.neuroTransmitter.toLowerCase().indexOf(searchNeurotransmitter) > -1)
        );
    }

    selectedTaskChange(SelectedTask) {
        this.subTaskModel = [];
        this.subSubTaskList = this.subTaskList.filter(x => SelectedTask.includes(x.taskID));
        this.filteredSubTaskList.next(this.subSubTaskList.slice());
    }

    selectedSpeciesChange(SelectedSpecies) {
        this.strainModel = [];
        this.subStrainList = this.strainList.filter(x => SelectedSpecies.includes(x.speciesID));
        this.filteredStrainList.next(this.subStrainList.slice());
    }

    selectedModelChange(SelectedModels) {
        this.subModel = [];
        this.subSubModelList = this.subModelList.filter(x => SelectedModels.includes(x.modelID));
        this.filteredSubModelList.next(this.subSubModelList.slice());
    }

    selectedMethodChange(SelectedMethods) {
        this.subMethodModel = [];
        this.subSubMethodList = this.subMethodList.filter(x => SelectedMethods.includes(x.methodID));
        this.filteredSubMethodList.next(this.subSubMethodList.slice());
    }

    selectedRegionChange(SelectedRegion) {
        this.subRegionModel = [];
        this.subSubRegionList = this.subRegionList.filter(x => SelectedRegion.includes(x.rid));
        this.filteredSubRegionList.next(this.subSubRegionList.slice());
    }

    setDisabledValSearch() {

        return false;

    }

    // Opening Dialog for adding a new publication.
    openDialogAddPublication(Publication): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: {
                publicationObj: Publication,
                //sourceOptionModel: this.sourceOptionModel,

            }

        });

        dialogref.afterClosed().subscribe(result => {
            //console.log('the dialog was closed');
            //this.DialogResult = result;
            this.GetAuthorList();
        });
    }

    // Edit publication
    openDialogEditPublication(Publication): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: { publicationObj: Publication }

        });

        dialogref.afterClosed().subscribe(result => {
            //console.log('the dialog was closed');
            this.search();
        });
    }
    
    selectYearToChange(yearFromVal, yearToVal) {
        //console.log(yearToVal)
        yearFromVal = yearFromVal === null ? 0 : yearFromVal;
        
        yearToVal < yearFromVal ? this.yearTo.setErrors({ 'incorrect': true }) : false;

    }

    
    getErrorMessageYearTo() {
        //return this.yearTo.getError('Year To should be greater than Year From');
        return YEARNEEDSTOBEGREATER;
    }

    // Function definition for searching publications based on search criteria
    search() {

        this._pubSCreenSearch.authourID = this.authorModel;
        this._pubSCreenSearch.title = this.titleModel;
        this._pubSCreenSearch.keywords = this.keywordsModel;
        this._pubSCreenSearch.doi = this.doiModel;
        //this._pubSCreenSearch.year = this.yearModel;
        //this._pubSCreenSearch.years = this.yearSearchModel;
        this._pubSCreenSearch.paperTypeIdSearch = this.paperTypeModel;
        this._pubSCreenSearch.taskID = this.cognitiveTaskModel;
        this._pubSCreenSearch.specieID = this.specieModel;
        this._pubSCreenSearch.sexID = this.sexModel;
        this._pubSCreenSearch.strainID = this.strainModel;
        this._pubSCreenSearch.diseaseID = this.diseaseModel;
        this._pubSCreenSearch.subModelID = this.subModel;
        this._pubSCreenSearch.regionID = this.regionModel;
        this._pubSCreenSearch.subRegionID = this.subRegionModel;
        this._pubSCreenSearch.cellTypeID = this.cellTypeModel;
        this._pubSCreenSearch.methodID = this.methodModel;
        this._pubSCreenSearch.subMethodID = this.subMethodModel;
        this._pubSCreenSearch.transmitterID = this.neurotransmitterModel;
        this._pubSCreenSearch.yearFrom = this.yearFromSearchModel;
        this._pubSCreenSearch.yearTo = this.yearToSearchModel;
        this._pubSCreenSearch.subTaskID = this.subTaskModel;
        this._pubSCreenSearch.search = this.searchModel;

        console.log(this._pubSCreenSearch);

        this.pubScreenService.searchPublication(this._pubSCreenSearch).subscribe(data => {

            this.searchResultList = data;
        });

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
        this.dialogRef.componentInstance.confirmMessage = CONFIRMDELETE


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

    getLink(guid) {

            this.dialogRefLink = this.dialog.open(NotificationDialogComponent, {
            });
            this.dialogRefLink.componentInstance.message = "http://localhost:4200/pubScreen-edit?paperlinkguid=" + guid;

    }

    //Function Definition to open a dialog for adding new mouse line to the system
    openDialogGeneral(): void {

        let dialogref = this.dialogML.open(ReqGeneralDialogeComponent, {
            height: '600px',
            width: '700px',
            data: {}

        });

    }

}


