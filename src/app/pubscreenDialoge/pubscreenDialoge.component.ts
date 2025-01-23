import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
//import { NgModel } from '@angular/forms';
//import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { PISiteService } from '../services/piSite.service';
import { NgxSpinnerService } from 'ngx-spinner';
//import { UploadService } from '../services/upload.service';
//import { SharedModule } from '../shared/shared.module';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
//import { IdentityService } from '../services/identity.service';
import { Subject ,  ReplaySubject } from 'rxjs';
import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
import { take, takeUntil } from 'rxjs/operators';
import { DOINOTVALID, FIELDISREQUIRED, PUBLICATIONEDITFAILED, PUBLICATIONEDITSUCCESSFULL, PUBLICATIONSUCESSFULLYADDED, PUBLICATIONWITHSAMEDOI, PUBMEDKEYNOTVALID, YEARNOTVALID } from '../shared/messages';

@Component({

    selector: 'app-pubscreenDialoge',
    templateUrl: './pubscreenDialoge.component.html',
    styleUrls: ['./pubscreenDialoge.component.scss'],
    providers: [TaskAnalysisService,  PISiteService]

})
export class PubscreenDialogeComponent implements OnInit {

    //Models Variables for adding Publication
    keywordsModel: any;
    cognitiveTaskModel: any;
    specieModel: any;
    sexModel: any;
    strainModel: any;
    diseaseModel: any;
    subModel: any;
    regionModel: any;
    subRegionModel: any;
    cellTypeModel: any;
    methodModel: any;
    subMethodModel: any;
    neurotransmitterModel: any;
    subTaskModel: any;
    authorModel2: any;
    //paperTypeModel2: any;
    referenceModel: any;
    taskOtherModel: string;
    specieOtherModel: string;
    strainOtherMouseModel: string;
    strainOtherRatModel: string;
    diseaseOtherModel: string;
    cellOtherModel: string;
    methodOtherModel: string;
    neurotransmitterOtherModel: string;

    isEditMode: boolean;
    publicationId: number;
    isPublicMode: boolean;

    //Models Variables for searching publication
    yearSearchModel: any

    // Definiing List Variables 
    paperTypeList: any;
    taskList: any;
    specieList: any;
    sexList: any;
    strainList: any;
    diseaseList: any;
    subModelList: any;
    //regionSubregionList: any
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
    paperInfoFromDoiList: any;
    subTaskList: any;
    subSubTaskList: any;
    //taskSubTaskList: any;
    subStrainList: any;
    subSubModelList: any;
    subSubMethodList: any;
    subSubRegionList: any;
    paperInfo: any;
    

    //private form: FormGroup;



    //onbj variable from Models
    _pubscreen: Pubscreen;
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

