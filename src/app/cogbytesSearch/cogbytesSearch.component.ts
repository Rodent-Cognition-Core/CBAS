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
            this.allRepositories = this.filterReposByAccess(data);
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
        // Defensive: if no repositories loaded, return empty
        if (!Array.isArray(this.allRepositories)) {
            this.filteredRepositories = [];
            return;
        }

        const titleFilter = this.titleModel && this.titleModel.length > 0 ? (Array.isArray(this.titleModel) ? this.titleModel : [this.titleModel]) : [];
        const authorFilter = Array.isArray(this.authorModel) && this.authorModel.length > 0 ? this.authorModel : [];
        const piFilter = Array.isArray(this.piModel) && this.piModel.length > 0 ? this.piModel : [];
        const fileTypeFilter = Array.isArray(this.fileTypeModel) && this.fileTypeModel.length > 0 ? this.fileTypeModel : [];
        const taskFilter = Array.isArray(this.cognitiveTaskModel) && this.cognitiveTaskModel.length > 0 ? this.cognitiveTaskModel : [];
        const specieFilter = Array.isArray(this.specieModel) && this.specieModel.length > 0 ? this.specieModel : [];
        const sexFilter = Array.isArray(this.sexModel) && this.sexModel.length > 0 ? this.sexModel : [];
        const strainFilter = Array.isArray(this.strainModel) && this.strainModel.length > 0 ? this.strainModel : [];
        const genoFilter = Array.isArray(this.genoModel) && this.genoModel.length > 0 ? this.genoModel : [];
        const diseaseFilter = Array.isArray(this.diseaseModel) && this.diseaseModel.length > 0 ? this.diseaseModel : [];
        const subModelFilter = Array.isArray(this.subModel) && this.subModel.length > 0 ? this.subModel : [];
        const regionFilter = Array.isArray(this.regionModel) && this.regionModel.length > 0 ? this.regionModel : [];
        const subRegionFilter = Array.isArray(this.subRegionModel) && this.subRegionModel.length > 0 ? this.subRegionModel : [];
        const cellTypeFilter = Array.isArray(this.cellTypeModel) && this.cellTypeModel.length > 0 ? this.cellTypeModel : [];
        const methodFilter = Array.isArray(this.methodModel) && this.methodModel.length > 0 ? this.methodModel : [];
        const subMethodFilter = Array.isArray(this.subMethodModel) && this.subMethodModel.length > 0 ? this.subMethodModel : [];
        const neurotransmitterFilter = Array.isArray(this.neurotransmitterModel) && this.neurotransmitterModel.length > 0 ? this.neurotransmitterModel : [];

        const yearFrom = this.yearFromSearchModel ? Number(this.yearFromSearchModel) : null;
        const yearTo = this.yearTo && this.yearTo.value ? Number(this.yearTo.value) : null;
        const doiFilter = this.doiModel && this.doiModel.toString().trim() !== '' ? this.doiModel.toString().toLowerCase() : null;
        const keywordsFilter = this.keywordsModel && this.keywordsModel.toString().trim() !== '' ? this.keywordsModel.toString().toLowerCase() : null;
        const interventionOpt = this.interventionModel || 'All';

        this.filteredRepositories = this.allRepositories.filter(rep => {
            // For each repo, apply multiple filters. Use defensive getters for renamed/missing properties.
            let ok = true;

            // Title(s)
            if (titleFilter.length > 0) {
                const repoTitle = (rep.title || '').toString().toLowerCase();
                ok = ok && titleFilter.some((t: any) => repoTitle.includes(t.toString().toLowerCase()));
            }

            // Authors (repo.authourID expected to be array of author ids)
            if (authorFilter.length > 0) {
                const repoAuthors = this.safeArray(rep, 'authourID');
                ok = ok && authorFilter.every((aid: any) => repoAuthors.indexOf(aid) !== -1);
            }

            // PIs
            if (piFilter.length > 0) {
                const repoPIs = this.safeArray(rep, 'psid') || this.safeArray(rep, 'psID');
                ok = ok && piFilter.every((pid: any) => repoPIs.indexOf(pid) !== -1);
            }

            // DOI
            if (doiFilter) {
                const repoDoi = (rep.doi || '').toString().toLowerCase();
                ok = ok && repoDoi.includes(doiFilter);
            }

            // Keywords
            if (keywordsFilter) {
                const repoKeywords = (rep.keywords || '').toString().toLowerCase();
                ok = ok && repoKeywords.includes(keywordsFilter);
            }

            // Year range (repo.date expected to be a string like 'YYYY-MM-DD...')
            if (yearFrom || yearTo) {
                const repoYear = this.getYearFromRepo(rep);
                if (repoYear != null) {
                    if (yearFrom && repoYear < yearFrom) ok = false;
                    if (yearTo && repoYear > yearTo) ok = false;
                }
            }

            // Intervention: repo-level metadata may include 'hasIntervention' boolean or check uploads/subexperiments
            if (interventionOpt && interventionOpt !== 'All') {
                const wantOnly = interventionOpt === 'Only';
                const wantNone = interventionOpt === 'No';
                const repoHasIntervention = this.repoHasIntervention(rep);
                if (wantOnly && !repoHasIntervention) ok = false;
                if (wantNone && repoHasIntervention) ok = false;
            }

            // Multi-select filters that were previously on uploads/datasets may now be stored at repository level
            // For each, check if repository has matching arrays; if not, try to infer from rep.experiment or rep.uploads
            if (fileTypeFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['fileTypeIDs', 'fileTypeID', 'fileTypeIds', 'fileTypes']);
                if (vals.length === 0) {
                    ok = ok && false;
                } else {
                    ok = ok && fileTypeFilter.every((f: any) => vals.indexOf(f) !== -1);
                }
            }

            if (taskFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['taskID', 'taskIDs', 'taskIds', 'taskid', 'task']);
                if (vals.length === 0) ok = ok && false; else ok = ok && taskFilter.every((t: any) => vals.indexOf(t) !== -1);
            }

            if (specieFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['specieID', 'specieIDs', 'specieIds', 'specie']);
                if (vals.length === 0) ok = ok && false; else ok = ok && specieFilter.every((s: any) => vals.indexOf(s) !== -1);
            }

            if (sexFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['sexID', 'sexIDs', 'sexIds', 'sex']);
                if (vals.length === 0) ok = ok && false; else ok = ok && sexFilter.every((s: any) => vals.indexOf(s) !== -1);
            }

            if (strainFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['strainID', 'strainIDs', 'strainIds', 'strain']);
                if (vals.length === 0) ok = ok && false; else ok = ok && strainFilter.every((s: any) => vals.indexOf(s) !== -1);
            }

            if (genoFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['genoID', 'genoIDs', 'genoIds', 'geno']);
                if (vals.length === 0) ok = ok && false; else ok = ok && genoFilter.every((g: any) => vals.indexOf(g) !== -1);
            }

            if (diseaseFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['diseaseID', 'diseaseIDs', 'diseaseIds', 'disease']);
                if (vals.length === 0) ok = ok && false; else ok = ok && diseaseFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (subModelFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['subModelID', 'subModelIDs', 'subModelIds', 'subModel']);
                if (vals.length === 0) ok = ok && false; else ok = ok && subModelFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (regionFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['regionID', 'regionIDs', 'regionIds', 'region']);
                if (vals.length === 0) ok = ok && false; else ok = ok && regionFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (subRegionFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['subRegionID', 'subRegionIDs', 'subRegionIds', 'subRegion']);
                if (vals.length === 0) ok = ok && false; else ok = ok && subRegionFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (cellTypeFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['cellTypeID', 'cellTypeIDs', 'cellTypeIds', 'cellType']);
                if (vals.length === 0) ok = ok && false; else ok = ok && cellTypeFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (methodFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['methodID', 'methodIDs', 'methodIds', 'method']);
                if (vals.length === 0) ok = ok && false; else ok = ok && methodFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (subMethodFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['subMethodID', 'subMethodIDs', 'subMethodIds', 'subMethod']);
                if (vals.length === 0) ok = ok && false; else ok = ok && subMethodFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            if (neurotransmitterFilter.length > 0) {
                const vals = this.combineRepoArrays(rep, ['transmitterID', 'transmitterIDs', 'transmitterIds', 'neurotransmitterID', 'neurotransmitterIDs']);
                if (vals.length === 0) ok = ok && false; else ok = ok && neurotransmitterFilter.every((d: any) => vals.indexOf(d) !== -1);
            }

            return ok;
        });
    }

    // Helper: safely return an array property from repo or empty array
    private safeArray(obj: any, prop: string): any[] {
        if (!obj) return [];
        if (Array.isArray(obj[prop])) return obj[prop];
        // support comma-separated string of ids
        if (typeof obj[prop] === 'string' && obj[prop].trim() !== '') {
            return obj[prop].split(',').map((s: string) => s.trim());
        }
        return [];
    }

    // Helper: combine several possible property names into one array
    private combineRepoArrays(obj: any, propCandidates: string[]): any[] {
        for (const p of propCandidates) {
            const arr = this.safeArray(obj, p);
            if (arr && arr.length > 0) return arr.map((v: any) => (typeof v === 'string' ? v : v));
        }

        // try to infer from nested structures: uploads, experiment, subexperiment
        if (Array.isArray(obj.uploads) && obj.uploads.length > 0) {
            // collect unique values from uploads for common properties
            const collected: any[] = [];
            for (const u of obj.uploads) {
                for (const p of propCandidates) {
                    if (u[p]) {
                        if (Array.isArray(u[p])) collected.push(...u[p]); else collected.push(u[p]);
                    }
                }
            }
            return Array.from(new Set(collected));
        }

        if (Array.isArray(obj.experiment) && obj.experiment.length > 0) {
            const collected: any[] = [];
            for (const e of obj.experiment) {
                for (const p of propCandidates) {
                    if (e[p]) {
                        if (Array.isArray(e[p])) collected.push(...e[p]); else collected.push(e[p]);
                    }
                }
                // try subexperiment inside experiment
                if (Array.isArray(e.subexperiment)) {
                    for (const se of e.subexperiment) {
                        for (const p of propCandidates) {
                            if (se[p]) {
                                if (Array.isArray(se[p])) collected.push(...se[p]); else collected.push(se[p]);
                            }
                        }
                    }
                }
            }
            return Array.from(new Set(collected));
        }

        return [];
    }

    // Helper: extract a year number from repo date or date-like fields
    private getYearFromRepo(rep: any): number | null {
        if (!rep) return null;
        const dateStr = rep.date || rep.Date || rep.createdDate || rep.created || '';
        if (!dateStr) return null;
        // try to parse leading 4-digit year
        const m = dateStr.toString().match(/(\d{4})/);
        if (m) return Number(m[1]);
        return null;
    }

    // Helper: determine if repository has any intervention flagged in experiments/uploads
    private repoHasIntervention(rep: any): boolean {
        if (!rep) return false;
        if (typeof rep.hasIntervention === 'boolean') return rep.hasIntervention;
        if (typeof rep.isIntervention === 'boolean') return rep.isIntervention;
        // search uploads
        if (Array.isArray(rep.uploads)) {
            for (const u of rep.uploads) {
                if (u.isIntervention === true) return true;
            }
        }
        // search experiments/subexperiments
        if (Array.isArray(rep.experiment)) {
            for (const e of rep.experiment) {
                if (Array.isArray(e.subexperiment)) {
                    for (const se of e.subexperiment) {
                        if (se.isIntervention === true) return true;
                    }
                }
                if (e.isIntervention === true) return true;
            }
        }
        return false;
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
        if (!rep || !rep.psid || !this.piList || !Array.isArray(rep.psid)) return "";
        for (let id of rep.psid) {
            // find matching PI entry (try common id property names)
            const piIndex = this.piList.findIndex((x: any) => x.id === id || x.ID === id || x.PSID === id || x.Psid === id);
            if (piIndex === -1) continue;
            const pi = this.piList[piIndex];
            // try several common name property variants
            const name = pi.pname ?? pi.PName ?? pi.PIFullName ?? pi.PISiteName ?? pi.piSiteName ?? "";
            if (name) {
                PIString += name + ", ";
            }
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

    ShowRepositories() {

        this.cogbytesService.showAllRepositories().subscribe(data => {
            this.repShowList = data;
            //console.log(this.repShowList);

        });
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

    private filterReposByAccess(repos: any[]): any[] {
        if (!Array.isArray(repos)) { return repos; }
        const isAdmin = this.authenticationService.isInRole('administrator');
        const isFull = this.authenticationService.isInRole('fulldataaccess');
        if (isAdmin || isFull) {
            return repos;
        }
        return repos.filter(r => r && r.privacyStatus === true);
    }
}


