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

    allRepositories: any[] = [];
    filteredRepositories: any[] = [];

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
    }

    ngOnInit() {
        this.cogbytesService.getAllRepositories().subscribe((data: any) => {
            this.allRepositories = data;
            console.log(this.allRepositories);
            this.filteredRepositories = [...this.allRepositories];
        });
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
        // ...existing code...

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.yearList = this.GetYear(1970).sort().reverse();

        this.interventionModel = "All";

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

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.yearList = this.GetYear(1970).sort().reverse();

        this.interventionModel = "All";
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

    // Remove ShowRepositories and GetRepositories logic related to showAll/isSearch

    // Update search to filter in-memory
    search() {
        this.filterRepositories();
    }

    filterRepositories() {
        this.filteredRepositories = this.allRepositories.filter(rep => {
            let matches = true;
            if (this.titleModel && this.titleModel.length > 0) {
                matches = matches && rep.title && rep.title.toLowerCase().includes(this.titleModel.toLowerCase());
            }
            if (this.authorModel && this.authorModel.length > 0) {
                matches = matches && this.authorModel.every((authorId: any) => rep.authourID && rep.authourID.includes(authorId));
            }
            // Add more filter conditions for other fields as needed
            return matches;
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

    private filterDisease() {
        if (!this.diseaseList) {
            return;
        }
        let searchDisease = this.diseaseMultiFilterCtrl.value;
        if (!searchDisease) {
            this.filteredDiseaseList.next(this.diseaseList.slice());
            return;
        } else {
            searchDisease = searchDisease.toLowerCase();
        }
        this.filteredDiseaseList.next(
            this.diseaseList.filter((x: any) => x.diseaseName && x.diseaseName.toLowerCase().indexOf(searchDisease) > -1)
        );
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

    // SubModel filter for multi-select search
    private filterSubModel() {
        if (!this.subModelList) {
            return;
        }
        let searchSubModel = this.subModelMultiFilterCtrl.value;
        if (!searchSubModel) {
            this.filteredSubModelList.next(this.subModelList.slice());
            return;
        } else {
            searchSubModel = searchSubModel.toLowerCase();
        }
        this.filteredSubModelList.next(
            this.subModelList.filter((x: any) => x.subModelName && x.subModelName.toLowerCase().indexOf(searchSubModel) > -1)
        );
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
    private filterRegion() {
        if (!this.regionList) {
            return;
        }
        let searchRegion = this.regionMultiFilterCtrl.value;
        if (!searchRegion) {
            this.filteredRegionList.next(this.regionList.slice());
            return;
        } else {
            searchRegion = searchRegion.toLowerCase();
        }
        this.filteredRegionList.next(
            this.regionList.filter((x: any) => x.regionName && x.regionName.toLowerCase().indexOf(searchRegion) > -1)
        );
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

    private filterSubRegion() {
        if (!this.subRegionList) {
            return;
        }
        let searchSubRegion = this.subRegionMultiFilterCtrl.value;
        if (!searchSubRegion) {
            this.filteredSubRegionList.next(this.subRegionList.slice());
            return;
        } else {
            searchSubRegion = searchSubRegion.toLowerCase();
        }
        this.filteredSubRegionList.next(
            this.subRegionList.filter((x: any) => x.subRegionName && x.subRegionName.toLowerCase().indexOf(searchSubRegion) > -1)
        );
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
    private filterCellType() {
        if (!this.cellTypeList) {
            return;
        }
        let searchCellType = this.cellTypeMultiFilterCtrl.value;
        if (!searchCellType) {
            this.filteredCellTypeList.next(this.cellTypeList.slice());
            return;
        } else {
            searchCellType = searchCellType.toLowerCase();
        }
        this.filteredCellTypeList.next(
            this.cellTypeList.filter((x: any) => x.cellTypeName && x.cellTypeName.toLowerCase().indexOf(searchCellType) > -1)
        );
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

    private filterMethod() {
        if (!this.methodList) {
            return;
        }
        let searchMethod = this.methodMultiFilterCtrl.value;
        if (!searchMethod) {
            this.filteredMethodList.next(this.methodList.slice());
            return;
        } else {
            searchMethod = searchMethod.toLowerCase();
        }
        this.filteredMethodList.next(
            this.methodList.filter((x: any) => x.methodName && x.methodName.toLowerCase().indexOf(searchMethod) > -1)
        );
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

    private filterSubMethod() {
        if (!this.subMethodList) {
            return;
        }
        let searchSubMethod = this.subMethodMultiFilterCtrl.value;
        if (!searchSubMethod) {
            this.filteredSubMethodList.next(this.subMethodList.slice());
            return;
        } else {
            searchSubMethod = searchSubMethod.toLowerCase();
        }
        this.filteredSubMethodList.next(
            this.subMethodList.filter((x: any) => x.subMethodName && x.subMethodName.toLowerCase().indexOf(searchSubMethod) > -1)
        );
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

    private filterNeurotransmitter() {
        if (!this.neurotransmitterList) {
            return;
        }
        let searchNeurotransmitter = this.neurotransmitterMultiFilterCtrl.value;
        if (!searchNeurotransmitter) {
            this.filteredNeurotransmitterList.next(this.neurotransmitterList.slice());
            return;
        } else {
            searchNeurotransmitter = searchNeurotransmitter.toLowerCase();
        }
        this.filteredNeurotransmitterList.next(
            this.neurotransmitterList.filter((x: any) => x.neurotransmitterName && x.neurotransmitterName.toLowerCase().indexOf(searchNeurotransmitter) > -1)
        );
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

    GetRepositories() {
        this.cogbytesService.getAllRepositories().subscribe((data: any) => {
            this.repList = data;
            this.filteredRepList.next(this.repList.slice());
            this.repMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterRep();
                });
        });
        return this.repList;
    }

    // handling multi filtered Rep list
    private filterRep() {
        if (!this.repList) {
            return;
        }
        let searchRep = this.repMultiFilterCtrl.value;
        if (!searchRep) {
            this.filteredRepList.next(this.repList.slice());
            return;
        } else {
            searchRep = searchRep.toLowerCase();
        }
        this.filteredRepList.next(
            this.repList.filter((x: any) => x.title.toLowerCase().indexOf(searchRep) > -1)
        );
    }

    GetAuthorList() {
        this.cogbytesService.getAuthor().subscribe((data: any) => {
            this.authorList = data;
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

    private filterAuthor() {
        if (!this.authorList) {
            return;
        }
        let searchAuthor = this.authorMultiFilterCtrl.value;
        if (!searchAuthor) {
            this.filteredAutorList.next(this.authorList.slice());
            return;
        } else {
            searchAuthor = searchAuthor.toLowerCase();
        }
        this.filteredAutorList.next(
            this.authorList.filter((x: any) => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
        );
    }

    GetPIList() {
        this.piSiteService.getPISite().subscribe((data: any) => {
            this.piList = data;
            this.filteredPIList.next(this.piList.slice());
            this.piMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterPI();
                });
        });
        return this.piList;
    }

    private filterPI() {
        if (!this.piList) {
            return;
        }
        let searchPI = this.piMultiFilterCtrl.value;
        if (!searchPI) {
            this.filteredPIList.next(this.piList.slice());
            return;
        } else {
            searchPI = searchPI.toLowerCase();
        }
        this.filteredPIList.next(
            this.piList.filter((x: any) => x.piSiteName.toLowerCase().indexOf(searchPI) > -1)
        );
    }

    selectYearToChange(yearFromVal: number, yearToVal: number) {
        // Ensure yearTo is not less than yearFrom
        if (yearToVal < yearFromVal) {
            this.yearTo.setErrors({ 'incorrect': true });
        } else {
            this.yearTo.setErrors(null);
        }
    }
}