    author: FormControl;
    title: FormControl;
    abstract: FormControl;
    doi: FormControl;
    doiKey: FormControl;
    paperType: FormControl;
    cognitiveTask: FormControl;
    //specie = new FormControl('', [Validators.required]);
    //sex = new FormControl('', [Validators.required]);
    addingOption: FormControl;
    year: FormControl;
    pubMedKey: FormControl;
    sourceOption: FormControl;
    bioAddingOption: FormControl;
    doiKeyBio: FormControl;
    subTask: FormControl;

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(
        public thisDialogRef: MatDialogRef<PubscreenDialogeComponent>,
        // private pagerService: PagerService,
        private spinnerService: NgxSpinnerService,
        public dialog: MatDialog,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fb: FormBuilder) {

        this.authorMultiFilterCtrl = fb.control('');
        this.strainMultiFilterCtrl = fb.control('');
        this.subTaskMultiFilterCtrl = fb.control('');
        this.diseaseMultiFilterCtrl = fb.control('');
        this.subModelMultiFilterCtrl = fb.control('');
        this.regionMultiFilterCtrl = fb.control('');
        this.subRegionMultiFilterCtrl = fb.control('');
        this.cellTypeMultiFilterCtrl = fb.control('');
        this.methodMultiFilterCtrl = fb.control('');
        this.subMethodMultiFilterCtrl = fb.control('');
        this.neurotransmitterMultiFilterCtrl = fb.control('');
        this.author = fb.control('', [Validators.required]);
        this.title = fb.control('', [Validators.required]);
        this.abstract = fb.control('', [Validators.required]);
        this.doi = fb.control('', [Validators.required]);
        this.doiKey = fb.control('', [Validators.required]);
        this.paperType = fb.control('', [Validators.required]);
        this.cognitiveTask = fb.control('', [Validators.required]);
        this.addingOption = fb.control('', [Validators.required]);
        this.year = fb.control('', [Validators.required]);
        this.pubMedKey = fb.control('', [Validators.required]);
        this.sourceOption = fb.control('', [Validators.required]);
        this.bioAddingOption = fb.control('', [Validators.required]);
        this.doiKeyBio = fb.control('', [Validators.required]);
        this.subTask = fb.control('', [Validators.required]);
        this.taskOtherModel = '';
        this.specieOtherModel = '';
        this.strainOtherMouseModel = '';
        this.strainOtherRatModel = '';
        this.diseaseOtherModel = '';
        this.cellOtherModel = '';
        this.methodOtherModel = '';
        this.neurotransmitterOtherModel = '';
        this.isEditMode = false;
        this.publicationId = 0;
        this.isPublicMode = false;
        this._pubSCreenSearch = {
            abstract: '', author: [], authorString: '', authourID: [], cellTypeID: [], celltypeOther: '',
            diseaseID: [], diseaseOther: '', doi: '', id: 0, keywords: '', methodID: [], methodOther: '',
            neurotransOther: '', paperType: '', paperTypeID: 0, paperTypeIdSearch: [], reference: '', regionID: [],
            search: '', sexID: [], source: '', specieID: [], specieOther: '', strainID: [], strainMouseOther: '',
            strainRatOther: '', subMethodID: [], subModelID: [], subRegionID: [], subTaskID: [], taskID: [], taskOther: '',
            title: '', transmitterID: [], year: '', yearFrom: 0, yearID: [], yearTo: 0
        }
        this._pubscreen = {
            abstract: '', author: [], authorString: '', authourID: [], cellTypeID: [], celltypeOther: '',
            diseaseID: [], diseaseOther: '', doi: '', id: 0, keywords: '', methodID: [], methodOther: '',
            neurotransOther: '', paperType: '', paperTypeID: 0, paperTypeIdSearch: [], reference: '', regionID: [],
            search: '', sexID: [], source: '', specieID: [], specieOther: '', strainID: [], strainMouseOther: '',
            strainRatOther: '', subMethodID: [], subModelID: [], subRegionID: [], subTaskID: [], taskID: [], taskOther: '',
            title: '', transmitterID: [], year: '', yearFrom: 0, yearID: [], yearTo: 0
        }
        this.resetFormVals();
    }

