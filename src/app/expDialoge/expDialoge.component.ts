import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
import { Experiment } from '../models/experiment';
import { Location } from '@angular/common';
import { TaskAnalysisService } from '../services/taskanalysis.service';
import { ExpDialogeService } from '../services/expdialoge.service';
import { PISiteService } from '../services/piSite.service';
import { LoadingService } from '../services/loadingservice'
// import { UploadService } from '../services/upload.service';
import { CogbytesService } from '../services/cogbytes.service';
import { ReplaySubject ,  Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { EXPERIMENTNAMETAKEN, FIELDISREQUIRED, NAIFNOTAPPLICABLE } from '../shared/messages';


@Component({

  selector: 'app-expdialoge',
  templateUrl: './ExpDialoge.component.html',
  styleUrls: ['./ExpDialoge.component.scss'],
  providers: [TaskAnalysisService, ExpDialogeService, PISiteService, CogbytesService]

  })
export class ExpDialogeComponent implements OnInit {

  doiModel: string;
  isTaken: boolean;
  isRepoLink: any;
  repModel: any;

  taskList: any;
  piSiteList: any;
  speciesList: any;
  repList: any;

  exp: UntypedFormControl;
  sDate: UntypedFormControl;
  eDate: UntypedFormControl;
  task: UntypedFormControl;
  species: UntypedFormControl;
  piSite: UntypedFormControl;
  status: UntypedFormControl;
  expDescription: UntypedFormControl;
  expBattery: UntypedFormControl;
  isMultipleSessions: UntypedFormControl;


  public repMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
  public filteredRepList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  private _experiment: Experiment;

  constructor(public thisDialogRef: MatDialogRef<ExpDialogeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialog: MatDialog, private location: Location,
    private taskAnalysisService: TaskAnalysisService, private expDialogeService: ExpDialogeService,
    private piSiteService: PISiteService, private spinnerService: LoadingService, private cogbytesService: CogbytesService,
    private fb: UntypedFormBuilder
  ) {
    this.doiModel = '';
    this.isTaken = false;
    this.exp = fb.control('', [Validators.required]);
    this.sDate = fb.control('', [Validators.required]);
    this.eDate = fb.control('', [Validators.required]);
    this.task = fb.control({ value: '', disabled: data.experimentObj != null}, [Validators.required]);
    this.species = fb.control('', [Validators.required]);
    this.piSite = fb.control('', [Validators.required]);
    this.status = fb.control('', [Validators.required]);
    this.expDescription = fb.control('', [Validators.required]);
    this.expBattery = fb.control('', [Validators.required]);
    this.isMultipleSessions = fb.control('', [Validators.required]);
    this._experiment = {
      doi: '', endExpDate: new Date(), expID: 0, expName: '', imageIds: [], imageInfo: '', multipleSessions: true,
      piSiteName: '', piSiteUser: '', puSID: 0, repoGuid: '', species: '', speciesID: 0, startExpDate: new Date(), status: false,
      taskBattery: '', taskDescription: '', taskID: 0, taskName: '', userID: '', userName: ''
    };
  }

  ngOnInit() {
    this.taskAnalysisService.getAllSelect().subscribe((data: any) => {
      this.taskList = data; /* console.log(this.taskList)*/
    });
    this.expDialogeService.getAllSpecies().subscribe((data: any) => {
      this.speciesList = data; /* console.log(this.speciesList)*/
    });
    this.piSiteService.getPISitebyUserID().subscribe((data: any) => {
      this.piSiteList = data;
    });

    this.getRepList();

    this.isRepoLink = '0';

    // console.log(this.data.experimentObj);
    // if it is an Edit model
    if (this.data.experimentObj != null) {
      this.exp.setValue(this.data.experimentObj.expName);
      this.sDate.setValue(this.data.experimentObj.startExpDate);
      this.eDate.setValue(this.data.experimentObj.endExpDate);
      this.task.setValue(this.data.experimentObj.taskID);
      this.species.setValue(this.data.experimentObj.speciesID);
      this.expDescription.setValue(this.data.experimentObj.taskDescription);
      this.expBattery.setValue(this.data.experimentObj.taskBattery);
      this.piSite.setValue(this.data.experimentObj.pusid);
      this.doiModel = this.data.experimentObj.doi;
      this.status.setValue(this.data.experimentObj.status ? '1' : '0');
      this.isMultipleSessions.setValue(this.data.experimentObj.multipleSessions ? '1' : '0');
      if (this.data.experimentObj.repoGuid !== '') {
        this.isRepoLink = '1';
        this.repModel = this.data.experimentObj.repoGuid;
      }
    }
  }

  getRepList() {

    this.cogbytesService.getAllRepositories().subscribe((data: any) => {
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

  onCloseCancel(): void {


    this.thisDialogRef.close('Cancel');

  }

  onCloseSubmit(): void {
    this.spinnerService.show();

    this._experiment.expName = this.exp.value;
    this._experiment.startExpDate = this.sDate.value;
    this._experiment.endExpDate = this.eDate.value;
    this._experiment.taskID = this.getSelectedTask(this.task.value).id; // should be readonly
    this._experiment.speciesID = this.species.value;
    this._experiment.taskDescription = this.expDescription.value;
    this._experiment.taskBattery = this.expBattery.value;
    this._experiment.puSID = this.getSelectedPIS(this.piSite.value).pusid;
    this._experiment.doi = this.doiModel;
    this._experiment.status = this.status.value === '1' ? true : false;
    this._experiment.multipleSessions = this.isMultipleSessions.value === '1' ? true : false;
    if (this.isRepoLink === '1') {
      this._experiment.repoGuid = this.repModel;
    }

    // console.log(this._experiment.ImageIds);

    if (this.data.experimentObj == null) {
      // Insert Mode: Insert Experiment
      this.isTaken = false;

      this.expDialogeService.create(this._experiment).map((res: any) => {
        if (res === 'Taken') {
          this.isTaken = true;
          this.exp.setErrors({ 'taken': true });
        } else {
          this.thisDialogRef.close();
        }

      }).subscribe((data: any) => {

        setTimeout(() => {
          this.spinnerService.hide();


        }, 500);

      });
    } else { // Edit Mode: edit experiment


      this.isTaken = false;
      this._experiment.expID = this.data.experimentObj.expID;
      this.expDialogeService.updateExp(this._experiment).map((res: any) => {



        if (res === 'Taken') {
          this.isTaken = true;
          this.exp.setErrors({ 'taken': true });
        } else {
          this.thisDialogRef.close();
        }

      }).subscribe((data: any) => {
        setTimeout(() => {
          this.spinnerService.hide();

        }, 500);

      });

    }

  }

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

  getSelectedTask(selectedValue: any) {
    return this.taskList.find((x: any) => x.id === selectedValue);
  }

  getSelectedPIS(selectedVal: any) {
    return this.piSiteList.find((x: any) => x.pusid === selectedVal);
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
      this.repList.filter((x: any) => x.title.toLowerCase().indexOf(searchRep) > -1)
    );
  }




}
