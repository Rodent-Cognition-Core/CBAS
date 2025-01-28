import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExpDialogeComponent } from '../expDialoge/expDialoge.component';
import { UploadService } from '../services/upload.service';
import { ExperimentService } from '../services/experiment.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { Location } from '@angular/common';
import { SubExpDialogeComponent } from '../sub-exp-dialoge/sub-exp-dialoge.component';
import { SubExpDialogeService } from '../services/subexpdialoge.service';
import { PostProcessingQcService } from '../services/postprocessingqc.service';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
// import * as _ from 'underscore';
import { NgxSpinnerService } from 'ngx-spinner';
import { CogbytesService } from '../services/cogbytes.service';
// import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { CONFIRMDELETE, PLEASERUNPREPROCESSING, POSTPROCESSINGDONE } from '../shared/messages';

@Component({
  selector: 'app-shared-experiment',
  templateUrl: './shared-experiment.component.html',
  styleUrls: ['./shared-experiment.component.scss'],
  providers: [UploadService, ExperimentService, SubExpDialogeService, PostProcessingQcService, CogbytesService]
  })
export class SharedExperimentComponent implements OnInit {


  @Input() hideSubExperiment: boolean;
  @Input() showSubExpTbl: boolean;

  @Output() outSelectedExperiment: EventEmitter<string> = new EventEmitter();
  @Output() outSelectedSubExperiment: EventEmitter<string> = new EventEmitter();
  // @Input() selectedValue: string;
  // ngModel vars
  faQuestionCircle = faQuestionCircle;
  selectedExpValue: any;
  selectedSubExpValue: any;
  subExpModel: any; // ngModel for Radio Button

  dialogResult = '';
  expList: any;
  subExpList: any;
  selectedImageResult: any;
  math: any;
  imageDescriptionNotNullVal = false;
  repList: any;

  public exps: any;

  constructor(
    private uploadService: UploadService,
    private experimentService: ExperimentService,
    private postProcessingQcService: PostProcessingQcService,
    private cogbytesService: CogbytesService,
    public dialog: MatDialog,
    private location: Location,
    private snackBar: MatSnackBar,
    private subexpDialogeService: SubExpDialogeService,
    private spinnerService: NgxSpinnerService,
    public dialogRef: MatDialog,
    public dialogRefPostProcessingResult: MatDialog) {

    this.hideSubExperiment = false;
    this.showSubExpTbl = false;
  }

  getSelectedExp(selValue: any) {

    return this.expList.find((x: any) => x.expID === selValue);
  }


  getSelectedSubExp(selValue: any) {
    return this.subExpList.find((x: any) => x.subExpID === selValue);
  }

  selectExpChange() {
    // this.outSelectChangeMethod.emit(this.selectedValue);
    this.selectedSubExpValue = null;
    const selectedExp: any = this.getSelectedExp(this.selectedExpValue);
    this.outSelectedExperiment.emit(selectedExp);
    this.getSubExpSelect(this.selectedExpValue);
  }

  selectedSubExpChangedRD(event: any) {

    this.selectedSubExpValue = event.value;
    this.selectSubExpChangeDD();

  }

  selectSubExpChangeDD() {
    this.subExpModel = this.selectedSubExpValue;
    const selectedSubExp: any = this.getSelectedSubExp(this.selectedSubExpValue);
    // console.log(selectedSubExp)
    if (selectedSubExp.imageIds != null) {
      this.selectedImageResult = selectedSubExp.imageIds.length;
    }

    this.outSelectedSubExperiment.emit(selectedSubExp);
  }

  // Creating Experiment
  openDialog(experiment: any): void {
    const dialogref = this.dialog.open(ExpDialogeComponent, {
      // height: '700px',
      width: '600px',
      data: { experimentObj: experiment }

    });

    dialogref.afterClosed().subscribe(result => {
      // console.log('the dialog was closed');
      this.dialogResult = result;
      this.getExpSelect();
    });
  }

