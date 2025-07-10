import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, UntypedFormBuilder } from '@angular/forms';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PISiteService } from '../services/piSite.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { CogbytesService } from '../services/cogbytes.service';
import { PubScreenService } from '../services/pubScreen.service';
import { Cogbytes } from '../models/cogbytes'
import { Subject ,  ReplaySubject } from 'rxjs';
import { CogbytesAuthorDialogueComponent } from '../cogbytesAuthorDialogue/cogbytesAuthorDialogue.component';
import { debounceTime, takeUntil, filter } from 'rxjs/operators';
import { CogbytesPIDialogeComponent } from '../cogbytesPIDialoge/cogbytesPIDialoge.component'
import { FIELDISREQUIRED, INVALIDNUMBERICALVALUE } from '../shared/messages';

@Component({

    selector: 'app-cogbytes-dialogue',
    templateUrl: './cogbytesDialogue.component.html',
    styleUrls: ['./cogbytesDialogue.component.scss'],
    providers: [TaskAnalysisService,  PISiteService]

})
export class CogbytesDialogueComponent implements OnInit {

    //Models Variables for adding Publication
    keywordsModel: any;
    doiModel: any;
    authorMultiSelect: any;
    isEditMode: boolean;
    descriptionModel: any;
    additionalNotesModel: any;
    linkModel: any;
    piMultiSelect: any;
    public speciesModel: any;
    public sexModel: any;
    public strainModel: any;
    public genotypeModel: any;
    public ageModel: any;
    public housingModel: any;
    public lightModel: any;
    public intDesModel: any;
    public imgDesModel: any;
    public taskBatteryModel: any;
    public diseaseModel: Array<number>;
    public subModel: Array<number>;
    public regionModel: Array<number>;
    public subRegionModel: Array<number>;
    public cellTypeModel: Array<number>;
    public methodModel: Array<number>;
    public subMethodModel: Array<number>;
    public neurotransmitterModel: Array<number>;

    // Definiing List Variables 
    authorList: any;
    piList: any;
    searchResultList: any;
    yearList: any;
    paperInfoFromDoiList: any;
    paperInfo: any;
    public taskList: any;
    public speciesList: any;
    public sexList: any;
    public strainList: any;
    public genosList: any;
    public ageList: any;
    public diseaseList: any;
    public subModelList: any;
    public regionList: any;
    public subRegionList: any;
    public cellTypeList: any;
    public methodList: any;
    public subMethodList: any;
    public neurotransmitterList: any;
    public subSubRegionList: any[];
    public subSubModelList: any[];
    public subSubMethodList: any[];

    repID: any;

    //private form: FormGroup;

    //Form Validation Variables for adding publications
    author: UntypedFormControl;
    pi: UntypedFormControl;
    title: UntypedFormControl;
    date: UntypedFormControl;
    privacyStatus: UntypedFormControl;
    cognitiveTask: UntypedFormControl;
    intervention: UntypedFormControl;
    numSubjects: UntypedFormControl;



    //onbj variable from Models
    _cogbytes: Cogbytes;

