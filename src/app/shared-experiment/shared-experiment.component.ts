import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ExpDialogeComponent } from '../expDialoge/expDialoge.component';
import { UploadService } from '../services/upload.service';
import { ExperimentService } from '../services/experiment.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { Location } from '@angular/common';
import { SubExpDialogeComponent } from '../sub-exp-dialoge/sub-exp-dialoge.component';
import { SubExpDialogeService } from '../services/subexpdialoge.service';
import { PostProcessingQcService } from '../services/postprocessingqc.service';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component'; 
import { MatSnackBar } from '@angular/material';
import * as _ from 'underscore';
import { NgxSpinnerService } from 'ngx-spinner';
import { CogbytesService } from '../services/cogbytes.service'
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { CONFIRMDELETE, PLEASERUNPREPROCESSING, POSTPROCESSINGDONE } from '../shared/messages';

@Component({
    selector: 'app-shared-experiment',
    templateUrl: './shared-experiment.component.html',
    styleUrls: ['./shared-experiment.component.scss'],
    providers: [UploadService, ExperimentService, SubExpDialogeService, PostProcessingQcService, CogbytesService]
})
export class SharedExperimentComponent implements OnInit {

    // @Input() selectedValue: string;
    //ngModel vars
    faQuestionCircle = faQuestionCircle;
    selectedExpValue: any;
    selectedSubExpValue: any;
    dialogRef: MatDialogRef<DeleteConfirmDialogComponent>;
    SubExpModel: any // ngModel for Radio Button
    dialogRefPostProcessingResult: MatDialogRef<GenericDialogComponent>;

    @Input() hideSubExperiment: boolean;
    @Input() showSubExpTbl: boolean;

    @Output() outSelectedExperiment: EventEmitter<string> = new EventEmitter();
    @Output() outSelectedSubExperiment: EventEmitter<string> = new EventEmitter();
    
    public Exps: any;
    DialogResult = "";
    expList: any;
    subExpList: any
    selectedImageResult: any;
    Math: any;
    imageDescriptionNotNullVal: boolean = false;
    repList: any;
    

    constructor(
        private uploadService: UploadService,
        private experimentService: ExperimentService,
        private postProcessingQcService: PostProcessingQcService,
        private cogbytesService: CogbytesService,
        public dialog: MatDialog,
        private location: Location,
        private snackBar: MatSnackBar,
        private subexpDialogeService: SubExpDialogeService,
        private spinnerService: NgxSpinnerService,) {

    }

    getSelectedExp(selValue) {
        
        return this.expList.find(x => x.expID === selValue);
    }


    getSelectedSubExp(selValue) {
        return this.subExpList.find(x => x.subExpID === selValue);
    }

    selectExpChange() {
        //this.outSelectChangeMethod.emit(this.selectedValue);
        this.selectedSubExpValue = null;
        var selectedExp: any;
        selectedExp = this.getSelectedExp(this.selectedExpValue);
        this.outSelectedExperiment.emit(selectedExp);
        this.GetSubExpSelect(this.selectedExpValue)
    }

    SelectedSubExpChangedRD(event) {

        this.selectedSubExpValue = event.value;
        this.selectSubExpChangeDD();
        
    }

    selectSubExpChangeDD() {
        this.SubExpModel = this.selectedSubExpValue;
        var selectedSubExp: any;
        selectedSubExp = this.getSelectedSubExp(this.selectedSubExpValue);
        //console.log(selectedSubExp)
        if (selectedSubExp.imageIds != null) {
            this.selectedImageResult = selectedSubExp.imageIds.length;
        }
        
        this.outSelectedSubExperiment.emit(selectedSubExp);
    }
    
    // Creating Experiment
    openDialog(Experiment): void {
        let dialogref = this.dialog.open(ExpDialogeComponent, {
            //height: '700px',
            width: '600px',
            data: { experimentObj: Experiment }

        });

        dialogref.afterClosed().subscribe(result => {
            //console.log('the dialog was closed');
            this.DialogResult = result;
            this.GetExpSelect();
        });
    }

