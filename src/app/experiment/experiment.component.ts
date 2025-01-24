import { Component, OnInit, NgModule, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthenticationService } from '../services/authentication.service';
import { ExperimentService } from '../services/experiment.service';
import { UploadService } from '../services/upload.service';
import { PagerService } from '../services/pager.service';
//import { ExpDialogeComponent } from '../expDialoge/expDialoge.component';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';
import { Location } from '@angular/common';
//import { Experiment } from '../models/experiment';
import { NgxSpinnerService } from 'ngx-spinner';
//import * as _ from 'underscore';
import { CONFIRMDELETE } from '../shared/messages';

//declare global {
//    interface Navigator {
//        msSaveBlob: (blobOrBase64: Blob | string, filename: string) => void
//    }
//}

@Component({
    selector: 'app-experiment',
    templateUrl: './experiment.component.html',
    styleUrls: ['./experiment.component.scss'],

})
export class ExperimentComponent implements OnInit {

    UploadList: any;
    selectedSubExperiment: any;
    expfilter: any = '';
    // ExpModel: any;
    pager: any = {};
    pagerTblFile: any = {};
    pagedItems: any[];
    pagedItemsTblFile: any[];
    //@ViewChild(MatPaginator) paginator: MatPaginator;

    constructor(private experimentService: ExperimentService, public dialog: MatDialog,
        private authenticationService: AuthenticationService, private location: Location,
        private uploadService: UploadService,
        private spinnerService: NgxSpinnerService,
        private pagerService: PagerService,
        public dialogRefDelFile: MatDialog,
        public dialogRefDelExp: MatDialog,
        public dialogRefPostProcessingResult: MatDialog
    ) {
        this.pagedItems = [];
        this.pagedItemsTblFile = [];

    }

    ngOnInit() {
        // this.GetAllExp();

    }

    GetUploadInfo(selectedSubExperimentID: number) {

        this.spinnerService.show();

        this.experimentService.getUploadInfoBySubExpId(selectedSubExperimentID).subscribe((data : any) => {
            this.UploadList = data;
            //console.log(this.UploadList);
            this.setPageTblFile(1);
            setTimeout(() => {
                this.spinnerService.hide();
            }, 500);

        },
            (error : any) => {
                setTimeout(() => {
                    this.spinnerService.hide();
                }, 500);
            });

    }

    SelectedExpChanged(experiment : any) {
        this.selectedSubExperiment = null;
    }

    SelectedSubExpChanged(subExperiment : any) {
        this.selectedSubExperiment = subExperiment;
        if (subExperiment != null) {
            var subExpId = subExperiment.subExpID;
            this.GetUploadInfo(subExpId);
        }

    }

    // Delete File Dialog
    openConfirmationDialogDelFile(uploadID : number, subExpID : number) {
        const dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRefDelFile.componentInstance.confirmMessage = CONFIRMDELETE

        dialogRefDelFile.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.experimentService.deleteFilebyID(uploadID).map((res : any) => {
                    this.spinnerService.hide();
                    //location.reload()
                    this.GetUploadInfo(subExpID);
                }).subscribe();
            }
            //this.dialogRefDelFile = null;
            dialogRefDelFile.close();
        });
    }


    // Delete File 
    deleteFile(uploadID : any, subExpID : any) {
        this.openConfirmationDialogDelFile(uploadID, subExpID);
    }

    DownloadFile(uploadId: number, userFileName: string): void {

        this.uploadService.downloadFile(uploadId)
            .subscribe(result => {

                // console.log('downloadresult', result);
                //let url = window.URL.createObjectURL(result);
                //window.open(url);

                var csvData = new Blob([result], { type: 'text/csv;charset=utf-8;' });
                var csvURL = null;
                //if (navigator.msSaveBlob) {
                //    csvURL = navigator.msSaveBlob(csvData, userFileName);
                //} else {
                //    csvURL = window.URL.createObjectURL(csvData);
                //}
                csvURL = window.URL.createObjectURL(csvData);
                var tempLink = document.createElement('a');
                tempLink.href = csvURL;
                tempLink.setAttribute('download', userFileName);
                document.body.appendChild(tempLink);
                tempLink.click();

            })
    }


    // Function defintion of pagination for Table File
    setPageTblFile(page: number) {
        // get pager object from service

        var filteredItems = this.UploadList;

        filteredItems = this.filterByString(this.UploadList, this.expfilter);

        this.pagerTblFile = this.pagerService.getPager(filteredItems.length, page, 10);

        if (page < 1 || page > this.pagerTblFile.totalPages) {
            this.pagedItemsTblFile = [];
            return;
        }

        // get current page of items
        this.pagedItemsTblFile = filteredItems.slice(this.pagerTblFile.startIndex, this.pagerTblFile.endIndex + 1);
    }

    search(): any {
        this.setPageTblFile(1);
    }

    filterByString(data : any, s : string): any {
        s = s.trim();
        return data.filter((e : any) => e.userFileName.toLowerCase().includes(s.toLowerCase()) || e.userAnimalID.toLowerCase().includes(s.toLowerCase()) || e.sessionName.toLowerCase().includes(s.toLowerCase())); // || e.another.includes(s)
        //.sort((a, b) => a.userFileName.includes(s) && !b.userFileName.includes(s) ? -1 : b.userFileName.includes(s) && !a.userFileName.includes(s) ? 1 : 0);
    }
  

}
