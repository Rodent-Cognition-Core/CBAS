import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UntypedFormControl, UntypedFormBuilder } from '@angular/forms';
import { ReplaySubject ,  Subject, Subscription } from 'rxjs';
import { takeUntil, filter, debounceTime } from 'rxjs/operators';
import { CogbytesService } from '../services/cogbytes.service'
import { PISiteService } from '../services/piSite.service';
import { PubScreenService } from '../services/pubScreen.service';
import { CogbytesSearch } from '../models/cogbytesSearch'
import { AuthenticationService } from '../services/authentication.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { INVALIDYEAR } from '../shared/messages';


@Component({
    selector: 'app-cogbytes-search',
    templateUrl: './cogbytesSearch.component.html',
    styleUrls: ['./cogbytesSearch.component.scss']
})
export class CogbytesSearchComponent implements OnInit, OnDestroy {

    readonly DATASET = 1;

    authorModel: any;
    piModel: any;
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
    genoModel: any;
    ageStartModel: any;
    ageEndModel: any;
    interventionModel: any;

    public diseaseModel: Array<number>;
    public subModel: Array<number>;
    public regionModel: Array<number>;
    public subRegionModel: Array<number>;
    public cellTypeModel: Array<number>;
    public methodModel: Array<number>;
    public subMethodModel: Array<number>;
    public neurotransmitterModel: Array<number>;

    yearFromSearchModel: any;

    fileTypeModel: any;

    panelOpenState: boolean;

    // Definiing List Variables
    repList: any;

    repShowList: any;

    paperTypeList: any;
    taskList: any;
    specieList: any;
    sexList: any;
    strainList: any;
    genoList: any;
    ageList: any;
    fileTypeList: any;
    public speciesList: any;
    public genosList: any;
    public diseaseList: any;
    public subModelList: any;
    public regionList: any;
    public subRegionList: any;
    public cellTypeList: any;
    public methodList: any;
    public subMethodList: any;
    public neurotransmitterList: any;

    authorList: any;
    authorList2: any;
    piList: any;
    searchResultList: any;
    yearList: any;
    paperInfoFromDoiList: any;
    checkYear: boolean;

    isAdmin: boolean;
    isUser: boolean;
    isFullDataAccess: boolean;

    _cogbytesSearch: CogbytesSearch;
    isSearch: boolean;
    filteredSearchList: any;

    //yearFrom = new UntypedFormControl('', []);
    yearTo: UntypedFormControl;

    public repMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredRepList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public authorMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public piMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredPIList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public diseaseMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredDiseaseList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public subModelMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredSubModelList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public regionMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public subRegionMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredSubRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public cellTypeMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredCellTypeList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public methodMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public subMethodMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredSubMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public neurotransmitterMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredNeurotransmitterList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    showAll: boolean;