    ngOnInit() {

        this.isEditMode = false;

        this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe((data : any) => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe((data: any) => { this.taskList = data; this.processList(this.taskList, "None", "task"); });
        this.pubScreenService.getSpecie().subscribe((data: any) => { this.specieList = data; this.processList(this.specieList, "Other", "species"); });
        this.pubScreenService.getSex().subscribe((data: any) => { this.sexList = data; });
        this.GetSubTaskList();
        this.GetStrainList();
        this.GetDiseaseList();
        this.GetSubModelList();
        this.GetRegionList();
        this.GetSubRegionList();
        this.GetCellTypeList();
        this.GetMethodList();
        this.GetSubMethodList();
        this.GetNeurotransmitterList();
        //this.pubScreenService.getStrain().subscribe(data => { this.strainList = data; this.processList(this.strainList, "Other", "strain"); });
        //this.pubScreenService.getDisease().subscribe(data => { this.diseaseList = data; this.processList(this.diseaseList, "Other", "diseaseModel"); });
        //this.pubScreenService.getSubModels().subscribe(data => { this.subModelList = data; this.processList(this.subModelList, "Other", "subModel"); });
        //this.pubScreenService.getRegion().subscribe(data => { this.regionList = data; });
        //this.pubScreenService.getCellType().subscribe(data => { this.cellTypeList = data; this.processList(this.cellTypeList, "Other", "cellType"); });
        //this.pubScreenService.getMethod().subscribe(data => { this.methodList = data; this.processList(this.methodList, "Other", "method"); });
        //this.pubScreenService.getSubMethod().subscribe(data => { this.subMethodList = data; });
        //this.pubScreenService.getNeurotransmitter().subscribe(data => { this.neurotransmitterList = data; this.processList(this.neurotransmitterList, "Other", "neuroTransmitter"); });

        //this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; console.log(this.yearList); });
        this.getAllYears();

        //console.log(this.data);
        // if it is an Edit model

        if (this.data.isPublic != null) {
            this.isPublicMode = true;
        }
        else {
            this.isPublicMode = false;
        }
        //console.log("isPublicMode: " + this.isPublicMode);
        if (this.data.publicationObj != null) {

            this.isEditMode = true;
            this.publicationId = this.data.publicationObj.id;

            this.spinnerService.show();

            this.pubScreenService.getPaperInfo(this.publicationId).subscribe((data: any) => {
                this.paperInfo = data;
                //console.log(this.paperInfo)
                this.doi.setValue(this.paperInfo.doi);
                this.keywordsModel = this.paperInfo.keywords;
                this.title.setValue(this.paperInfo.title);
                this.abstract.setValue(this.paperInfo.abstract);
                this.year.setValue(this.paperInfo.year);
                this.referenceModel = this.paperInfo.reference;

                this.author.setValue(this.paperInfo.authourID ? this.paperInfo.authourID : []);
                this.cellTypeModel = this.paperInfo.cellTypeID ? this.paperInfo.cellTypeID : [];
                this.diseaseModel = this.paperInfo.diseaseID ? this.paperInfo.diseaseID : [];
                this.subModel = this.paperInfo.subModelID ? this.paperInfo.subModelID : [];
                this.methodModel = this.paperInfo.methodID ? this.paperInfo.methodID : [];
                this.subMethodModel = this.paperInfo.subMethodID ? this.paperInfo.subMethodID : [];
                this.paperType.setValue(this.paperInfo.paperTypeID ? this.paperInfo.paperTypeID : []);
                this.regionModel = this.paperInfo.regionID ? this.paperInfo.regionID : [];
                this.sexModel = this.paperInfo.sexID ? this.paperInfo.sexID : [];
                this.specieModel = this.paperInfo.specieID ? this.paperInfo.specieID : [];
                this.strainModel = this.paperInfo.strainID ? this.paperInfo.strainID : [];

                this.subRegionModel = this.paperInfo.subRegionID ? this.paperInfo.subRegionID : [];
                //this.pubScreenService.getRegionSubRegion().subscribe(dataSubRegion => {
                //    this.selectedRegionChange(this.regionModel)
                //    this.subRegionModel = this.paperInfo.subRegionID;
                //});

                this.cognitiveTaskModel = this.paperInfo.taskID ? this.paperInfo.taskID : [];
                this.subTaskModel = this.paperInfo.subTaskID ? this.paperInfo.subTaskID : [];
                //this.pubScreenService.getTaskSubTask().subscribe(dataSubTask => {
                //    this.selectedTaskChange(this.cognitiveTaskModel)
                //    this.subTaskModel = this.paperInfo.subTaskID;
                //});
                this.subSubTaskList = this.cognitiveTaskModel && this.subTaskList ? this.subTaskList.filter((x: any) => this.cognitiveTaskModel.includes(x.taskID)) : [];
                this.filteredSubTaskList.next(this.subSubTaskList.slice());
                this.subStrainList = this.specieModel ? this.strainList.filter((x : any) => this.specieModel.includes(x.speciesID)) : [];
                this.filteredStrainList.next(this.subStrainList.slice());
                this.subSubModelList = this.diseaseModel && this.subModelList ? this.subModelList.filter((x: any) => this.diseaseModel.includes(x.modelID)) : [];
                this.filteredSubModelList.next(this.subSubModelList.slice());
                this.subSubMethodList = this.methodModel && this.subMethodList ? this.subMethodList.filter((x: any) => this.methodModel.includes(x.methodID)) : [];
                this.filteredSubMethodList.next(this.subSubMethodList.slice());
                this.subSubRegionList = this.regionModel && this.subRegionList ? this.subRegionList.filter((x: any) => this.regionModel.includes(x.rid)) : [];
                this.filteredSubRegionList.next(this.subSubRegionList.slice());

                this.neurotransmitterModel = this.paperInfo.transmitterID ? this.paperInfo.transmitterID : [];

                this.sourceOption.setValue(3);

                this.setDisabledVal();

                this.spinnerService.hide();
            });
        }

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

        //this.resetFormVals();

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

    // Getting list of all years  in database ???
    getAllYears() {
        return this.pubScreenService.getAllYears().subscribe((data: any) => { this.yearList = data; /*console.log(this.yearList); */ });
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
            this.neurotransmitterList.filter((x: any) => x.neuroTransmitter.toLowerCase().indexOf(searchNeurotransmitter) > -1)
        );
    }

    // function definition to get list of all tasks and subtasks
    //selectedTaskChange(SelectedTask) {

    //    this.pubScreenService.getTaskSubTask().subscribe(data => {
    //        this.taskSubTaskList = data;
            
    //        var filtered = this.taskSubTaskList.filter(function (item) {
    //            return SelectedTask.indexOf(item.taskID) !== -1;
    //        });

    //        //console.log(filtered);
    //        this.subTaskList = JSON.parse(JSON.stringify(filtered));
    //    });


    //}

    selectedTaskChange(SelectedTask : any) {
        this.subTaskModel = [];
        this.subSubTaskList = this.subTaskList.filter((x: any) => SelectedTask.includes(x.taskID));
        this.filteredSubTaskList.next(this.subSubTaskList.slice());
    }


    selectedSpeciesChange(SelectedSpecies : any) {
        this.strainModel = [];
        this.subStrainList = this.strainList.filter((x: any) => SelectedSpecies.includes(x.speciesID));
        this.filteredStrainList.next(this.subStrainList.slice());
    }

    //selectedModelChange(SelectedModels) {
    //    this.subModel = [];
    //    this.subSubModelList = this.subModelList.filter(x => SelectedModels.includes(x.modelID));
    //}

    //selectedMethodChange(SelectedMethods) {
    //    this.subMethodModel = [];
    //    this.subSubMethodList = this.subMethodList.filter(x => SelectedMethods.includes(x.methodID));
    //}

    //selectedRegionChange(SelectedRegion) {

    //    this.pubScreenService.getRegionSubRegion().subscribe(data => {
    //        this.regionSubregionList = data;
    //        console.log(this.regionSubregionList);
    //        var filtered = this.regionSubregionList.filter(function (item) {
    //            return SelectedRegion.indexOf(item.rid) !== -1;
    //        });

    //        //console.log(filtered);
    //        this.subRegionList = JSON.parse(JSON.stringify(filtered));
    //    });

    //    console.log(this.subRegionList);
    //}

    selectedModelChange(SelectedModels : any) {
        this.subModel = [];
        this.subSubModelList = this.subModelList.filter((x: any) => SelectedModels.includes(x.modelID));
        this.filteredSubModelList.next(this.subSubModelList.slice());
    }

    selectedMethodChange(SelectedMethods : any) {
        this.subMethodModel = [];
        this.subSubMethodList = this.subMethodList.filter((x: any) => SelectedMethods.includes(x.methodID));
        this.filteredSubMethodList.next(this.subSubMethodList.slice());
    }

    selectedRegionChange(SelectedRegion : any) {
        this.subRegionModel = [];
        this.subSubRegionList = this.subRegionList.filter((x: any) => SelectedRegion.includes(x.rid));
        this.filteredSubRegionList.next(this.subSubRegionList.slice());
    }


    isOtherStrainMouse() {
        if (this.strainList != undefined) {
        
            let otherIndex = this.strainList.find((x: any) => x.strain == "Other (mouse)").id;
            if (this.strainModel.includes(otherIndex)) {
                return true;
            }
        }
        this.strainOtherMouseModel = "";
        return false;
    }

    isOtherStrainRat() {
        if (this.strainList != undefined) {

            let otherIndex = this.strainList.find((x: any) => x.strain == "Other (rat)").id;
            if (this.strainModel.includes(otherIndex)) {
                return true;
            }
        }
        this.strainOtherRatModel = "";
        return false;
    }
    //Form Validation Variables for adding publications

    // Handling Error for the required fields
    getErrorMessageAuthor() {

        return this.author.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageTitle() {

        return this.title.hasError('required') ? FIELDISREQUIRED : '';
        
    }

    getErrorMessageYear() {
        return this.year.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageYearVal() {
        return this.year.hasError('pattern') ? 'You must enter a valid value' : '';
    }

    getErrorMessageDOI() {
        return this.doi.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessagePaperType() {

        return this.paperType.hasError('required') ? FIELDISREQUIRED : '';

    }

    getErrorMessageTask() {
        return this.cognitiveTask.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageSubTask() {
        return this.subTask.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessagePaperOption() {
        return this.addingOption.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageDOIKey() {
        return this.doiKey.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessagePubMedKey() {
        return this.pubMedKey.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessagePaperSource() {
        return this.sourceOption.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessagePaperOptionBio() {
        return this.bioAddingOption.hasError('required') ? FIELDISREQUIRED : '';
    }

    getErrorMessageDOIKeyBio() {
        return this.doiKeyBio.hasError('required') ? FIELDISREQUIRED : '';
    }

    setDisabledVal() {

        if (this.author.value === null && this.author.hasError('required')) {

            return true;
        }

        if (this.paperType.value === null && this.paperType.hasError('required')) {
            return true;
        }

        if (this.sourceOption.value === 1 && this.addingOption.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.sourceOption.value === 2 && this.bioAddingOption.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.sourceOption.value === 3 && this.author.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.addingOption.value === 1 && this.doiKey.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.addingOption.value === 2 && this.pubMedKey.hasError('required') && !this.isEditMode) {
            return true;

        }

        if (this.bioAddingOption.value === 1 && this.doiKeyBio.hasError('required') && !this.isEditMode) {
            return true;

        }

        else if (
            
            ((this.title.value === null || this.title.value === "") && this.title.hasError('required')) ||
            ((this.doi.value === null || this.doi.value === "") && this.doi.hasError('required'))||
            ((this.year.value === null || this.year.value === "") && this.year.hasError('required')) ||
            ((this.sourceOption.value === null || this.sourceOption.value === "") && this.sourceOption.hasError('required')) ||
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
    addDOI(doi : string) {
        this.pubScreenService.getPaparInfoFromDOI(doi).subscribe((data: any) => {
            if (data == null) {
                alert(DOINOTVALID);
            }
            if (data.result) {
                console.log('data.result: ' + data.result);
            }
            else {
                console.log(data);
            }
            let apiResult = 'result' in data ? data.result : data;


            if (data.apiResult == null) {
                alert(DOINOTVALID);

            }
            else {

                this.authorModel2 = apiResult.authorString;
                this.title.setValue(apiResult.title);
                this.abstract.setValue(apiResult.abstract);
                this.year.setValue(apiResult.year);
                this.keywordsModel = apiResult.keywords;
                this.doi.setValue(apiResult.doi);
                this.referenceModel = apiResult.reference;
                this.authorList2 = apiResult.author;
            }
        });
    }

    // Adding pubmed key to get paper information from pubMed
    addPubMedID(PubMedKey : string) {
        this.pubScreenService.getPaparInfoFromPubmedKey(PubMedKey).subscribe((data: any) => {
            if (data == null) {
                alert(DOINOTVALID);
            }
            let apiResult = 'result' in data ? data.result : data;
            if (apiResult == null) {
                alert(PUBMEDKEYNOTVALID);
            }
            else {

                this.authorModel2 = apiResult.authorString;
                this.title.setValue(apiResult.title);
                this.abstract.setValue(apiResult.abstract);
                this.year.setValue(apiResult.year);
                this.keywordsModel = apiResult.keywords;
                this.doi.setValue(apiResult.doi);
                this.referenceModel = apiResult.reference;
                this.authorList2 = apiResult.author;               
            }
        });
    }

    addDOIBio(doi : string) {
        this.pubScreenService.getPaparInfoFromDOIBio(doi).subscribe((data: any) => {
            if (data == null) {
                alert(DOINOTVALID);
            }
            let apiResult = 'result' in data ? data.result : data;

            if (apiResult == null) {
                alert(DOINOTVALID);
            }
            else {

                this.authorModel2 = apiResult.authorString;
                this.title.setValue(apiResult.title);
                this.abstract.setValue(apiResult.abstract);
                this.year.setValue(apiResult.year);
                this.keywordsModel = apiResult.keywords;
                this.doi.setValue(apiResult.doi);
                this.referenceModel = apiResult.reference;
                this.authorList2 = apiResult.author;               

            }
        });

    }

    addDOICrossref(doi : string) {

        this.pubScreenService.getPaparInfoFromDOICrossref(doi).subscribe((data: any) => {
            if (data == null) {
                alert(DOINOTVALID);
            }
            let apiResult = 'result' in data ? data.result : data;

            if (apiResult == null) {
                alert(DOINOTVALID);
            }
            else {

                this.authorModel2 = apiResult.authorString;
                this.title.setValue(apiResult.title);
                this.abstract.setValue(apiResult.abstract);
                this.year.setValue(apiResult.year);
                this.doi.setValue(apiResult.doi);
                this.referenceModel = apiResult.reference;
                this.authorList2 = apiResult.author;
            }

        });

    }

    // Adding a new publication to DB by cliking on Submit button
    AddEditPublication() {

        this.spinnerService.show();

        if (!this.year.value.match(/^(19|20)\d{2}$/)) {
            alert(YEARNOTVALID);
            return;
        }

        if (this.author.value !== null && this.author.value.length !== 0) {
            this._pubscreen.authourID = this.author.value;
            //console.log(this.authorModel)
        }
        else {

            this._pubscreen.author = this.authorList2;
            this._pubscreen.authorString = this.authorModel2;
            //console.log(this._pubscreen.author)
            //console.log(this.authorList2);

        }

      
        if (this.referenceModel !== null) {
            this._pubscreen.reference = this.referenceModel;
        }

        this._pubscreen.title = this.title.value;
        this._pubscreen.abstract = this.abstract.value;
        this._pubscreen.keywords = this.keywordsModel;
        this._pubscreen.doi = this.doi.value;
        this._pubscreen.year = this.year.value;
        this._pubscreen.paperTypeID = this.paperType.value;
        this._pubscreen.taskID = this.cognitiveTaskModel;
        this._pubscreen.subTaskID = this.subTaskModel;
        this._pubscreen.specieID = this.specieModel;
        this._pubscreen.sexID = this.sexModel;
        this._pubscreen.strainID = this.strainModel;
        this._pubscreen.diseaseID = this.diseaseModel;
        this._pubscreen.subModelID = this.subModel;
        this._pubscreen.regionID = this.regionModel;
        this._pubscreen.subRegionID = this.subRegionModel;
        this._pubscreen.cellTypeID = this.cellTypeModel;
        this._pubscreen.methodID = this.methodModel;
        this._pubscreen.subMethodID = this.subMethodModel;
        this._pubscreen.transmitterID = this.neurotransmitterModel;
        this._pubscreen.taskOther = this.taskOtherModel;
        this._pubscreen.specieOther = this.specieOtherModel;
        this._pubscreen.strainMouseOther = this.strainOtherMouseModel;
        this._pubscreen.strainRatOther = this.strainOtherRatModel;
        this._pubscreen.diseaseOther = this.diseaseOtherModel;
        this._pubscreen.celltypeOther = this.cellOtherModel;
        this._pubscreen.methodOther = this.methodOtherModel;
        this._pubscreen.neurotransOther = this.neurotransmitterOtherModel;

                    

        switch (this.sourceOption.value) {

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
            if (this.isPublicMode) {
                this.pubScreenService.EditPublicationPublic(this.publicationId, this._pubscreen).subscribe(data => {

                    if (data === true) {
                        alert(PUBLICATIONEDITSUCCESSFULL);
                        this.thisDialogRef.close();
                    } else {
                        alert(PUBLICATIONEDITFAILED)
                    }

                    setTimeout(() => {
                        this.spinnerService.hide();

                    }, 500);

                });
            }
            else {
                this.pubScreenService.EditPublication(this.publicationId, this._pubscreen).subscribe(data => {

                    if (data === true) {
                        alert(PUBLICATIONEDITSUCCESSFULL);
                        this.thisDialogRef.close();
                    } else {
                        alert(PUBLICATIONEDITFAILED)
                    }

                    setTimeout(() => {
                        this.spinnerService.hide();

                    }, 500);

                });
            }

        } else {
            this.pubScreenService.addPublication(this._pubscreen).subscribe(data => {

                if (data === null) {
                    alert(PUBLICATIONWITHSAMEDOI);
                } else {
                    this.thisDialogRef.close();
                    alert(PUBLICATIONSUCESSFULLYADDED);
                }
                this.resetFormVals();

                setTimeout(() => {
                    this.spinnerService.hide();

                }, 500);

            });
        }

        //this.spinnerService.hide();

    }

    resetFormVals() {

        this.title.setValue('');
        this.abstract.setValue('');
        this.keywordsModel = '';
        this.doi.setValue('');
        this.year.setValue('');
        this.yearSearchModel = [];
        this.author.setValue([]);
        this.paperType.setValue('');
        this.cognitiveTaskModel = [];
        this.subTaskModel = [];
        this.specieModel = [];
        this.sexModel = [];
        this.strainModel = [];
        this.diseaseModel = [];
        this.subModel = [];
        this.regionModel = [];
        this.subRegionModel = [];
        this.cellTypeModel = [];
        this.methodModel = [];
        this.neurotransmitterModel = [];
        this.doiKey.setValue('');
        this.authorModel2 = '';
        this.referenceModel = '';
        this.pubMedKey.setValue('');
        this.doiKeyBio.setValue('');
        this.taskOtherModel = '';
        this.specieOtherModel = '';
        this.strainOtherMouseModel = '';
        this.strainOtherRatModel = '';
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

    processList(data : any, item : any, propertyName : any) {

        const ret = data.filter((row : any) => (row[propertyName] === item));
        if (ret.length > 0) {
            data.splice(data.findIndex((row: any) => (row[propertyName] === item)), 1);
            data.push(ret[0])
        }
        
        return data
    }
    
}
