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
import { DOINOTVALID, FIELDISREQUIRED, PUBLICATIONEDITFAILED, PUBLICATIONEDITSUCCESSFULL, PUBLICATIONSUCESSFULLYADDED, PUBLICATIONWITHSAMEDOI, PUBMEDKEYNOTVALID, YEARNOTVALID } from '../shared/messages';

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
    subModel: any;
    regionModel: any;
    subRegionModel: any;
    cellTypeModel: any;
    addingOptionModel: any;
    methodModel: any;
    subMethodModel: any;
    neurotransmitterModel: any;
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
    doiKeyModel: any;
    PubMedKeyModel: any;
    subTaskModel: any;
    authorModel2: any;
    //paperTypeModel2: any;
    referenceModel: any;
    sourceOptionModel: any;
    bioAddingOptionModel: any;
    bioDoiKeyModel: any;
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
    

    private form: FormGroup;



    //onbj variable from Models
    _pubscreen = new Pubscreen();
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
        this.pubScreenService.getTask().subscribe(data => { this.taskList = data; this.processList(this.taskList, "None", "task"); });
        this.pubScreenService.getSpecie().subscribe(data => { this.specieList = data; this.processList(this.specieList, "Other", "species"); });
        this.pubScreenService.getSex().subscribe(data => { this.sexList = data; });
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

            this.pubScreenService.getPaperInfo(this.publicationId).subscribe(data => {
                this.paperInfo = data;
                //console.log(this.paperInfo)
                this.doiModel = this.paperInfo.doi;
                this.keywordsModel = this.paperInfo.keywords;
                this.titleModel = this.paperInfo.title;
                this.abstractModel = this.paperInfo.abstract;
                this.yearModel = this.paperInfo.year;
                this.referenceModel = this.paperInfo.reference;

                this.authorModel = this.paperInfo.authourID;
                this.cellTypeModel = this.paperInfo.cellTypeID;
                this.diseaseModel = this.paperInfo.diseaseID;
                this.subModel = this.paperInfo.subModelID;
                this.methodModel = this.paperInfo.methodID;
                this.subMethodModel = this.paperInfo.subMethodID;
                this.paperTypeModel = this.paperInfo.paperTypeID;
                this.regionModel = this.paperInfo.regionID;
                this.sexModel = this.paperInfo.sexID;
                this.specieModel = this.paperInfo.specieID;
                this.strainModel = this.paperInfo.strainID;

                this.subRegionModel = this.paperInfo.subRegionID;
                //this.pubScreenService.getRegionSubRegion().subscribe(dataSubRegion => {
                //    this.selectedRegionChange(this.regionModel)
                //    this.subRegionModel = this.paperInfo.subRegionID;
                //});

                this.cognitiveTaskModel = this.paperInfo.taskID;
                this.subTaskModel = this.paperInfo.subTaskID;
                //this.pubScreenService.getTaskSubTask().subscribe(dataSubTask => {
                //    this.selectedTaskChange(this.cognitiveTaskModel)
                //    this.subTaskModel = this.paperInfo.subTaskID;
                //});
                this.subSubTaskList = this.subTaskList.filter(x => this.cognitiveTaskModel.includes(x.taskID));
                this.filteredSubTaskList.next(this.subSubTaskList.slice());
                this.subStrainList = this.strainList.filter(x => this.specieModel.includes(x.speciesID));
                this.filteredStrainList.next(this.subStrainList.slice());
                this.subSubModelList = this.subModelList.filter(x => this.diseaseModel.includes(x.modelID));
                this.filteredSubModelList.next(this.subSubModelList.slice());
                this.subSubMethodList = this.subMethodList.filter(x => this.methodModel.includes(x.methodID));
                this.filteredSubMethodList.next(this.subSubMethodList.slice());
                this.subSubRegionList = this.subRegionList.filter(x => this.regionModel.includes(x.rid));
                this.filteredSubRegionList.next(this.subSubRegionList.slice());

                this.neurotransmitterModel = this.paperInfo.transmitterID;

                this.sourceOptionModel = 3;

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

    // Getting list of all years  in database ???
    getAllYears() {
    return this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; /*console.log(this.yearList); */ });
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


    isOtherStrainMouse() {
        if (this.strainList != undefined) {
        
            let otherIndex = this.strainList.find(x => x.strain == "Other (mouse)").id;
            if (this.strainModel.includes(otherIndex)) {
                return true;
            }
        }
        this.strainOtherMouseModel = "";
        return false;
    }

    isOtherStrainRat() {
        if (this.strainList != undefined) {

            let otherIndex = this.strainList.find(x => x.strain == "Other (rat)").id;
            if (this.strainModel.includes(otherIndex)) {
                return true;
            }
        }
        this.strainOtherRatModel = "";
        return false;
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
    subTask = new FormControl('', [Validators.required]);

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
                alert(DOINOTVALID);

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.keywordsModel = data.result.keywords;
                this.doiModel = data.result.doi;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                

            }

        });

    }

    // Adding pubmed key to get paper information from pubMed
    addPubMedID(PubMedKey) {

        this.pubScreenService.getPaparInfoFromPubmedKey(PubMedKey).subscribe(data => {

            //console.log(data);
            //console.log(data.result);

            if (data.result == null) {
                alert(PUBMEDKEYNOTVALID);

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.keywordsModel = data.result.keywords;
                this.doiModel = data.result.doi;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
               
            }

        });

    }

    addDOIBio(doi) {

        this.pubScreenService.getPaparInfoFromDOIBio(doi).subscribe(data => {

            //console.log(data);
            //console.log(data.result);

            if (data.result == null) {
                alert(DOINOTVALID);

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.keywordsModel = data.result.keywords;
                this.doiModel = data.result.doi;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;
                

            }

        });

    }

    addDOICrossref(doi) {

        this.pubScreenService.getPaparInfoFromDOICrossref(doi).subscribe(data => {

            //console.log(data);
            //console.log(data.result);

            if (data.result == null) {
                alert(DOINOTVALID);

            }
            else {

                this.authorModel2 = data.result.authorString;
                this.titleModel = data.result.title;
                this.abstractModel = data.result.abstract;
                this.yearModel = data.result.year;
                this.doiModel = data.result.doi;
                this.referenceModel = data.result.reference;
                this.authorList2 = data.result.author;


            }

        });

    }

    // Adding a new publication to DB by cliking on Submit button
    AddEditPublication() {

        this.spinnerService.show();

        if (!this.yearModel.match(/^(19|20)\d{2}$/)) {
            alert(YEARNOTVALID);
            return;
        }

        if (this.authorModel !== null && this.authorModel.length !== 0) {
            this._pubscreen.authourID = this.authorModel;
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

        this._pubscreen.title = this.titleModel;
        this._pubscreen.abstract = this.abstractModel;
        this._pubscreen.keywords = this.keywordsModel;
        this._pubscreen.doi = this.doiModel;
        this._pubscreen.year = this.yearModel;
        this._pubscreen.paperTypeID = this.paperTypeModel;
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

        this.titleModel = '';
        this.abstractModel = '';
        this.keywordsModel = '';
        this.doiModel = '';
        this.yearModel = '';
        this.yearSearchModel = [];
        this.authorModel = [];
        this.paperTypeModel = '';
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
        this.doiKeyModel = '';
        this.authorModel2 = '';
        this.paperTypeModel = '';
        this.referenceModel = '';
        this.PubMedKeyModel = '';
        this.bioDoiKeyModel = '';

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

    processList(data, item, propertyName) {

        const ret = data.filter(row => (row[propertyName] === item));
        if (ret.length > 0) {
            data.splice(data.findIndex(row => (row[propertyName] === item)), 1);
            data.push(ret[0])
        }
        
        return data
    }
    
}