    private subscription: Subscription = new Subscription();

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();


    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private cogbytesService: CogbytesService,
        private piSiteService: PISiteService,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        private spinnerService: NgxSpinnerService,
        private route: ActivatedRoute,
        private router: Router,
        private fb: UntypedFormBuilder,
        public dialogRef: MatDialog) {

        this._cogbytesSearch = {
            authorID: [], doi: '', fileTypeID: [], genoID: [], intervention: '', keywords: '',
            psID: [], repID: [], sexID: [], specieID: [], strainID: [], taskID: [], yearFrom: undefined, yearTo: undefined,
            startAge: null, endAge: null,
            diseaseID: [], subModelID: [], regionID: [], subRegionID: [], cellTypeID: [], methodID: [], subMethodID: [], transmitterID: []
        }
        this.checkYear = false;
        this.isAdmin = false;
        this.isUser = false;
        this.isFullDataAccess = false;
        this.isSearch = false;
        this.showAll = false;
        this.authorModel= [];
        this.piModel = [];
        this.titleModel = [];
        this.yearFromSearchModel = undefined;
        this.fileTypeModel = [];
        this.cognitiveTaskModel = [];
        this.specieModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.genoModel = [];
        this.diseaseModel = [];
        this.subModel = [];
        this.regionModel = [];
        this.subRegionModel = [];
        this.cellTypeModel = [];
        this.methodModel = [];
        this.subMethodModel = [];
        this.neurotransmitterModel = [];
        this.taskList = [];
        this.specieList = [];
        this.sexList = [];
        this.strainList = [];
        this.genoList = [];
        this.ageList = [];
        this.methodList = [];
        this.subMethodList = [];
        this.diseaseList = [];
        this.subModelList = [];
        this.regionList = [];
        this.subRegionList = [];
        this.cellTypeList = [];
        this.neurotransmitterList = [];
        this.doiModel = '';
        this.keywordsModel = '';
        this.interventionModel = '';
        this.panelOpenState = false;
        this.yearTo = fb.control('')
        this.route.queryParams.subscribe(params => {
            this.showAll = false;
            if (params['showall'] != null && params['showall'] == 'true') {
                this.showAll = true;
            }
            //console.log(this.showAll);
        });
    }

    ngOnInit() {

        if (this.showAll == true) {
            this.ShowRepositories();
        } else {
            this.GetRepositories();
            this.GetAuthorList();
            this.GetPIList();
            this.GetDiseaseList();
            this.GetSubModelList();
            this.GetRegionList();
            this.GetSubRegionList();
            this.GetCellTypeList();
            this.GetMethodList();
            this.GetSubMethodList();
            this.GetNeurotransmitterList();
            this.cogbytesService.getTask().subscribe((data: any) => { this.taskList = data; });
            this.cogbytesService.getSpecies().subscribe((data: any) => { this.specieList = data; });
            this.cogbytesService.getSex().subscribe((data: any) => { this.sexList = data; });
            this.cogbytesService.getStrain().subscribe((data: any) => { this.strainList = data; });
            this.cogbytesService.getGenos().subscribe((data: any) => { this.genoList = data; });
            this.cogbytesService.getFileTypes().subscribe((data: any) => { this.fileTypeList = data; });
        }



        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.yearList = this.GetYear(1970).sort().reverse();

        this.interventionModel = "All";

        this.isSearch = false;

        this.subscription.add(
            this.router.events
              .pipe(filter(event => event instanceof NavigationEnd))
              .subscribe(() => {
                this.resetState();
              })
          );

    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    resetState(): void {
        this._cogbytesSearch = {
            authorID: [], doi: '', fileTypeID: [], genoID: [], intervention: '', keywords: '',
            psID: [], repID: [], sexID: [], specieID: [], strainID: [], taskID: [], yearFrom: undefined, yearTo: undefined,
            startAge: null, endAge: null,
            diseaseID: [], subModelID: [], regionID: [], subRegionID: [], cellTypeID: [], methodID: [], subMethodID: [], transmitterID: []
        }
        this.checkYear = false;
        this.isAdmin = false;
        this.isUser = false;
        this.isFullDataAccess = false;
        this.isSearch = false;
        this.authorModel= [];
        this.piModel = [];
        this.titleModel = [];
        this.yearFromSearchModel = undefined;
        this.fileTypeModel = [];
        this.cognitiveTaskModel = [];
        this.specieModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.genoModel = [];
        this.doiModel = '';
        this.keywordsModel = '';
        this.interventionModel = '';
        this.panelOpenState = false;
        this.yearTo.setValue('');
        if (this.showAll == true) {
            this.ShowRepositories();
        } else {
            this.GetRepositories();
            this.GetAuthorList();
            this.GetPIList();
            this.cogbytesService.getTask().subscribe((data: any) => { this.taskList = data; });
            this.cogbytesService.getSpecies().subscribe((data: any) => { this.specieList = data; });
            this.cogbytesService.getSex().subscribe((data: any) => { this.sexList = data; });
            this.cogbytesService.getStrain().subscribe((data: any) => { this.strainList = data; });
            this.cogbytesService.getGenos().subscribe((data: any) => { this.genoList = data; });
            this.cogbytesService.getFileTypes().subscribe((data: any) => { this.fileTypeList = data; });
            this.GetRepositories();
            this.GetAuthorList();
            this.GetPIList();
            this.GetDiseaseList();
            this.GetSubModelList();
            this.GetRegionList();
            this.GetSubRegionList();
            this.GetCellTypeList();
            this.GetMethodList();
            this.GetSubMethodList();
            this.GetNeurotransmitterList();
        }



        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.yearList = this.GetYear(1970).sort().reverse();

        this.interventionModel = "All";

        this.isSearch = false;
    }

    // Function definition to get list of years
    GetYear(startYear: number) {
        var currentYear = new Date().getFullYear(), years = [];
        startYear = startYear || 1980;
        while (startYear <= currentYear) {
            years.push(startYear++);
        }
        return years;
    }

    GetRepositories() {

        this.cogbytesService.getAllRepositories().subscribe((data : any) => {
            this.repList = data;

            // load the initial expList
            this.filteredRepList.next(this.repList.slice());

            this.repMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterRep();
                });

        });

        return this.repList;
    }

    ShowRepositories() {

        this.cogbytesService.showAllRepositories().subscribe(data => {
            this.repShowList = data;
            //console.log(this.repShowList);

        });
    }

    // handling multi filtered Rep list
    private filterRep() {
        if (!this.repList) {
            return;
        }

        // get the search keyword
        let searchRep = this.repMultiFilterCtrl.value;

        if (!searchRep) {
            this.filteredRepList.next(this.repList.slice());
            return;
        } else {
            searchRep = searchRep.toLowerCase();
        }

        // filter the rep
        this.filteredRepList.next(
            this.repList.filter((x : any) => x.title.toLowerCase().indexOf(searchRep) > -1)
        );
    }


    GetAuthorList() {


        this.cogbytesService.getAuthor().subscribe((data: any) => {
            this.authorList = data;

            // load the initial expList
            this.filteredAutorList.next(this.authorList.slice());

            this.authorMultiFilterCtrl.valueChanges
                .pipe(
                                    debounceTime(100),
                                    filter(search => search && search.length >= 3),
                                    takeUntil(this._onDestroy)
                            )
                .subscribe(() => {
                    this.filterAuthor();
                });

        });

        return this.authorList;
    }


    GetPIList() {

        this.piSiteService.getPISite().subscribe((data: any) => {
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
            this.piList.filter((x : any) => x.piSiteName.toLowerCase().indexOf(searchPI) > -1)
        );
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
            this.authorList.filter((x: any) => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
        );
    }

    // Disease filter for multi-select search
    private filterDisease() {
        if (!this.diseaseList) {
            return;
        }
        let searchDisease = this.diseaseMultiFilterCtrl?.value;
        if (!searchDisease) {
            this.filteredDiseaseList?.next(this.diseaseList.slice());
            return;
        } else {
            searchDisease = searchDisease.toLowerCase();
        }
        this.filteredDiseaseList?.next(
            this.diseaseList.filter((x: any) => x.diseaseModel.toLowerCase().indexOf(searchDisease) > -1)
        );
    }

    // SubModel filter for multi-select search
    private filterSubModel() {
        if (!this.subModelList) {
            return;
        }
        let searchSubModel = this.subModelMultiFilterCtrl?.value;
        if (!searchSubModel) {
            this.filteredSubModelList?.next(this.subModelList.slice());
            return;
        } else {
            searchSubModel = searchSubModel.toLowerCase();
        }
        this.filteredSubModelList?.next(
            this.subModelList.filter((x: any) => x.subModel.toLowerCase().indexOf(searchSubModel) > -1)
        );
    }

    // Region filter for multi-select search
    private filterRegion() {
        if (!this.regionList) {
            return;
        }
        let searchRegion = this.regionMultiFilterCtrl?.value;
        if (!searchRegion) {
            this.filteredRegionList?.next(this.regionList.slice());
            return;
        } else {
            searchRegion = searchRegion.toLowerCase();
        }
        this.filteredRegionList?.next(
            this.regionList.filter((x: any) => x.brainRegion.toLowerCase().indexOf(searchRegion) > -1)
        );
    }

    // SubRegion filter for multi-select search
    private filterSubRegion() {
        if (!this.subRegionList) {
            return;
        }
        let searchSubRegion = this.subRegionMultiFilterCtrl?.value;
        if (!searchSubRegion) {
            this.filteredSubRegionList?.next(this.subRegionList.slice());
            return;
        } else {
            searchSubRegion = searchSubRegion.toLowerCase();
        }
        this.filteredSubRegionList?.next(
            this.subRegionList.filter((x: any) => x.subRegion.toLowerCase().indexOf(searchSubRegion) > -1)
        );
    }

    // CellType filter for multi-select search
    private filterCellType() {
        if (!this.cellTypeList) {
            return;
        }
        let searchCellType = this.cellTypeMultiFilterCtrl?.value;
        if (!searchCellType) {
            this.filteredCellTypeList?.next(this.cellTypeList.slice());
            return;
        } else {
            searchCellType = searchCellType.toLowerCase();
        }
        this.filteredCellTypeList?.next(
            this.cellTypeList.filter((x: any) => x.cellType.toLowerCase().indexOf(searchCellType) > -1)
        );
    }

    // Method filter for multi-select search
    private filterMethod() {
        if (!this.methodList) {
            return;
        }
        let searchMethod = this.methodMultiFilterCtrl?.value;
        if (!searchMethod) {
            this.filteredMethodList?.next(this.methodList.slice());
            return;
        } else {
            searchMethod = searchMethod.toLowerCase();
        }
        this.filteredMethodList?.next(
            this.methodList.filter((x: any) => x.method.toLowerCase().indexOf(searchMethod) > -1)
        );
    }

    // SubMethod filter for multi-select search
    private filterSubMethod() {
        if (!this.subMethodList) {
            return;
        }
        let searchSubMethod = this.subMethodMultiFilterCtrl?.value;
        if (!searchSubMethod) {
            this.filteredSubMethodList?.next(this.subMethodList.slice());
            return;
        } else {
            searchSubMethod = searchSubMethod.toLowerCase();
        }
        this.filteredSubMethodList?.next(
            this.subMethodList.filter((x: any) => x.subMethod.toLowerCase().indexOf(searchSubMethod) > -1)
        );
    }

    // Neurotransmitter filter for multi-select search
    private filterNeurotransmitter() {
        if (!this.neurotransmitterList) {
            return;
        }
        let searchNeuro = this.neurotransmitterMultiFilterCtrl?.value;
        if (!searchNeuro) {
            this.filteredNeurotransmitterList?.next(this.neurotransmitterList.slice());
            return;
        } else {
            searchNeuro = searchNeuro.toLowerCase();
        }
        this.filteredNeurotransmitterList?.next(
            this.neurotransmitterList.filter((x: any) => x.neuroTransmitter.toLowerCase().indexOf(searchNeuro) > -1)
        );
    }

    // Handle changes in selected disease models
    selectedModelChange(selectedModels: number[]) {
        // Implement logic to update subModelList or subSubModelList based on selectedModels if needed
        // This is a placeholder for any dynamic update logic
    }

    // Handle changes in selected methods
    selectedMethodChange(selectedMethods: number[]) {
        // Implement logic to update subMethodList or subSubMethodList based on selectedMethods if needed
        // This is a placeholder for any dynamic update logic
    }

    // Handle changes in selected regions
    selectedRegionChange(selectedRegions: number[]) {
        // Implement logic to update subRegionList or subSubRegionList based on selectedRegions if needed
        // This is a placeholder for any dynamic update logic
    }

    setDisabledValSearch() {

        if
            (
            this.authorModel.length == 0 && this.piModel.length == 0 && this.titleModel.length == 0 && this.keywordsModel == ''
            && this.doiModel == '' && this.cognitiveTaskModel.length == 0 && this.specieModel.length == 0 && this.sexModel.length == 0
            && this.strainModel.length == 0 && this.genoModel.length == 0
        ) {
            return true;
        }

        return false;

    }

    selectYearToChange(yearFromVal : number, yearToVal : number) {
        //console.log(yearToVal)
        yearFromVal = yearFromVal === null ? 0 : yearFromVal;

        yearToVal < yearFromVal ? this.yearTo.setErrors({ 'incorrect': true }) : false;

    }


    getErrorMessageYearTo() {
        //return this.yearTo.getError('Year To should be greater than Year From');
        return INVALIDYEAR;
    }


    //GetRepositories() {
    //    this.cogbytesService.getAllRepositories().subscribe(data => { this.repList = data; console.log(data); });
    //    return this.repList;
    //}

    // Function definition for searching publications based on search criteria
    search() {

        this._cogbytesSearch.authorID = this.authorModel;
        this._cogbytesSearch.psID = this.piModel;
        this._cogbytesSearch.repID = this.titleModel;
        this._cogbytesSearch.keywords = this.keywordsModel;
        this._cogbytesSearch.doi = this.doiModel;
        this._cogbytesSearch.taskID = this.cognitiveTaskModel;
        this._cogbytesSearch.specieID = this.specieModel;
        this._cogbytesSearch.sexID = this.sexModel;
        this._cogbytesSearch.strainID = this.strainModel;
        this._cogbytesSearch.genoID = this.genoModel;
        this._cogbytesSearch.startAge = this.ageStartModel;
        this._cogbytesSearch.endAge = this.ageEndModel;
        this._cogbytesSearch.yearFrom = this.yearFromSearchModel;
        this._cogbytesSearch.yearTo = (this.yearTo.value === '') ? undefined : +this.yearTo.value;
        this._cogbytesSearch.intervention = this.interventionModel;
        this._cogbytesSearch.fileTypeID = this.fileTypeModel;
        this._cogbytesSearch.diseaseID = this.diseaseModel;
        this._cogbytesSearch.subModelID = this.subModel;
        this._cogbytesSearch.regionID = this.regionModel;
        this._cogbytesSearch.subRegionID = this.subRegionModel;
        this._cogbytesSearch.cellTypeID = this.cellTypeModel;
        this._cogbytesSearch.methodID = this.methodModel;
        this._cogbytesSearch.subMethodID = this.subMethodModel;
        this._cogbytesSearch.transmitterID = this.neurotransmitterModel;


        this.cogbytesService.searchRepositories(this._cogbytesSearch).subscribe(data => {

            this.searchResultList = data;
            //console.log(this.searchResultList);
            this.isSearch = true;
        });
    }

    // Function to filter search list based on repository
    getFilteredSearchList(repID: number) {
        return this.searchResultList.filter((x: any) => x.repID == repID);
    }

    // Function for getting string of repository authors
    getRepAuthorString(rep: any) {
        let authorString: string = "";
        for (let id of rep.authourID) {
            let firstName: string = this.authorList[this.authorList.map(function (x: any) { return x.id }).indexOf(id)].firstName;
            let lastName: string = this.authorList[this.authorList.map(function (x: any) { return x.id }).indexOf(id)].lastName;
            authorString += firstName + "-" + lastName + ", ";
        }
        return authorString.slice(0, -2);
    }

    // Function for getting string of repository PIs
    getRepPIString(rep: any) {
        let PIString: string = "";
        for (let id of rep.psID) {
            PIString += this.piList[this.piList.map(function (x: any) { return x.id }).indexOf(id)].psFullName + ", ";
        }
        return PIString.slice(0, -2);
    }

    getFileType(upload: any) {
        let fileType: string = this.fileTypeList[this.fileTypeList.map(function (x: any) { return x.id }).indexOf(upload.fileTypeID)].fileType;
        return fileType;
    }

    // Function for getting string of upload tasks
    getUploadTaskString(upload: any) {
        let taskString: string = "";
        for (let id of upload.taskID) {
            taskString += this.taskList[this.taskList.map(function (x: any) { return x.id }).indexOf(id)].task + ", ";
        }
        return taskString.slice(0, -2);
    }

    // Function for getting string of upload species
    getUploadSpeciesString(upload: any) {
        let speciesString: string = "";
        for (let id of upload.specieID) {
            speciesString += this.specieList[this.specieList.map(function (x: any) { return x.id }).indexOf(id)].species + ", ";
        }
        return speciesString.slice(0, -2);
    }

    // Function for getting string of upload sexes
    getUploadSexString(upload: any) {
        let sexString: string = "";
        for (let id of upload.sexID) {
            sexString += this.sexList[this.sexList.map(function (x: any) { return x.id }).indexOf(id)].sex + ", ";
        }
        return sexString.slice(0, -2);
    }

    // Function for getting string of upload strains
    getUploadStrainString(upload: any) {
        let strainString: string = "";
        for (let id of upload.strainID) {
            strainString += this.strainList[this.strainList.map(function (x: any) { return x.id }).indexOf(id)].strain + ", ";
        }
        return strainString.slice(0, -2);
    }

    // Function for getting string of upload genotypes
    getUploadGenoString(upload: any) {
        let genoString: string = "";
        for (let id of upload.genoID) {
            genoString += this.genoList[this.genoList.map(function (x: any) { return x.id }).indexOf(id)].genotype + ", ";
        }
        return genoString.slice(0, -2);
    }

    // Function for getting string of upload ages
    getUploadAgeString(upload: any) {
        let ageString: string = "";
        for (let id of upload.ageID) {
            ageString += this.ageList[this.ageList.map(function (x: any) { return x.id }).indexOf(id)].ageInMonth + ", ";
        }
        return ageString.slice(0, -2);
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

    DownloadFile(file: any): void {

        let path = file.permanentFilePath + '\\' + file.sysFileName;
        this.cogbytesService.downloadFile(path)
            .subscribe(result => {

                // console.log('downloadresult', result);
                //let url = window.URL.createObjectURL(result);
                //window.open(url);

                var fileData = new Blob([result]);
                var csvURL = null;
                const _win = window.navigator as any
                if (_win.msSaveBlob) {
                    csvURL = _win.msSaveBlob(fileData, file.userFileName);
                } else {
                    csvURL = window.URL.createObjectURL(fileData);
                }
                var tempLink = document.createElement('a');
                tempLink.href = csvURL;
                tempLink.setAttribute('download', file.userFileName);
                document.body.appendChild(tempLink);
                tempLink.click();

            },
                error => {
                    if (error.error instanceof Error) {
                        console.log('An error occurred:', error.error.message);
                    } else {
                        console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                    }
                });
    }

    getLinkURL(rep : any) {
        return "http://localhost:4200/comp-edit?repolinkguid=" + rep.repoLinkGuid;
    }
}


