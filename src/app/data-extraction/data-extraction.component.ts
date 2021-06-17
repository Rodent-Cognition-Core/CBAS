import { Component, OnInit, HostListener } from '@angular/core';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
//import { NgModel } from '@angular/forms';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Subject } from 'rxjs/Subject';
import { take, takeUntil } from 'rxjs/operators';
import { DataExtractionService } from '../services/dataextraction.service'
import { DataExtraction } from '../models/dataextraction';
import { MatTableDataSource, MatDialogRef, MatDialog } from '@angular/material';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { AuthenticationService } from '../services/authentication.service';
import { PagerService } from '../services/pager.service';
import { Geno } from '../models/geno';
import { TermsDialogeComponent } from '../termsDialoge/termsDialoge.component';
import { UploadService } from '../services/upload.service';
import { ExpDialogeService } from '../services/expdialoge.service';

declare var $: any;


@Component({
    selector: 'app-data-extraction',
    templateUrl: './data-extraction.component.html',
    styleUrls: ['./data-extraction.component.scss']
})
export class DataExtractionComponent implements OnInit {

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
    _geno = new Geno();
    filteredGenoList: any[] = [];
    InterventionList: any[] = [];
    speciesList: any

    // ngModels vars
    selectedTaskvalue: any;
    selectedExpvalue: any;
    selectedSubTaskValue: any;
    selectedSessionInfoValue: any;
    selectedMarkerInfoValue: any;
    selectedPiSiteValue: any;
    selectedAgeValue: any;
    selectedSexValue: any;
    selectedGenotypeValue: any;
    selectedStrainValue: any;
    selectedAggFunction: any;
    termsChecked: boolean;
    isTrialByTrial: any;
    selectedInterventionValue: any
    selectedSubSessionValue: any
    selectedSpeciesvalue: any

    // FormControls vars
    task = new FormControl('', [Validators.required]);
    subtask = new FormControl('', [Validators.required]);
    exp = new FormControl('', [Validators.required]);
    sessioninfo = new FormControl('', [Validators.required]);
    markerinfo = new FormControl('', [Validators.required]);
    PISite = new FormControl('', [Validators.required]);
    aggFunc = new FormControl('', [Validators.required]);
    species = new FormControl('', [Validators.required]);

    _dataExtractionObj = new DataExtraction();

    linkGuid: any;
    dialogRefLink: MatDialogRef<NotificationDialogComponent>;
    showGeneratedLink: any;


    dataSource: MatTableDataSource<Element[]>;
    result: any;
    colNames: any;

    pager: any = {};
    pagedItems: any[];


