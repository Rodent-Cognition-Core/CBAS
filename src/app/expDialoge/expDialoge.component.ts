import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { Experiment } from '../models/experiment';
import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { ExpDialogeService } from '../services/expdialoge.service';
import { PISiteService } from '../services/piSite.service';
import { NgxSpinnerService } from 'ngx-spinner';
//import { UploadService } from '../services/upload.service';
import { SharedModule } from '../shared/shared.module';
import { CogbytesService } from '../services/cogbytes.service'
import { ReplaySubject ,  Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { EXPERIMENTNAMETAKEN, FIELDISREQUIRED, NAIFNOTAPPLICABLE } from '../shared/messages';


@Component({

    selector: 'app-expDialoge',
    templateUrl: './ExpDialoge.component.html',
    styleUrls: ['./ExpDialoge.component.scss'],
    providers: [TaskAnalysisService, ExpDialogeService, PISiteService, CogbytesService]

})
export class ExpDialogeComponent implements OnInit {

    expNameModel: string;
    sDateModel: Date;
    eDateModel: Date;
    taskDesModel: string;
    DOIModel: string;
    statusModel: string;
    isTaken: boolean;
    selectedvalue: any;
    selectPISvalue: any;
    speciesModel: any;
    taskBatteryModel: any;
    isMultipleSessionsModel: string;
    isRepoLink: any;
    repModel: any;

    taskList: any;
    piSiteList: any;
    speciesList: any;
    repList: any;

    public repMultiFilterCtrl: FormControl = new FormControl();
    public filteredRepList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    repMultiSelect: any;

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    private _experiment = new Experiment();

    constructor(public thisDialogRef: MatDialogRef<ExpDialogeComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, private location: Location,
        private taskAnalysisService: TaskAnalysisService, private expDialogeService: ExpDialogeService,
        private piSiteService: PISiteService, private spinnerService: NgxSpinnerService, private cogbytesService: CogbytesService) { }

    ngOnInit() {
        this.taskAnalysisService.getAllSelect().subscribe(data => { this.taskList = data; /*console.log(this.taskList)*/; });
        this.piSiteService.getPISitebyUserID().subscribe(data => { this.piSiteList = data; });
        this.expDialogeService.getAllSpecies().subscribe(data => { this.speciesList = data; /*console.log(this.speciesList)*/; });
        this.GetRepList();

        this.isRepoLink = '0';

        //console.log(this.data.experimentObj);
        // if it is an Edit model
        if (this.data.experimentObj != null) {
            this.expNameModel = this.data.experimentObj.expName;
            this.sDateModel = this.data.experimentObj.startExpDate;
            this.eDateModel = this.data.experimentObj.endExpDate;
            this.selectedvalue = this.data.experimentObj.taskID;
            this.speciesModel = this.data.experimentObj.speciesID
            this.taskDesModel = this.data.experimentObj.taskDescription;
            this.taskBatteryModel = this.data.experimentObj.taskBattery;
            this.selectPISvalue = this.data.experimentObj.pusid;
            this.DOIModel = this.data.experimentObj.doi;
            this.statusModel = this.data.experimentObj.status ? "1" : "0";
            this.isMultipleSessionsModel = this.data.experimentObj.multipleSessions ? "1" : "0";
            if (this.data.experimentObj.repoGuid != "") {
                this.isRepoLink = '1';
                this.repModel = this.data.experimentObj.repoGuid;
            }
        }
    }

    GetRepList() {

        this.cogbytesService.getAllRepositories().subscribe(data => {
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
            this.repList.filter(x => x.title.toLowerCase().indexOf(searchRep) > -1)
        );
    }

    onCloseCancel(): void {


        this.thisDialogRef.close('Cancel');

    }

    onCloseSubmit(): void {
        this.spinnerService.show();

        this._experiment.ExpName = this.expNameModel;
        this._experiment.StartExpDate = this.sDateModel;
        this._experiment.EndExpDate = this.eDateModel;
        this._experiment.TaskID = this.getSelectedTask(this.selectedvalue).id; // should be readonly
        this._experiment.SpeciesID = this.speciesModel;
        this._experiment.TaskDescription = this.taskDesModel;
        this._experiment.TaskBattery = this.taskBatteryModel;
        this._experiment.PUSID = this.getSelectedPIS(this.selectPISvalue).pusid;
        this._experiment.DOI = this.DOIModel;
        this._experiment.Status = this.statusModel == "1" ? true : false;
        this._experiment.multipleSessions = this.isMultipleSessionsModel == "1" ? true : false;
        if (this.isRepoLink == '1') {
            this._experiment.repoGuid = this.repModel;
        }

        //console.log(this._experiment.ImageIds);

        if (this.data.experimentObj == null) {
            // Insert Mode: Insert Experiment
            this.isTaken = false;

            this.expDialogeService.create(this._experiment).map(res => {
                if (res == "Taken") {
                    this.isTaken = true;
                    this.exp.setErrors({ 'taken': true });
                } else {
                    this.thisDialogRef.close();
                }

            }).subscribe(data => {

                setTimeout(() => {
                    this.spinnerService.hide();


                }, 500);

            });
        } else { // Edit Mode: edit experiment


            this.isTaken = false;
            this._experiment.ExpID = this.data.experimentObj.expID;
            this.expDialogeService.updateExp(this._experiment).map(res => {



                if (res == "Taken") {
                    this.isTaken = true;
                    this.exp.setErrors({ 'taken': true });
                } else {
                    this.thisDialogRef.close();
                }

            }).subscribe(data => {
                setTimeout(() => {
                    this.spinnerService.hide();

                }, 500);

            });

        }

    }

    exp = new FormControl('', [Validators.required]);
    sDate = new FormControl('', [Validators.required]);
    eDate = new FormControl('', [Validators.required]);
    task = new FormControl('', [Validators.required]);
    species = new FormControl('', [Validators.required]);
    piSite = new FormControl('', [Validators.required]);
    status = new FormControl('', [Validators.required]);
    expDescription = new FormControl('', [Validators.required]);
    expBattery = new FormControl('', [Validators.required]);
    isMultipleSessions = new FormControl('', [Validators.required]);

    getErrorMessage() {

        return this.exp.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageTaken() {

        return this.exp.hasError('taken') ? EXPERIMENTNAMETAKEN :
            '';

    }

    getErrorMessagesDate() {

        return this.sDate.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageeDate() {

        return this.eDate.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    getErrorMessageTask() {


        return this.task.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    getErrorMessagePiSite() {


        return this.piSite.hasError('required') ? FIELDISREQUIRED :
            '';

    }

    getErrorMessageStatus() {   
        return this.status.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageExpDescription() {
        return this.expDescription.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageSpecies() {
        return this.species.hasError('required') ? FIELDISREQUIRED :
            '';
    }

    getErrorMessageExpBattery() {

        return this.expBattery.hasError('required') ? FIELDISREQUIRED + ' ' + NAIFNOTAPPLICABLE :
            '';

    }

    getErrorMessageMultipleSessions() {

        return this.isMultipleSessions.hasError('required') ? FIELDISREQUIRED :
            '';
    }


    setDisabledVal() {

        if (this.exp.hasError('required') ||
            this.sDate.hasError('required') ||
            this.eDate.hasError('required') ||
            this.task.hasError('required') ||
            this.piSite.hasError('required') ||
            this.status.hasError('required') ||
            this.expDescription.hasError('required') ||
            this.expBattery.hasError('required') ||
            this.species.hasError('required') ||
            this.isMultipleSessions.hasError('required')
        ) {

            return true;
        }

        return false;
    }

    getSelectedTask(selectedValue) {
        return this.taskList.find(x => x.id === selectedValue);
    }

    getSelectedPIS(selectedVal) {
        return this.piSiteList.find(x => x.pusid === selectedVal);
    }




}