  // Creating Sub experiment
  openDialogSubExp(subExperiment: any, expID: any): void {
    // console.log(SubExperiment);
    const experiment = this.getSelectedExp(expID);
    const dialogref = this.dialog.open(SubExpDialogeComponent, {
      width: '600px',
      data: { subexperimentObj: subExperiment, expObj: experiment} // change it for editing

    });

    dialogref.afterClosed().subscribe(result => {
      // console.log('the dialog was closed');

      this.getSubExpSelect(this.selectedExpValue);
    });
  }

  // Deleting Experiment
  openConfirmationDialog(expID: any) {
    const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
      disableClose: false
    });
    dialogRef.componentInstance.confirmMessage = CONFIRMDELETE;


    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.spinnerService.show();

        this.experimentService.deleteExperimentbyID(expID).map((res: any) => {


          this.spinnerService.hide();


          location.reload();

        }).subscribe();
      }
      // this.dialogRef = null;
      dialogRef.close();
    });
  }

  // Deleting Sub Experiment
  openSubExpConfirmationDialog(subExp: any) {
    const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
      disableClose: false
    });
    dialogRef.componentInstance.confirmMessage = CONFIRMDELETE;

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.spinnerService.show();
        this.subexpDialogeService.deleteSubExperimentbyID(subExp.subExpID).map((res: any) => {
          // location.reload()
          this.getSubExpSelect(this.selectedExpValue);
          this.spinnerService.hide();
          this.outSelectedSubExperiment.emit(undefined);

        }).subscribe();
      }
      // this.dialogRef = null;
      dialogRef.close();
    });
  }


  ngOnInit() {
    // this.imageDescriptionNotNull = false;
    this.getExpSelect();
    this.cogbytesService.getAllRepositories().subscribe((data: any) => {
      this.repList = data; /* console.log(this.repList);*/
    });

    if (this.hideSubExperiment == null) {
      this.hideSubExperiment = false;
    }
    if (this.showSubExpTbl == null) {
      this.showSubExpTbl = false;
    }

  }

  // Get List of all Experiment
  getExpSelect() {

    this.uploadService.getAllExperiment().subscribe((data: any) => {
      this.expList = data;
      // console.log(this.expList);
    });

  }

  // Get List of All sub Experiment
  getSubExpSelect(selectedExpVal: any) {
    this.subexpDialogeService.getAllSubExp(selectedExpVal).subscribe((data: any) => {
      this.subExpList = data.sort((a: any, b: any) => a.ageID - b.ageID);
    });
  }

  // Deleting Experiment
  delExp(expID: any) {

    this.openConfirmationDialog(expID);
  }

  // Deleting Sub Experiment
  delSubExp(subExp: any) {
    // console.log(subExp)
    this.openSubExpConfirmationDialog(subExp);

  }

  // function definition to ge the min age from the subexp list
  getMinAge() {

    const minValue = Math.min.apply(Math, this.subExpList.map(function (o: any) {
      return o.ageInMonth;
    }));
    // var age = JSON.parse(this.subExpList);
    // console.log(minValue);
    return minValue;

  }

  runPostQC(subExpObj: any) {

    this.spinnerService.show();

    this.postProcessingQcService.postProcessSubExperiment(subExpObj).subscribe((errorMessage: string) => {

      setTimeout(() => {
        this.spinnerService.hide();
      }, 500);

      if (errorMessage === '') {
        // this.GetAllSubExp();
      } else {
        this.snackBar.open(POSTPROCESSINGDONE, '', {
          duration: 2000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
        });

        this.getSubExpSelect(this.selectedExpValue);

      }

    });


  }

  // post processing result dialog
  openPostProcessingResult(subExpID: any) {

    this.postProcessingQcService.getPostProcessingResult(subExpID).subscribe((errorMessage: string) => {
      if (errorMessage === '') {
        errorMessage = PLEASERUNPREPROCESSING;
      }

      const dialogRefPostProcessingResult = this.dialog.open(GenericDialogComponent, {
        disableClose: false
      });
      dialogRefPostProcessingResult.componentInstance.message = errorMessage;
    });

  }

  getRepTitle(repGuid: any) {
    if (repGuid === '') {
      return 'N/A';
    }
    return this.repList[this.repList.map(function (x: any) {
      return x.repoLinkGuid;
    }).indexOf(repGuid)].title;
  }
}

