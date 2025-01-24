import { Component, OnInit, HostListener, inject } from '@angular/core';
//import { TaskAnalysisService } from '../services/taskanalysis.service';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
//import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
//import { NgModel } from '@angular/forms';
import { ReplaySubject ,  Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { DataExtractionService } from '../services/dataextraction.service'
import { DataExtraction } from '../models/dataextraction';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatTableModule, MatTableDataSource } from '@angular/material/table'
import { NgxSpinnerService } from 'ngx-spinner';
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { AuthenticationService } from '../services/authentication.service';
import { PagerService } from '../services/pager.service';
import { Geno } from '../models/geno';
import { TermsDialogeComponent } from '../termsDialoge/termsDialoge.component';
import { UploadService } from '../services/upload.service';
import { ExpDialogeService } from '../services/expdialoge.service';
import { FIELDISREQUIRED } from '../shared/messages';

declare var $: any;


@Component({
    selector: 'app-data-extraction',
    templateUrl: './data-extraction.component.html',
    styleUrls: ['./data-extraction.component.scss']
})
export class DataExtractionComponent implements OnInit {
    //private formBuilder = inject(FormBuilder);

    taskList: any;
    expList: any;
    subTakList: any;
    markerInfoList: any;
    AnimalInfoListAge: any;
    AnimalInfoListSex: any;
    AnimalInfoListGenotype: any;
    AnimalInfoListStrain: any;
    subSessionList: any[] = [];
    genoIDList: number[] = [];
    _geno: Geno;
    filteredGenoList: any[] = [];
    InterventionList: any[] = [];
    speciesList: any

    // ngModels vars
    selectedPiSiteValue: any;
    selectedAgeValue: any;
    selectedSexValue: any;
    selectedGenotypeValue: any;
    selectedStrainValue: any;
    termsChecked: boolean;
    isTrialByTrial: any;
    selectedInterventionValue: any;
    selectedSubSessionValue: any;

    // FormControls vars
    //dataextractionForm = new FormGroup({
    task: FormControl;
    subtask: FormControl;
    exp: FormControl;
    sessioninfo: FormControl;
    markerinfo: FormControl;
    PISite: FormControl;
    aggFunc: FormControl;
    species: FormControl;
    //});

    data : any

    _dataExtractionObj: DataExtraction;

    linkGuid: any;
    showGeneratedLink: any;

    public dataSource: MatTableDataSource<Element[]>
    result: any;
    colNames: any;

    pager: any = {};
    pagedItems: any[];

    dataextractionForm = FormControl;

    // SessionInfo Features' names
    public sessionInfoFeature: any[] = [
        { name: 'Analysis_Name' },
        { name: 'Schedule_Name' },
        { name: 'Max_Number_Trials' },
        { name: 'Max_Schedule_Time' },
        { name: 'Database_Name' },
        { name: 'Date_Time' },
        { name: 'Environment' },
        { name: 'Machine_Name' },
        { name: 'Schedule_Run_ID' },
        { name: 'Version' },
        { name: 'Version_Name' },
        { name: 'Application_Version' },
        { name: 'Schedule_Description' },
        { name: 'Schedule_Start_Time' }

    ];

    // Aggregation functions' names
    public aggregationFunction: any[] = [

        { name: 'COUNT' },
        { name: 'MEAN' },
        { name: 'STDEV' },
        { name: 'SUM'}

    ];

    /** control for the selected experiments for multi-selection */
    public expMultiCtrl: FormControl = new FormControl();
    /** control for the MatSelect filter keyword multi-selection */
    public expMultiFilterCtrl: FormControl = new FormControl();
    public MarkerInfoMultiFilterCtrl: FormControl = new FormControl();

    //dataSource: MatTableDataSource<Element[]>;