    // SessionInfo Features' names
    private sessionInfoFeature: any[] = [
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
    private aggregationFunction: any[] = [

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

    constructor(
        public dialog: MatDialog,
        private dataExtractionService: DataExtractionService,
        private spinnerService: Ng4LoadingSpinnerService,
        private authenticationService: AuthenticationService,
        private pagerService: PagerService,
        public dialogTerms: MatDialog,
        public uploadService: UploadService,
        private expDialogeService: ExpDialogeService,
    ) {
        this.colNames = [];
        this.showGeneratedLink = false;
        this.resetDdls();
    }

    resetDdls() {
        this.selectedSubTaskValue = '';
        this.selectedExpvalue = [];
        this.selectedSessionInfoValue = [];
        this.selectedMarkerInfoValue = [];
        this.selectedPiSiteValue = [];
        this.selectedAggFunction = [];
        this.selectedAgeValue = [];
        this.selectedStrainValue = [];
        this.selectedGenotypeValue = [];
        this.selectedSexValue = [];
        this.isTrialByTrial = false;
        this.selectedSubSessionValue = [];


        this.subtask = new FormControl('', [Validators.required]);
        this.exp = new FormControl('', [Validators.required]);
        this.sessioninfo = new FormControl('', [Validators.required]);
        this.markerinfo = new FormControl('', [Validators.required]);
        this.PISite = new FormControl('', [Validators.required]);

    }

    /** list of experiments filtered by search keyword for multi-selection */
    public filteredExpMulti: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    public filteredMarkerInfoList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    @HostListener('window:resize', ['$event'])
    onResize(event) {
        $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
    }

    ngOnInit() {

        // loading speciesList for Species Dropdown 
        this.expDialogeService.getAllSpecies().subscribe(data => { this.speciesList = data; console.log(this.speciesList); });


        // loading TaskList for Task Dropdown
        this.dataExtractionService.getAllTask().subscribe(data => { this.taskList = data; });

    }

    selectedSpeciesChange() {


        this.selectedTaskvalue = '';
        this.resetDdls();

    }

    // Getting Selected Task ID and Pass it to get Explist & SubtaskList
    selectedTaskChange(selectedTaskVal, selectedSpeciesvalue) {

        var isUser = this.authenticationService.isInRole("user");
        var isAdmin = this.authenticationService.isInRole("administrator");
        this.subSessionList = [];

        if (isUser || isAdmin) {

            this.dataExtractionService.getUserGuid().subscribe(userGuid => {

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
    selectedSubTaskChange(selectedSubTaskVal, selectedExpVal) {
        this.selectedMarkerInfoValue = [];

        //this.subSessionList = [];
        this.uploadService.getSessionInfo().subscribe(data => {

            this.subSessionList = data;
            this.selectedSubSessionValue = [];
            this.markerinfo = new FormControl('', [Validators.required]);

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
                    {
                        this.subSessionList = this.subSessionList.filter(x => (x.taskID === 14));
                        break;
                    }
                case 104:
                    {
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

    selectedExpChange(selectedExpVal) {
        this.selectedSubTaskValue = '';
        this.subtask = new FormControl('', [Validators.required]);
        this.selectedSubTaskChange(this.selectedSubTaskValue, selectedExpVal);
        // call function for extracting intervention list
        this.getInterventionList(selectedExpVal);
    }


    // Getting ExpList for the selected Task ID
    getExpList(selected_TaskValue, userGuid, selectedSpeciesvalue): any {

        this.dataExtractionService.getAllExpByTaskID(selected_TaskValue, userGuid, selectedSpeciesvalue).subscribe(data => {
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
    getSubTaskList(selected_TaskValue) {

        this.dataExtractionService.getAllSubtaskByTaskID(selected_TaskValue).subscribe(data => {
            this.subTakList = data;
            //console.log(this.subTakList);

        });

        return this.subTakList;

    }

    // Getting InterventionList for the selected experiments
    getInterventionList(selectedExpVal) {

        this.dataExtractionService.getAllInterventionByExpID(selectedExpVal).subscribe(data => {
            this.InterventionList = data;
            //console.log(this.subTakList);

        });

        return this.InterventionList;

    }

    getSelectedSubTask(selValue) {
        return this.subTakList.find(x => x.id === selValue);
    }


    getSelectedTask(selValue) {
        return this.taskList.find(x => x.id === selValue);
    }

    getSelectedSpecies(selValue) {
        return this.speciesList.find(x => x.id === selValue);
    }

    // Getting MarkerInfoList for the selected SubTaskID & ExpID
    getMarkerInfo(selected_SubTaskValue, selected_ExpVal) {
        if (selected_SubTaskValue == '' || selected_ExpVal.length == 0) {
            this.markerInfoList = [];
            return;
        }


        this.spinnerService.show();

        this.dataExtractionService.getMarkerInfoBySubTaskIDExpID(selected_SubTaskValue, selected_ExpVal).subscribe(data => {
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
    getAnimalInfo(selected_ExpVal) {

        // Age
        this.dataExtractionService.getAnimalAgebyExpIDs(selected_ExpVal).subscribe(data => {
            this.AnimalInfoListAge = data;
            //console.log(this.AnimalInfoListAge);
        });

        // Sex
        this.dataExtractionService.getAnimalSexbyExpIDs(selected_ExpVal).subscribe(data => {
            this.AnimalInfoListSex = data;
            //console.log(this.AnimalInfoListSex);
        });

        // Strain
        this.dataExtractionService.getAnimalStrainbyExpIDs(selected_ExpVal).subscribe(data => {
            this.AnimalInfoListStrain = data;
            //console.log(this.AnimalInfoListStrain);
        });



    }

    //To extract list of Genotypes
    selectedStrainChange(selected_StrainVal, selected_ExpVal) {
        //console.log(selected_StrainVal);
        this.selectedGenotypeValue = [];
        // apply filtering to Genotype list based on what selected from Strain List
        if (selected_StrainVal !== undefined || selected_StrainVal.length != 0) {

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

            //console.log(this.genoIDList);
            //selected_ExpVal
            this.dataExtractionService.getAnimalGenotypebyExpIDs(this.genoIDList).subscribe(data => {
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

        return this.task.hasError('required') ? 'You must enter a value' :
            '';

    }

    // Experiment Dropdown
    getErrorMessageExp() {


        return this.exp.hasError('required') ? 'You must enter a value' :
            '';

    }

    // Subtask Dropdown
    getErrorMessageSubTask() {


        return this.subtask.hasError('required') ? 'You must enter a value' :
            '';

    }

    // SessionInfo Dropdown
    getErrorMessageSessionInfo() {


        return this.sessioninfo.hasError('required') ? 'You must enter a value' :
            '';

    }

    // MarkerInfo Dropdown
    getErrorMessageMarkerInfo() {

        return this.markerinfo.hasError('required') ? 'You must enter a value' :
            '';

    }

    // Aggregation Function DropDown
    getErrorMessageAggregation() {

        return this.aggFunc.hasError('required') ? 'You must enter a value' :
            '';
    }

    // Species Dropdown
    // Aggregation Function DropDown
    getErrorMessageSpecies() {

        return this.species.hasError('required') ? 'You must enter a value' :
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
            this.expList.filter(t => t.expName.toLowerCase().indexOf(search) > -1)
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
            this.markerInfoList.filter(x => x.toLowerCase().indexOf(searchMarkerInfo) > -1)
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

        var selectedTask = this.getSelectedTask(this.selectedTaskvalue);
        var selectedSubTask = this.getSelectedSubTask(this.selectedSubTaskValue);
        var selectedSpeceis = this.getSelectedSpecies(this.selectedSpeciesvalue);

        // Function Definition 
        this._dataExtractionObj.isTrialByTrials = this.isTrialByTrial;
        if (this.isTrialByTrial) {
            this.selectedAggFunction = ['COUNT'];
        }

        this._dataExtractionObj.taskID = this.selectedTaskvalue;
        this._dataExtractionObj.taskName = selectedTask.name;
        this._dataExtractionObj.speciesID = this.selectedSpeciesvalue;
        this._dataExtractionObj.species = selectedSpeceis.species
        this._dataExtractionObj.expIDs = this.selectedExpvalue;
        this._dataExtractionObj.subtaskID = this.selectedSubTaskValue;
        this._dataExtractionObj.subTaskName = selectedSubTask.originalName;
        this._dataExtractionObj.sessionInfoNames = this.selectedSessionInfoValue;
        this._dataExtractionObj.markerInfoNames = this.selectedMarkerInfoValue;
        this._dataExtractionObj.aggNames = this.selectedAggFunction;
        this._dataExtractionObj.pisiteIDs = this.selectedPiSiteValue;
        this._dataExtractionObj.ageVals = this.selectedAgeValue;
        this._dataExtractionObj.sexVals = this.selectedSexValue;
        this._dataExtractionObj.genotypeVals = this.selectedGenotypeValue;
        this._dataExtractionObj.strainVals = this.selectedStrainValue;
        this._dataExtractionObj.subExpID = this.selectedInterventionValue;
        this._dataExtractionObj.sessionName = this.selectedSubSessionValue;

        this.dataExtractionService.getData(this._dataExtractionObj).subscribe(data => {
            //console.log(this._dataExtractionObj);
            //console.log(data);

            this.result = data.listOfRows;

            

            this.setPage(1);

            this.linkGuid = data.linkGuid;
            // this.dataSource = new MatTableDataSource(this.Result);
            this.colNames = [];

            if (this.result.length > 0) {
                var a = this.result[0];
                Object.keys(a).forEach(function (key) { return console.log(key); });
                for (var key in a) {

                    if (key == 'Image' || key == 'Image_Description') {
                        if (this.selectedTaskvalue == 3 || this.selectedTaskvalue == 4 || this.selectedTaskvalue == 11) {
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
        this.dataExtractionService.saveLink(this.linkGuid).subscribe(data => {
            if (data === true) {
                this.showGeneratedLink = true;

                this.dialogRefLink = this.dialog.open(NotificationDialogComponent, {
                });
                this.dialogRefLink.componentInstance.message = "http://localhost:4200/data-link?linkguid=" + this.linkGuid;



            } else {
                console.log('Not Done!');

            }
        });
    }

    DownloadCsv() {

        this.dataExtractionService.IncreaseCounter().subscribe(data => {

            let csv: '';

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
                        console.log(csv)
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
            if (window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveBlob(blob, filename);
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
        this.selectedExpvalue = [];

        //// in future if we want to enable select all with filter follow this:
        //this.filteredExpMulti.subscribe(value => {
        //    for (let filteredItem of value) {
        //        console.log(filteredItem.expID);
        //    }
        //})

        for (let exp of this.expList) {
            this.selectedExpvalue.push(exp.expID);
        }
        this.selectedExpChange(this.selectedExpvalue);
    }

    deselectAllExperiments() {
        this.selectedExpvalue = [];
        this.selectedExpChange(this.selectedExpvalue);
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
