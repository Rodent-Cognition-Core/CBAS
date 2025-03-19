import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormControl, FormBuilder } from '@angular/forms';
import { ReplaySubject ,  Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CogbytesService } from '../services/cogbytes.service'
import { CogbytesSearch } from '../models/cogbytesSearch'
import { AuthenticationService } from '../services/authentication.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ActivatedRoute } from '@angular/router';
import { INVALIDYEAR } from '../shared/messages';


@Component({
    selector: 'app-cogbytesSearch',
    templateUrl: './cogbytesSearch.component.html',
    styleUrls: ['./cogbytesSearch.component.scss']
})
export class CogbytesSearchComponent implements OnInit {

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
    ageModel: any;
    interventionModel: any;

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

    //yearFrom = new FormControl('', []);
    yearTo: FormControl;

    public repMultiFilterCtrl: FormControl = new FormControl();
    public filteredRepList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public piMultiFilterCtrl: FormControl = new FormControl();
    public filteredPIList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    showAll: boolean;

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();


    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private cogbytesService: CogbytesService,
        public dialogAuthor: MatDialog,
        private spinnerService: NgxSpinnerService,
        private route: ActivatedRoute,
        private fb: FormBuilder,
        public dialogRef: MatDialog) {

        this._cogbytesSearch = {
            ageID: [], authorID: [], doi: '', fileTypeID: [], genoID: [], intervention: '', keywords: '',
            piID: [], repID: [], sexID: [], specieID: [], strainID: [], taskID: [], yearFrom: 0, yearTo: 0
        }
        this.checkYear = false;
        this.isAdmin = false;
        this.isUser = false;
        this.isFullDataAccess = false;
        this.isSearch = false;
        this.showAll = false;
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
            this.cogbytesService.getFileTypes().subscribe((data: any) => { this.fileTypeList = data; });
            this.cogbytesService.getTask().subscribe((data: any) => { this.taskList = data; });
            this.cogbytesService.getSpecies().subscribe((data: any) => { this.specieList = data; });
            this.cogbytesService.getSex().subscribe((data: any) => { this.sexList = data; });
            this.cogbytesService.getStrain().subscribe((data: any) => { this.strainList = data; });
            this.cogbytesService.getGenos().subscribe((data: any) => { this.genoList = data; });
            this.cogbytesService.getAges().subscribe((data: any) => { this.ageList = data; });
        }



        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.yearList = this.GetYear(1970).sort().reverse();

        this.interventionModel = "All";

        this.isSearch = false;



    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
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
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterAuthor();
                });

        });

        return this.authorList;
    }


    GetPIList() {

        this.cogbytesService.getPI().subscribe((data: any) => {
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
            this.piList.filter((x: any) => x.piFullName.toLowerCase().indexOf(searchPI) > -1)
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

    setDisabledValSearch() {

        if
            (
            this.authorModel == null && this.piModel == null && this.titleModel == null && this.keywordsModel == ''
            && this.doiModel == '' && this.cognitiveTaskModel == null && this.specieModel == null && this.sexModel == null
            && this.strainModel == null && this.genoModel == null && this.ageModel == null
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
        this._cogbytesSearch.piID = this.piModel;
        this._cogbytesSearch.repID = this.titleModel;
        this._cogbytesSearch.keywords = this.keywordsModel;
        this._cogbytesSearch.doi = this.doiModel;
        //this._pubSCreenSearch.year = this.yearModel;
        //this._pubSCreenSearch.years = this.yearSearchModel;
        this._cogbytesSearch.taskID = this.cognitiveTaskModel;
        this._cogbytesSearch.specieID = this.specieModel;
        this._cogbytesSearch.sexID = this.sexModel;
        this._cogbytesSearch.strainID = this.strainModel;
        this._cogbytesSearch.genoID = this.genoModel;
        this._cogbytesSearch.ageID = this.ageModel;

        this._cogbytesSearch.yearFrom = this.yearFromSearchModel;
        this._cogbytesSearch.yearTo = (this.yearTo.value === '') ? undefined : +this.yearTo.value

        this._cogbytesSearch.intervention = this.interventionModel;

        this._cogbytesSearch.fileTypeID = this.fileTypeModel;

        console.log(this._cogbytesSearch);

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
        for (let id of rep.piid) {
            PIString += this.piList[this.piList.map(function (x: any) { return x.id }).indexOf(id)].piFullName + ", ";
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