    // Creating Sub experiment
    openDialogSubExp(SubExperiment, ExpID): void {
        //console.log(SubExperiment);
        var Experiment = this.getSelectedExp(ExpID)
        let dialogref = this.dialog.open(SubExpDialogeComponent, {
            width: '600px',
            data: { subexperimentObj: SubExperiment, expObj: Experiment} // change it for editing

        });

        dialogref.afterClosed().subscribe(result => {
            //console.log('the dialog was closed');

            this.GetSubExpSelect(this.selectedExpValue);
        });
    } 

    // Deleting Experiment
    openConfirmationDialog(expID) {
        this.dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = CONFIRMDELETE;


        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                this.experimentService.deleteExperimentbyID(expID).map(res => {

                    
                    this.spinnerService.hide();
                    

                    location.reload()

                }).subscribe();
            }
            this.dialogRef = null;
        });
    }

    // Deleting Sub Experiment
    openSubExpConfirmationDialog(subExp) {
        this.dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = CONFIRMDELETE

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.subexpDialogeService.deleteSubExperimentbyID(subExp.subExpID).map(res => {
                   // location.reload()
                    this.GetSubExpSelect(this.selectedExpValue);
                    this.spinnerService.hide();
                    this.outSelectedSubExperiment.emit(null);
                   
                }).subscribe();
            }
            this.dialogRef = null;
        });
    }


    ngOnInit() {
        //this.imageDescriptionNotNull = false;
        this.GetExpSelect();
        this.cogbytesService.getAllRepositories().subscribe(data => { this.repList = data; /*console.log(this.repList);*/ });

        if (this.hideSubExperiment == null) {
            this.hideSubExperiment = false;
        }
        if (this.showSubExpTbl == null) {
            this.showSubExpTbl = false;
        }

    }

    // Get List of all Experiment
    GetExpSelect() {

        this.uploadService.getAllExperiment().subscribe(data => {
            this.expList = data;
            //console.log(this.expList);
        });

    }

    // Get List of All sub Experiment
    GetSubExpSelect(selectedExpVal) {
        
        this.subexpDialogeService.getAllSubExp(selectedExpVal).subscribe(data => {
                        
            // this.subExpList = data;
            this.subExpList = _.sortBy(data, function (i) {

                               
                return i.ageID;                      
            
            });
                       
        });
                       
        
    }

    // Deleting Experiment
    delExp(expID) {

        this.openConfirmationDialog(expID);
    }

    // Deleting Sub Experiment
    delSubExp(subExp) {
        //console.log(subExp)
        this.openSubExpConfirmationDialog(subExp);

    }

    // function definition to ge the min age from the subexp list
    getMinAge() {

       var minValue = Math.min.apply(Math, this.subExpList.map(function (o) { return o.ageInMonth; }));
       //var age = JSON.parse(this.subExpList);
       // console.log(minValue);
       return minValue;
        
    }
    
    runPostQC(subExpObj) {

        this.spinnerService.show();

        this.postProcessingQcService.postProcessSubExperiment(subExpObj).subscribe(errorMessage => {

            setTimeout(() => {
                this.spinnerService.hide();
            }, 500);

            if (errorMessage == "") {
                // this.GetAllSubExp();
            } else {
                this.snackBar.open(POSTPROCESSINGDONE, "", {
                    duration: 2000,
                    horizontalPosition: 'right',
                    verticalPosition: 'top',
                });

                this.GetSubExpSelect(this.selectedExpValue);

            }

        });


    }

    // post processing result dialog
    openPostProcessingResult(subExpID) {
        
        this.postProcessingQcService.getPostProcessingResult(subExpID).subscribe(errorMessage => {
            if (errorMessage == "")
                errorMessage = PLEASERUNPREPROCESSING;

            this.dialogRefPostProcessingResult = this.dialog.open(GenericDialogComponent, {
                disableClose: false
            });
            this.dialogRefPostProcessingResult.componentInstance.message = errorMessage;
        });

    }

    getRepTitle(repGuid) {
        if (repGuid == "") {
            return "N/A";
        }
        return this.repList[this.repList.map(function (x) { return x.repoLinkGuid }).indexOf(repGuid)].title;
    }
}