    constructor(
        public dialog: MatDialog,
        private dataExtractionService: DataExtractionService,
        private spinnerService: NgxSpinnerService,
        private authenticationService: AuthenticationService,
        private pagerService: PagerService,
        public dialogTerms: MatDialog,
        public uploadService: UploadService,
        private expDialogeService: ExpDialogeService,
        private fb: FormBuilder,
        public dialogRefLink: MatDialog
    ) {
        this.colNames = [];
        this.showGeneratedLink = false;
        this.termsChecked = false;
        this.pagedItems = [];
        this._geno = { Description: '', Genotype: '', ID: 0, Link: '' }
        this._dataExtractionObj = {
            ageVals: [], aggNames: '', expIDs: [], genotypeVals: [], isTrialByTrials: false, markerInfoNames: [], pisiteIDs: [],
            sessionInfoNames: [], sessionName: [], sexVals: [], species: '', speciesID: 0, strainVals: [], subExpID: [], subtaskID: 0,
            subTaskName: '', taskID: 0, taskName: ''
        }
        this.task = fb.control('', [Validators.required])
        this.subtask = fb.control('', [Validators.required])
        this.exp = fb.control('', [Validators.required])
        this.sessioninfo = fb.control('', [Validators.required])
        this.markerinfo = fb.control('', [Validators.required])
        this.PISite = fb.control('', [Validators.required])
        this.aggFunc = fb.control('', [Validators.required])
        this.species = fb.control('', [Validators.required])
        this.resetDdls();
    }

    resetDdls() {
        this.subtask.setValue('');
        this.exp.setValue([]);
        this.sessioninfo.setValue([]);
        this.markerinfo.setValue([]);
        this.selectedPiSiteValue = [];
        this.aggFunc.setValue('');
        this.selectedAgeValue = [];
        this.selectedStrainValue = [];
        this.selectedGenotypeValue = [];
        this.selectedSexValue = [];
        this.isTrialByTrial = false;
        this.selectedSubSessionValue = [];

    }

    /** list of experiments filtered by search keyword for multi-selection */
    public filteredExpMulti: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public filteredMarkerInfoList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    @HostListener('window:resize', ['$event'])
    onResize(event : any) {
        $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
    }

    ngOnInit() {

        // loading speciesList for Species Dropdown 
        this.expDialogeService.getAllSpecies().subscribe((data : any) => { this.speciesList = data; console.log(this.speciesList); });


        // loading TaskList for Task Dropdown
        this.dataExtractionService.getAllTask().subscribe((data: any) => { this.taskList = data; });

    }

    selectedSpeciesChange() {


        this.task.setValue('');
        this.resetDdls();

    }

    // Getting Selected Task ID and Pass it to get Explist & SubtaskList
    selectedTaskChange(selectedTaskVal : any, selectedSpeciesvalue :any) {

        var isUser = this.authenticationService.isInRole("user");
        var isAdmin = this.authenticationService.isInRole("administrator");
        var isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
        this.subSessionList = [];

        if (isUser || isAdmin) {

            this.dataExtractionService.getUserGuid().subscribe((userGuid : any) => {

                this.resetDdls();
                this.getExpList(selectedTaskVal, userGuid, selectedSpeciesvalue);
                this.getSubTaskList(selectedTaskVal);
                this.showGeneratedLink = false;

            });

        } else {

            this.resetDdls();
            this.getExpList(selectedTaskVal, "", selectedSpeciesvalue);
            this.getSubTaskList(selectedTaskVal);
            this.showGeneratedLink = false;

        }

    }

    // Getting SubTask ID and ExpID and pass them for getting MarkerInfolist
    selectedSubTaskChange(selectedSubTaskVal : any, selectedExpVal : any) {
        this.markerinfo.setValue([]);

        //this.subSessionList = [];
        this.uploadService.getSessionInfo().subscribe((data : any) => {

            this.subSessionList = data;
            this.selectedSubSessionValue = [];
            //this.markerinfo = new FormControl('', [Validators.required]);

            this.getMarkerInfo(selectedSubTaskVal, selectedExpVal);
            //console.log(selectedSubTaskVal)
            //console.log(this.subSessionList)
            switch (selectedSubTaskVal) {

                case 21:
                case 22:
                case 23:
                case 27:
                    { // case 5C-Analysis
                        this.subSessionList = this.subSessionList.filter((x => x.taskID === 2));
                        break;

                    }
                case 31:
                    { // case PD Analysis
                        this.subSessionList = this.subSessionList.filter((x => x.taskID === 3));
                        break;

                    }
                case 69:
                    { // case PR Analysis
                        this.subSessionList = this.subSessionList.filter((x => x.taskID === 9));
                        break;
                    }

                case 75:
                    { // case PRL Analysis
                        this.subSessionList = this.subSessionList.filter((x => x.taskID === 10));
                        break;
                    }
                case 78:
                    { // case CPT Analysis for Stage 1 to Stage 4
                        this.subSessionList = this.subSessionList.filter(x => (x.taskID === 11) && (x.sessionName == 'Stage 1 - Stimulus Touch') &&
                            (x.sessionName == 'Stage 2 - Target Stimulus Touch') && (x.sessionName == 'Stage 3 - One Target and one non-target') &&
                            (x.sessionName == 'Stage 4 - One Target and four non-targets'));
                        break;
                    }
                
                case 88:
                    {   // VMLC Analysis

                        this.subSessionList = this.subSessionList.filter(x => (x.taskID === 12) && (x.sessionName != 'Punish Incorrect II'));
                        break;

                    }
                case 91:
                    { // Autoshaping 
                        this.subSessionList = this.subSessionList.filter(x => (x.taskID === 13));
                        break;
                    }
                case 98:
                case 99:
                    { // Extinction
                        this.subSessionList = this.subSessionList.filter(x => (x.taskID === 14));
                        break;
                    }
                case 104:
                    { // Long-sequence
                        this.subSessionList = this.subSessionList.filter(x => (x.taskID === 15));
                        break;
                    }


                default:
                    {
                        this.subSessionList = [];
                        //statements; 
                        break;
                    }


            }
            //console.log(this.subSessionList);
        });
    }

