import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormControl, FormBuilder } from '@angular/forms';
import { map } from 'rxjs/operators';
import { ReplaySubject ,  Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { NgxSpinnerService } from 'ngx-spinner';
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

    authorModel: Array<number>;
    titleModel: any;
    abstractModel: any;
    yearModel: any;
    keywordsModel: any;
    doiModel: any;
    paperTypeModel: Array<number>;
    cognitiveTaskModel: Array<number>;
    specieModel: Array<number>;
    sexModel: Array<number>;
    strainModel: Array<number>;
    diseaseModel: Array<number>;
    subModel: Array<number>;
    regionModel: Array<number>;
    subRegionModel: Array<number>;
    cellTypeModel: Array<number>;
    addingOptionModel: any;
    methodModel: Array<number>;
    subMethodModel: Array<number>;
    neurotransmitterModel: Array<number>;
    yearFromSearchModel: any;
    subTaskModel: Array<number>;
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

    _pubSCreenSearch: Pubscreen;

    public authorMultiFilterCtrl: FormControl;
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public strainMultiFilterCtrl: FormControl;
    public filteredStrainList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subTaskMultiFilterCtrl: FormControl;
    public filteredSubTaskList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public diseaseMultiFilterCtrl: FormControl;
    public filteredDiseaseList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subModelMultiFilterCtrl: FormControl;
    public filteredSubModelList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public regionMultiFilterCtrl: FormControl;
    public filteredRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subRegionMultiFilterCtrl: FormControl;
    public filteredSubRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public cellTypeMultiFilterCtrl: FormControl;
    public filteredCellTypeList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public methodMultiFilterCtrl: FormControl;
    public filteredMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subMethodMultiFilterCtrl: FormControl;
    public filteredSubMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public neurotransmitterMultiFilterCtrl: FormControl;
    public filteredNeurotransmitterList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();



    showGeneratedLink: any;

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        private spinnerService: NgxSpinnerService,
        public dialogML: MatDialog,
        private fb: FormBuilder,
        //public dialogRef: MatDialogRef<DeleteConfirmDialogComponent>,
        public dialogRef: MatDialog,
        public dialogRefLink: MatDialog) { 

        this.pubCount = 0;
        this.featureCount = 0;
        this.checkYear = false;
        this.isAdmin = false;
        this.isUser = false;
        this.isFullDataAccess = false;
        this.authorModel = [];
        this.titleModel = '';
        this.doiModel = '';
        this.keywordsModel = '';
        this.searchModel = '';
        this.authorModel = [];
        this.cognitiveTaskModel = [];
        this.paperTypeModel = [];
        this.specieModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.diseaseModel = [];
        this.subModel = [];
        this.regionModel = [];
        this.subRegionModel = [];
        this.cellTypeModel = [];
        this.methodModel = [];
        this.subMethodModel = [];
        this.neurotransmitterModel = [];
        this.subTaskModel = [];
        this.authorMultiFilterCtrl = fb.control([]),
        this.strainMultiFilterCtrl = fb.control([]),
        this.subTaskMultiFilterCtrl = fb.control([]),
        this.diseaseMultiFilterCtrl = fb.control([]),
        this.subModelMultiFilterCtrl = fb.control([]),
        this.regionMultiFilterCtrl = fb.control([]),
        this.subRegionMultiFilterCtrl = fb.control([]),
        this.cellTypeMultiFilterCtrl = fb.control([]),
        this.methodMultiFilterCtrl = fb.control([]),
        this.subMethodMultiFilterCtrl = fb.control([]),
        this.neurotransmitterMultiFilterCtrl = fb.control([]),
        this._pubSCreenSearch = {
            abstract: '',
            author: [],
            authorString: '',
            authourID: [],
            cellTypeID: [],
            celltypeOther: '',
            diseaseID: [],
            diseaseOther: '',
            doi: '',
            id: undefined,
            keywords: '',
            methodID: [],
            methodOther: '',
            neurotransOther: '',
            paperType: '',
            paperTypeID: undefined,
            paperTypeIdSearch: [],
            reference: '',
            regionID: [],
            search: '',
            sexID: [],
            source: '',
            specieID: [],
            specieOther: '',
            strainID: [],
            strainMouseOther: '',
            strainRatOther: '',
            subMethodID: [],
            subModelID: [],
            subRegionID: [],
            subTaskID: [],
            taskID: [],
            taskOther: '',
            title: '',
            transmitterID: [],
            year: undefined,
            yearFrom: undefined,
            yearID: [],
            yearTo: undefined
        }
        }

    ngOnInit() {

        this.pubScreenService.getPubCount().subscribe((data : any) => {
            this.pubCount = data['pubCount'];
            this.featureCount = data['featureCount'];
        }, (error : any) => {
            console.error('Error fetching pub count:', error);
        });

        this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe((data : any) => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe((data: any) => { this.taskList = data; });
        this.GetSubTaskList();
        // this.pubScreenService.getTaskSubTask().subscribe(data => { this.subTaskList = data; });
        this.pubScreenService.getSpecie().subscribe((data: any) => { this.specieList = data; });
        this.pubScreenService.getSex().subscribe((data: any) => { this.sexList = data; });
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
    GetYear(startYear : number) {
        var currentYear = new Date().getFullYear(), years = [];
        startYear = startYear || 1980;
        while (startYear <= currentYear) {
            years.push(startYear++);
        }
        return years;
    }


    GetAuthorList() {


        this.pubScreenService.getAuthor().subscribe((data: any) => {
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

        this.pubScreenService.getStrain().subscribe((data: any) => {
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

        this.pubScreenService.getTaskSubTask().subscribe((data: any) => {
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

        this.pubScreenService.getDisease().subscribe((data: any) => {
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

        this.pubScreenService.getSubModels().subscribe((data: any) => {
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

        this.pubScreenService.getRegion().subscribe((data: any) => {
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

        this.pubScreenService.getRegionSubRegion().subscribe((data: any) => {
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

        this.pubScreenService.getCellType().subscribe((data: any) => {
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

        this.pubScreenService.getMethod().subscribe((data: any) => {
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

        this.pubScreenService.getSubMethod().subscribe((data: any) => {
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

        this.pubScreenService.getNeurotransmitter().subscribe((data: any) => {
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
            this.authorList.filter((x : any) => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
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
            this.subStrainList.filter((x: any) => x.strain.toLowerCase().indexOf(searchStrain) > -1)
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
            this.subSubTaskList.filter((x: any) => x.subTask.toLowerCase().indexOf(searchSubTask) > -1)
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
            this.diseaseList.filter((x: any) => x.diseaseModel.toLowerCase().indexOf(searchDisease) > -1)
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
            this.subSubModelList.filter((x: any) => x.subModel.toLowerCase().indexOf(searchSubModel) > -1)
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
            this.regionList.filter((x: any) => x.brainRegion.toLowerCase().indexOf(searchRegion) > -1)
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
            this.subSubRegionList.filter((x: any) => x.subRegion.toLowerCase().indexOf(searchSubRegion) > -1)
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
            this.cellTypeList.filter((x: any) => x.cellType.toLowerCase().indexOf(searchCellType) > -1)
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
            this.methodList.filter((x: any) => x.method.toLowerCase().indexOf(searchMethod) > -1)
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
            this.subSubMethodList.filter((x: any) => x.subMethod.toLowerCase().indexOf(searchSubMethod) > -1)
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
            this.neurotransmitterList.filter((x : any) => x.neuroTransmitter.toLowerCase().indexOf(searchNeurotransmitter) > -1)
        );
    }

    selectedTaskChange(SelectedTask : number[]) {
        this.subTaskModel = [];
        this.subSubTaskList = this.subTaskList.filter((x : any) => SelectedTask.includes(x.taskID));
        this.filteredSubTaskList.next(this.subSubTaskList.slice());
    }

    selectedSpeciesChange(SelectedSpecies: number[]) {
        debugger;
        this.strainModel = [];
        this.subStrainList = this.strainList.filter((x: any) => SelectedSpecies.includes(x.speciesID));
        this.filteredStrainList.next(this.subStrainList.slice());
    }

    selectedModelChange(SelectedModels: number[]) {
        this.subModel = [];
        this.subSubModelList = this.subModelList.filter((x: any) => SelectedModels.includes(x.modelID));
        this.filteredSubModelList.next(this.subSubModelList.slice());
    }

    selectedMethodChange(SelectedMethods : number[]) {
        this.subMethodModel = [];
        this.subSubMethodList = this.subMethodList.filter((x: any) => SelectedMethods.includes(x.methodID));
        this.filteredSubMethodList.next(this.subSubMethodList.slice());
    }

    selectedRegionChange(SelectedRegion : number[]) {
        this.subRegionModel = [];
        this.subSubRegionList = this.subRegionList.filter((x: any) => SelectedRegion.includes(x.rid));
        this.filteredSubRegionList.next(this.subSubRegionList.slice());
    }

    setDisabledValSearch() {

        return false;

    }

    // Opening Dialog for adding a new publication.
    openDialogAddPublication(Publication?: any): void {
        if (Publication == 'undefined') {
            Publication = null;
        }
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
    openDialogEditPublication(Publication : any): void {
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
    
    selectYearToChange(yearFromVal: number, yearToVal: number) {
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
        this.spinnerService.show();
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
        this._pubSCreenSearch.yearTo = (this.yearTo.value === '') ? undefined : +this.yearTo.value;
        this._pubSCreenSearch.subTaskID = this.subTaskModel;
        this._pubSCreenSearch.search = this.searchModel;

        console.log(this._pubSCreenSearch);

        this.pubScreenService.searchPublication(this._pubSCreenSearch).subscribe(data => {

            this.searchResultList = data;
            this.spinnerService.hide();
        });

    }

    // Deleting publication
    delPub(pubRow: any) {
        this.openConfirmationDialog(pubRow.id);
    }

    // Deleting Experiment
    openConfirmationDialog(pubID: any) {
        const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRef.componentInstance.confirmMessage = CONFIRMDELETE


        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                console.log(pubID);

                this.pubScreenService.deletePublicationById(pubID).pipe(map((res : any) => {


                    this.spinnerService.hide();


                    location.reload()

                })).subscribe();
            }
            //this.dialogRef = null;
            dialogRef.close();
        });
    }

    getLink(guid : any) {

            const dialogRefLink = this.dialog.open(NotificationDialogComponent, {
            });
            dialogRefLink.componentInstance.message = "http://localhost:4200/pubScreen-edit?paperlinkguid=" + guid;

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