    public authorMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public piMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredPIList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public diseaseMultiFilterCtrl: UntypedFormControl;
    public filteredDiseaseList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subModelMultiFilterCtrl: UntypedFormControl;
    public filteredSubModelList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public regionMultiFilterCtrl: UntypedFormControl;
    public filteredRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subRegionMultiFilterCtrl: UntypedFormControl;
    public filteredSubRegionList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public cellTypeMultiFilterCtrl: UntypedFormControl;
    public filteredCellTypeList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public methodMultiFilterCtrl: UntypedFormControl;
    public filteredMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public subMethodMultiFilterCtrl: UntypedFormControl;
    public filteredSubMethodList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    public neurotransmitterMultiFilterCtrl: UntypedFormControl;
    public filteredNeurotransmitterList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);


    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(
        public thisDialogRef: MatDialogRef<CogbytesDialogueComponent>,
        // private pagerService: PagerService,
        private spinnerService: NgxSpinnerService,
        public dialog: MatDialog,
        private cogbytesService: CogbytesService,
        private piSiteService: PISiteService,
        private pubScreenService: PubScreenService,
        //private cogbytesService: CogbytesService,
        public dialogAuthor: MatDialog,
        public fb: UntypedFormBuilder,

        //private resolver: ComponentFactoryResolver,
        
        @Inject(MAT_DIALOG_DATA) public data: any,) {

        this.isEditMode = false;
        this.author = fb.control('', [Validators.required])
        this.pi = fb.control('', [Validators.required])
        this.title = fb.control('', [Validators.required])
        this.date = fb.control('', [Validators.required])
        this.privacyStatus = fb.control('', [Validators.required])
        this.cognitiveTask = fb.control('', [Validators.required]);
        this.intervention = fb.control('', [Validators.required]);
        this.numSubjects = fb.control('', [Validators.pattern('[0-9]*')]);
        this.diseaseMultiFilterCtrl = fb.control([]),
        this.subModelMultiFilterCtrl = fb.control([]),
        this.regionMultiFilterCtrl = fb.control([]),
        this.subRegionMultiFilterCtrl = fb.control([]),
        this.cellTypeMultiFilterCtrl = fb.control([]),
        this.methodMultiFilterCtrl = fb.control([]),
        this.subMethodMultiFilterCtrl = fb.control([]),
        this.neurotransmitterMultiFilterCtrl = fb.control([]),
        this.subSubRegionList = [];
        this.subSubMethodList = [];
        this.subSubModelList = [];
        this.diseaseModel = [];
        this.subModel = [];
        this.regionModel = [];
        this.subRegionModel = [];
        this.cellTypeModel = [];
        this.methodModel = [];
        this.subMethodModel = [];
        this.neurotransmitterModel = [];
        this._cogbytes = {
            additionalNotes: '', authorString: '', authourID: [], date: '', dateRepositoryCreated: '', description: '',
            doi: '', id: 0, keywords: '', link: '', piID: [], piString: '', privacyStatus: false, title: '',
            taskID: [], specieID: [], sexID: [], strainID: [], genoID: [], ageID: [], numSubjects: 0, housing: '',
            lightCycle: '', taskBattery: ''
        }

        this.cogbytesService.getTask().subscribe((data: any) => { this.taskList = data; });
        this.cogbytesService.getSpecies().subscribe((data: any) => { this.speciesList = data; });
        this.cogbytesService.getSex().subscribe((data: any) => { this.sexList = data; });
        this.cogbytesService.getStrain().subscribe((data: any) => { this.strainList = data; });
        this.cogbytesService.getGenos().subscribe((data: any) => { this.genosList = data; });
        this.cogbytesService.getAges().subscribe((data: any) => { this.ageList = data; });

        this.resetFormVals();
    }

    ngOnInit() {

        this.isEditMode = false;

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

        //console.log(this.data);

        // if it is an Edit model
        if (this.data.repObj != null) {

            this.isEditMode = true;
            this.repID = this.data.repObj.id;
            this.title.setValue(this.data.repObj.title);
            this.date.setValue(new Date(this.data.repObj.date));
            this.keywordsModel = this.data.repObj.keywords;
            this.doiModel = this.data.repObj.doi;
            this.linkModel = this.data.repObj.link;
            this.privacyStatus.setValue(this.data.repObj.privacyStatus ? "true" : "false");
            this.descriptionModel = this.data.repObj.description;
            this.additionalNotesModel = this.data.repObj.additionalNotes;
            this.author.setValue(this.data.repObj.authourID);
            this.pi.setValue(this.data.repObj.piid);
        }



    }

    // Function Definition to open a dialog for adding new Author to the system
    openDialogAuthor(): void {

        let dialogref = this.dialogAuthor.open(CogbytesAuthorDialogueComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe((_result : any) => {

            this.GetAuthorList();

        });
    }

    openDialogPI(): void { //PI Dialog Component must be implemented!

        let dialogref = this.dialogAuthor.open(CogbytesPIDialogeComponent, {
            height: '500px',
            width: '700px',
            data: {}

        });

        dialogref.afterClosed().subscribe((_result : any) => {

            this.GetPIList();
        });
    }

    //ngOnDestroy() {
    //    this._onDestroy.next();
    //    this._onDestroy.complete();
    //}

    GetAuthorList() {

        this.cogbytesService.getAuthor().subscribe((data : any) => {
            this.authorList = data;

            // load the initial AuthorList
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

    compareAuthors(author1: string, author2: string): boolean {
    return author1 && author2 ? author1 === author2 : author1 === author2;
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
            this.authorList.filter((x : any) => x.nameAffiliation.toLowerCase().indexOf(searchAuthor) > -1)
        );
    }


    GetPIList() {

        // this.cogbytesService.getPI().subscribe((data : any) => {
        //     this.piList = data;

        //     // load the initial expList
        //     this.filteredPIList.next(this.piList.slice());

        //     this.piMultiFilterCtrl.valueChanges
        //         .pipe(takeUntil(this._onDestroy))
        //         .subscribe(() => {
        //             this.filterPI();
        //         });

        // });

        this.piSiteService.getPISite().subscribe((data : any) => {
            this.piList = data

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

    // Handling Error for the required fields
    getErrorMessageAuthor() {

        return this.author.hasError('required') ? 'You must enter a value' : '';
    }

    getErrorMessagePi() {
        return this.pi.hasError('required') ? 'You must enter a value' : '';
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

    getErrorMessageTask() {
            return this.cognitiveTask.hasError('required') ? FIELDISREQUIRED : '';
        }
    
    getErrorMessageIntervention() {
        return this.intervention.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageNumSubjects() {
        return this.numSubjects.hasError('pattern') ? INVALIDNUMBERICALVALUE: '';
    }

    setDisabledVal() {
       
        if (
            this.title.hasError('required') ||
            this.author.hasError('required') ||
            this.pi.hasError('required') ||
            this.privacyStatus.hasError('required') ||
            this.date.hasError('required') ||
            ((this.title.value == null || this.title.value == "") && this.title.hasError('required'))

        ) {

            return true;
        }

        else {

            return false;
        }

    }


    AddRepository() {

        this.spinnerService.show();

        this._cogbytes.authourID = this.author.value;
        this._cogbytes.title = this.title.value;
        this._cogbytes.keywords = this.keywordsModel;
        this._cogbytes.doi = this.doiModel;
        this._cogbytes.piID = this.pi.value;
        this._cogbytes.link = this.linkModel;
        this._cogbytes.privacyStatus = this.privacyStatus.value == "true" ? true : false; 
        this._cogbytes.description = this.descriptionModel;
        this._cogbytes.additionalNotes = this.additionalNotesModel;
        this._cogbytes.date = this.date.value.toISOString().split('T')[0];

        let today = new Date();
        this._cogbytes.dateRepositoryCreated = today.toISOString().split('T')[0];

        // ADD LINK TO COGBYTES DATABASE HERE

        this.cogbytesService.addRepository(this._cogbytes).subscribe((_data : any) => {

            this.thisDialogRef.close();
            setTimeout(() => {
                this.spinnerService.hide();


            }, 500);


        });
    }

    EditRepository() {

        this.spinnerService.show();

        this._cogbytes.authourID = this.author.value;
        this._cogbytes.title = this.title.value;
        this._cogbytes.keywords = this.keywordsModel;
        this._cogbytes.doi = this.doiModel;
        this._cogbytes.piID = this.pi.value;
        this._cogbytes.link = this.linkModel;
        this._cogbytes.privacyStatus = this.privacyStatus.value == "true" ? true : false;
        this._cogbytes.description = this.descriptionModel;
        this._cogbytes.additionalNotes = this.additionalNotesModel;
        this._cogbytes.date = this.date.value.toISOString().split('T')[0];

        // let today = new Date();
        //this._cogbytes.dateRepositoryCreated = today.toISOString().split('T')[0];

        // ADD LINK TO COGBYTES DATABASE HERE

        this.cogbytesService.editRepository(this.repID, this._cogbytes).subscribe(_data => {

            this.thisDialogRef.close('Cancel');
            setTimeout(() => {
                this.spinnerService.hide();


            }, 500);

        });
    }

    resetFormVals() {

        this.title.setValue('');
        this.keywordsModel = '';
        this.doiModel = '';
        this.date.setValue('');
        this.author.setValue([]);
        this.descriptionModel = '';
        this.additionalNotesModel = '';
        this.linkModel = '';
        this.pi.setValue([]);
        this.cognitiveTask.setValue([]);
        this.speciesModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.genotypeModel = [];
        this.ageModel = [];
        this.housingModel = '';
        this.lightModel = '';
        this.intDesModel = '';
        this.imgDesModel = '';
        this.taskBatteryModel = '';
        this.intervention.setValue(null);
    }


    processList(data : any, item : any, propertyName : any) {

        const ret = data.filter((row : any) => (row[propertyName] === item));
        if (ret.length > 0) {
            data.splice(data.findIndex((row: any) => (row[propertyName] === item)), 1);
            data.push(ret[0])
        }
        
        return data
    }
    
}