    selectedExpChange(selectedExpVal : number) {
        this.subtask.setValue('');
        this.selectedSubTaskChange(this.subtask.value, selectedExpVal);
        // call function for extracting intervention list
        this.getInterventionList(selectedExpVal);
    }


    // Getting ExpList for the selected Task ID
    getExpList(selected_TaskValue : number, userGuid : any, selectedSpeciesvalue : number): any {

        var isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");

        this.dataExtractionService.getAllExpByTaskID(selected_TaskValue, userGuid, isFullDataAccess, selectedSpeciesvalue).subscribe((data : any) => {
            this.expList = data;
            //console.log(this.expList);

            // load the initial expList
            this.filteredExpMulti.next(this.expList.slice());

            this.expMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterExpMulti();
                });

        });

        return this.expList;

    }

    // Getting subTakList for the selected Task ID
    getSubTaskList(selected_TaskValue : number) {

        this.dataExtractionService.getAllSubtaskByTaskID(selected_TaskValue).subscribe((data : any) => {
            this.subTakList = data;
            //console.log(this.subTakList);

        });

        return this.subTakList;

    }

    // Getting InterventionList for the selected experiments
    getInterventionList(selectedExpVal : number) {

        this.dataExtractionService.getAllInterventionByExpID(selectedExpVal).subscribe((data : any) => {
            this.InterventionList = data;
            //console.log(this.subTakList);

        });

        return this.InterventionList;

    }

    getSelectedSubTask(selValue : number) {
        return this.subTakList.find((x : any) => x.id === selValue);
    }


    getSelectedTask(selValue : number) {
        return this.taskList.find((x : any) => x.id === selValue);
    }

    getSelectedSpecies(selValue : number) {
        return this.speciesList.find((x : any) => x.id === selValue);
    }

    // Getting MarkerInfoList for the selected SubTaskID & ExpID
    getMarkerInfo(selected_SubTaskValue : string, selected_ExpVal : string) {
        if (selected_SubTaskValue == '' || selected_ExpVal.length == 0) {
            this.markerInfoList = [];
            return;
        }


        this.spinnerService.show();

        this.dataExtractionService.getMarkerInfoBySubTaskIDExpID(selected_SubTaskValue, selected_ExpVal).subscribe((data : any) => {
            this.markerInfoList = data;
            //console.log(this.markerInfoList);

            this.spinnerService.hide();


            // load the initial expList
            this.filteredMarkerInfoList.next(this.markerInfoList.slice());

            this.MarkerInfoMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterMarkerInfo();
                });

        });

        return this.markerInfoList;
    }

    // Getting AnimalList for the selected ExpIDs
    getAnimalInfo(selected_ExpVal : string) {

        // Age
        this.dataExtractionService.getAnimalAgebyExpIDs(selected_ExpVal).subscribe((data : any) => {
            this.AnimalInfoListAge = data;
            //console.log(this.AnimalInfoListAge);
        });

        // Sex
        this.dataExtractionService.getAnimalSexbyExpIDs(selected_ExpVal).subscribe((data : any) => {
            this.AnimalInfoListSex = data;
            //console.log(this.AnimalInfoListSex);
        });

        // Strain
        this.dataExtractionService.getAnimalStrainbyExpIDs(selected_ExpVal).subscribe((data : any) => {
            this.AnimalInfoListStrain = data;
            //console.log(this.AnimalInfoListStrain);
        });



    }

    //To extract list of Genotypes
    selectedStrainChange(selected_StrainVal : any, selected_ExpVal : string) {
        //console.log(selected_StrainVal);
        this.selectedGenotypeValue = [];
        // apply filtering to Genotype list based on what selected from Strain List
        if (selected_StrainVal.length != 0 || selected_StrainVal != undefined) {

            this.genoIDList = [];
            if (selected_StrainVal.indexOf(1) > -1) {
                this.genoIDList.push(1);
                this.genoIDList.push(4);

            }
            if (selected_StrainVal.indexOf(2) > -1) {
                this.genoIDList.push(2);
                this.genoIDList.push(5);

            }
            if (selected_StrainVal.indexOf(3) > -1) {
                this.genoIDList.push(3);
                this.genoIDList.push(6);

            }
            if (selected_StrainVal.indexOf(4) > -1) {
                this.genoIDList.push(4);

            }
            if (selected_StrainVal.indexOf(5) > -1) {
                this.genoIDList.push(5);

            }
            if (selected_StrainVal.indexOf(6) > -1) {
                this.genoIDList.push(6);

            }
            if (selected_StrainVal.indexOf(7) > -1) {
                this.genoIDList.push(7);
                this.genoIDList.push(11);

            }
            if (selected_StrainVal.indexOf(8) > -1) {
                this.genoIDList.push(8);
                this.genoIDList.push(11);

            }
            if (selected_StrainVal.indexOf(9) > -1) {
                this.genoIDList.push(9);
                this.genoIDList.push(10);
                this.genoIDList.push(12);
                this.genoIDList.push(35);

            }
            if (selected_StrainVal.indexOf(10) > -1) {
                this.genoIDList.push(12);

            }
            if (selected_StrainVal.indexOf(11) > -1) {
                this.genoIDList.push(11);

            }
            if (selected_StrainVal.indexOf(12) > -1) {
                this.genoIDList.push(6);
                this.genoIDList.push(13);
                this.genoIDList.push(14);
                this.genoIDList.push(15);

            }
            if (selected_StrainVal.indexOf(13) > -1) {
                this.genoIDList.push(6);
                this.genoIDList.push(13);
                this.genoIDList.push(16);
                this.genoIDList.push(17);
                this.genoIDList.push(18);

            }
            if (selected_StrainVal.indexOf(14) > -1) {
                this.genoIDList.push(6);
                this.genoIDList.push(13);
                this.genoIDList.push(19);
                this.genoIDList.push(20);
                this.genoIDList.push(21);

            }
            if (selected_StrainVal.indexOf(15) > -1) {
                this.genoIDList.push(22);
                this.genoIDList.push(23);
                this.genoIDList.push(24);

            }
            if (selected_StrainVal.indexOf(16) > -1) {
                this.genoIDList.push(11);
                this.genoIDList.push(25);
                this.genoIDList.push(26);

            }
            if (selected_StrainVal.indexOf(17) > -1) {
                this.genoIDList.push(26);

            }

            if (selected_StrainVal.indexOf(18) > -1) {
                this.genoIDList.push(27);
                this.genoIDList.push(6);

            }
            if (selected_StrainVal.indexOf(19) > -1) {
                this.genoIDList.push(28);
                this.genoIDList.push(6);

            }
            if (selected_StrainVal.indexOf(20) > -1) {
                this.genoIDList.push(29);
                this.genoIDList.push(6);

            }
            if (selected_StrainVal.indexOf(21) > -1) {
                this.genoIDList.push(30);
                this.genoIDList.push(6);

            }
            if (selected_StrainVal.indexOf(22) > -1) {
                this.genoIDList.push(31);

            }
            if (selected_StrainVal.indexOf(23) > -1) {
                this.genoIDList.push(31);
                this.genoIDList.push(32);

            }
            if (selected_StrainVal.indexOf(24) > -1) {
                this.genoIDList.push(26);
                this.genoIDList.push(33);

            }
            if (selected_StrainVal.indexOf(25) > -1) {
                this.genoIDList.push(31);
                this.genoIDList.push(34);

            }

            if (selected_StrainVal.indexOf(26) > -1) {
                this.genoIDList.push(35);
            }

            if (selected_StrainVal.indexOf(27) > -1) {
                this.genoIDList.push(36);
                this.genoIDList.push(37);
            }

            if (selected_StrainVal.indexOf(28) > -1) {
                this.genoIDList.push(37);
                
            }

            if (selected_StrainVal.indexOf(29) > -1) {
                this.genoIDList.push(42);
                this.genoIDList.push(39);
                this.genoIDList.push(40);
                this.genoIDList.push(41);

            }

            if (selected_StrainVal.indexOf(30) > -1) {
                this.genoIDList.push(39);

            }

            if (selected_StrainVal.indexOf(31) > -1) {
                this.genoIDList.push(40);

            }

            if (selected_StrainVal.indexOf(32) > -1) {
                this.genoIDList.push(41);

            }

            if (selected_StrainVal.indexOf(33) > -1) {
                this.genoIDList.push(42);

            }
            if (selected_StrainVal.indexOf(34) > -1) {
                this.genoIDList.push(43);
                this.genoIDList.push(44);

            }
            if (selected_StrainVal.indexOf(35) > -1) {
                this.genoIDList.push(44);

            }

            if (selected_StrainVal.indexOf(36) > -1) {
                this.genoIDList.push(45);
                this.genoIDList.push(46);

            }

            if (selected_StrainVal.indexOf(37) > -1) {
                this.genoIDList.push(46);

            }


            if (selected_StrainVal.indexOf(38) > -1) {
                this.genoIDList.push(47);
                this.genoIDList.push(48);
                this.genoIDList.push(49);
                this.genoIDList.push(50);
                this.genoIDList.push(51);
                this.genoIDList.push(55);

            }

            if (selected_StrainVal.indexOf(39) > -1) {
                this.genoIDList.push(52);
                this.genoIDList.push(53);
                this.genoIDList.push(54);

            }

            if (selected_StrainVal.indexOf(40) > -1) {
                this.genoIDList.push(53);

            }
            if (selected_StrainVal.indexOf(41) > -1) {
                this.genoIDList.push(56);
                this.genoIDList.push(57);
                this.genoIDList.push(58);

            }
            if (selected_StrainVal.indexOf(42) > -1) {
                this.genoIDList.push(57);

            }
            if (selected_StrainVal.indexOf(43) > -1) {
                this.genoIDList.push(58);

            }
            if (selected_StrainVal.indexOf(44) > -1) {
                this.genoIDList.push(59);
                this.genoIDList.push(60);
                this.genoIDList.push(61);
                this.genoIDList.push(62);
                this.genoIDList.push(63);

            }
            if (selected_StrainVal.indexOf(45) > -1) {
                this.genoIDList.push(60);

            }
            if (selected_StrainVal.indexOf(46) > -1) {
                this.genoIDList.push(61);

            }
            if (selected_StrainVal.indexOf(47) > -1) {
                this.genoIDList.push(62);

            }
            if (selected_StrainVal.indexOf(48) > -1) {
                this.genoIDList.push(63);

            }
            if (selected_StrainVal.indexOf(49) > -1) {
                this.genoIDList.push(64);
                this.genoIDList.push(65);
                this.genoIDList.push(66);
                this.genoIDList.push(67);
                this.genoIDList.push(68);

            }
            if (selected_StrainVal.indexOf(50) > -1) {
                this.genoIDList.push(65);

            }
            if (selected_StrainVal.indexOf(51) > -1) {
                this.genoIDList.push(66);

            }
            if (selected_StrainVal.indexOf(52) > -1) {
                this.genoIDList.push(67);

            }
            if (selected_StrainVal.indexOf(53) > -1) {
                this.genoIDList.push(68);

            }
            if (selected_StrainVal.indexOf(54) > -1) {
                this.genoIDList.push(6);
                this.genoIDList.push(69);
            }
            if (selected_StrainVal.indexOf(55) > -1) {
                this.genoIDList.push(70);
                this.genoIDList.push(71);
                this.genoIDList.push(6);
            }



            //console.log(this.genoIDList);
            //selected_ExpVal
            this.dataExtractionService.getAnimalGenotypebyExpIDs(this.genoIDList).subscribe((data : any) => {
                this.AnimalInfoListGenotype = data;
                //console.log(this.AnimalInfoListGenotype);


            });


        }

    }


    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }



    //**************Handling Error Message for the required Drop Down Fields

    // Task Dropdown
    getErrorMessageTask() {

        return this.task.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    // Experiment Dropdown
    getErrorMessageExp() {


        return this.exp.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    // Subtask Dropdown
    getErrorMessageSubTask() {


        return this.subtask.hasError('required') ? FIELDISREQUIRED:
            '';

    }

    // SessionInfo Dropdown
    getErrorMessageSessionInfo() {


        return this.sessioninfo.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    // MarkerInfo Dropdown
    getErrorMessageMarkerInfo() {

        return this.markerinfo.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    // Aggregation Function DropDown
    getErrorMessageAggregation() {

        return this.aggFunc.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    // Species Dropdown
    // Aggregation Function DropDown
    getErrorMessageSpecies() {

        return this.species.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    setDisabledVal() {


        if (!this.termsChecked ||
            (this.task.hasError('required') ||
            this.exp.hasError('required') ||
            this.subtask.hasError('required') ||
            this.sessioninfo.hasError('required') ||
            this.markerinfo.hasError('required') ||
            this.species.hasError('required') ||
            (this.aggFunc.hasError('required') && this.isTrialByTrial == false)
            )


        ) {
            return true;
        }
        return false;
    }

    //***************End of Error Handling***********************

    // handling multi filtered experiment list
    private filterExpMulti() {
        if (!this.expList) {
            return;
        }

        // get the search keyword
        let search = this.expMultiFilterCtrl.value;

        if (!search) {
            this.filteredExpMulti.next(this.expList.slice());
            return;
        } else {
            search = search.toLowerCase();
        }

        // filter the Experiment
        this.filteredExpMulti.next(
            this.expList.filter((t : any) => t.expName.toLowerCase().indexOf(search) > -1)
        );
    }

    // handling multi filtered Markerinfo list
    private filterMarkerInfo() {
        if (!this.markerInfoList) {
            return;
        }

        // get the search keyword
        let searchMarkerInfo = this.MarkerInfoMultiFilterCtrl.value;

        if (!searchMarkerInfo) {
            this.filteredMarkerInfoList.next(this.markerInfoList.slice());
            return;
        } else {
            searchMarkerInfo = searchMarkerInfo.toLowerCase();
        }

        // filter the Experiment
        this.filteredMarkerInfoList.next(
            this.markerInfoList.filter((x : any) => x.toLowerCase().indexOf(searchMarkerInfo) > -1)
        );
    }

    // Function defintion for opening Terms of service
    openDialogTerms(): void {

        let dialogref = this.dialogTerms.open(TermsDialogeComponent, {
            //height: '700px',
            //width: '900px',
            data: {}

        });

        dialogref.afterClosed().subscribe();
    }

    //*********Get Data From Database and show the Result in Table*********
    GetData() {
        this.spinnerService.show();

        var selectedTask = this.getSelectedTask(this.task.value);
        var selectedSubTask = this.getSelectedSubTask(this.subtask.value);
        var selectedSpeceis = this.getSelectedSpecies(this.species.value);

        // Function Definition 
        this._dataExtractionObj.isTrialByTrials = this.isTrialByTrial;
        if (this.isTrialByTrial) {
            this.aggFunc.setValue('COUNT');
        }

        this._dataExtractionObj.taskID = this.task.value;
        this._dataExtractionObj.taskName = selectedTask.name;
        this._dataExtractionObj.speciesID = this.species.value;
        this._dataExtractionObj.species = selectedSpeceis.species
        this._dataExtractionObj.expIDs = this.exp.value;
        this._dataExtractionObj.subtaskID = this.subtask.value;
        this._dataExtractionObj.subTaskName = selectedSubTask.originalName;
        this._dataExtractionObj.sessionInfoNames = this.sessioninfo.value;
        this._dataExtractionObj.markerInfoNames = this.markerinfo.value;
        this._dataExtractionObj.aggNames = this.aggFunc.value;
        this._dataExtractionObj.pisiteIDs = this.selectedPiSiteValue;
        this._dataExtractionObj.ageVals = this.selectedAgeValue;
        this._dataExtractionObj.sexVals = this.selectedSexValue;
        this._dataExtractionObj.genotypeVals = this.selectedGenotypeValue;
        this._dataExtractionObj.strainVals = this.selectedStrainValue;
        this._dataExtractionObj.subExpID = this.selectedInterventionValue;
        this._dataExtractionObj.sessionName = this.selectedSubSessionValue;

        this.dataExtractionService.getData(this._dataExtractionObj).subscribe((data : any) => {
            //console.log(this._dataExtractionObj);
            //console.log(data);

            this.result = data.listOfRows;

            

            this.setPage(1);

            this.linkGuid = data.linkGuid;
            // this.dataSource = new MatTableDataSource(this.Result);
            this.colNames = [];

            if (this.result.length > 0) {
                var a = this.result[0];
                Object.keys(a).forEach(function (key) { return /*console.log(key)*/; });
                for (var key in a) {

                    if (key == 'Image' || key == 'Image_Description') {
                        if (this.task.value == 3 || this.task.value == 4 || this.task.value == 11) {
                            this.colNames.push(key);
                        } // else -> do not push Image col 
                    } else {
                        this.colNames.push(key);
                    }


                }
            }

            setTimeout(() => {
                this.spinnerService.hide();

                $('.pane-hScroll').scroll(function () {
                    $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
                });

            }, 500);


        });



    }

    GenerateLink() {
        this.dataExtractionService.saveLink(this.linkGuid).subscribe((data : any) => {
            if (data === true) {
                this.showGeneratedLink = true;

                const dialogRefLink = this.dialog.open(NotificationDialogComponent, {
                });
                dialogRefLink.componentInstance.message = "http://localhost:4200/data-link?linkguid=" + this.linkGuid;



            } else {
                //console.log('Not Done!');

            }
        });
    }

    DownloadCsv() {

        this.dataExtractionService.IncreaseCounter().subscribe(data => {
            var csv: string;
            csv= '';

            //var items = this.result;
            var items = this.result;
            // Loop the array of objects
            for (let row = 0; row < items.length; row++) {
                let keysAmount = Object.keys(items[row]).length

                let keysCounter = 0

                // If this is the first row, generate the headings
                if (row === 0) {

                    // Loop each property of the object
                    for (let key in items[row]) {
                        // This is to not add a comma at the last cell
                        // The '\r\n' adds a new line
                        if (key == 'AnimalID') { csv = ''; }
                        csv += key + (keysCounter + 1 < keysAmount ? ',' : '\r\n')
                        //console.log(csv)
                        keysCounter++
                    }
                }

                keysCounter = 0;
                
                for (let key in items[row]) {

                    var csvToAdd = "";
                    if (items[row][key] !== null) {
                        csvToAdd = items[row][key].toString();
                        csvToAdd = csvToAdd.split(",").join("-");

                    }

                    csv += csvToAdd + (keysCounter + 1 < keysAmount ? ',' : '\r\n')
                    keysCounter++

                }

                keysCounter = 0
            }


            var blob = new Blob([csv], { type: 'text/csv' });
            var filename = 'exported_' + new Date().toLocaleString() + '.csv';
            const _win = window.navigator as any
            if (_win.msSaveOrOpenBlob) {
                _win.msSaveBlob(blob, filename);
            }
            else {
                var elem = window.document.createElement('a');
                elem.href = window.URL.createObjectURL(blob);
                elem.download = filename;
                document.body.appendChild(elem);
                elem.click();
                document.body.removeChild(elem);
            }

        });



    }

    selectAllExperiments() {
        this.exp.setValue([]);

        //// in future if we want to enable select all with filter follow this:
        //this.filteredExpMulti.subscribe(value => {
        //    for (let filteredItem of value) {
        //        console.log(filteredItem.expID);
        //    }
        //})

        for (let exp of this.expList) {
            this.exp.value.push(exp.expID);
        }
        this.selectedExpChange(this.exp.value);
    }

    deselectAllExperiments() {
        this.exp.setValue([]);
        this.selectedExpChange(this.exp.value);
    }

    // Function defintion to add pagination to table 
    setPage(page: number) {
        // get pager object from service
        this.pager = this.pagerService.getPager(this.result.length, page, 20);

        if (page < 1 || page > this.pager.totalPages) {
            this.pagedItems = [];
            return;
        }

        // get current page of items
        this.pagedItems = this.result.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }




}
