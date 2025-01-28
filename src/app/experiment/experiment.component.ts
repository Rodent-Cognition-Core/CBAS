import { Component, OnInit, NgModule, ViewChild } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
import { AuthenticationService } from '../services/authentication.service';
import { ExperimentService } from '../services/experiment.service';
import { UploadService } from '../services/upload.service';
import { PagerService } from '../services/pager.service';
// import { ExpDialogeComponent } from '../expDialoge/expDialoge.component';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';
import { Location } from '@angular/common';
// import { Experiment } from '../models/experiment';
import { NgxSpinnerService } from 'ngx-spinner';
// import * as _ from 'underscore';
import { CONFIRMDELETE } from '../shared/messages';

// declare global {
//    interface Navigator {
//        msSaveBlob: (blobOrBase64: Blob | string, filename: string) => void
//    }
// }

@Component({
  selector: 'app-experiment',
  templateUrl: './experiment.component.html',
  styleUrls: ['./experiment.component.scss'],

})
export class ExperimentComponent implements OnInit {

  uploadList: any;
  selectedSubExperiment: any;
  expfilter: any = '';
  // ExpModel: any;
  pager: any = {};
  pagerTblFile: any = {};
  pagedItems: any[];
  pagedItemsTblFile: any[];
  // @ViewChild(MatPaginator) paginator: MatPaginator;

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

  getUploadInfo(selectedSubExperimentID: number) {

    this.spinnerService.show();

    this.experimentService.getUploadInfoBySubExpId(selectedSubExperimentID).subscribe((data: any) => {
      this.uploadList = data;
      // console.log(this.UploadList);
      this.setPageTblFile(1);
      setTimeout(() => {
        this.spinnerService.hide();
      }, 500);

    },
    (error: any) => {
      setTimeout(() => {
        this.spinnerService.hide();
      }, 500);
    });

  }

  selectedExpChanged(experiment: any) {
    this.selectedSubExperiment = null;
  }

  selectedSubExpChanged(subExperiment: any) {
    this.selectedSubExperiment = subExperiment;
    if (subExperiment != null) {
      const subExpId = subExperiment.subExpID;
      this.getUploadInfo(subExpId);
    }

  }

  // Delete File Dialog
  openConfirmationDialogDelFile(uploadID: number, subExpID: number) {
    const dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
      disableClose: false
    });
    dialogRefDelFile.componentInstance.confirmMessage = CONFIRMDELETE;

    dialogRefDelFile.afterClosed().subscribe(result => {
      if (result) {
        this.spinnerService.show();
        this.experimentService.deleteFilebyID(uploadID).map((res: any) => {
          this.spinnerService.hide();
          // location.reload()
          this.getUploadInfo(subExpID);
        }).subscribe();
      }
      // this.dialogRefDelFile = null;
      dialogRefDelFile.close();
    });
  }


  // Delete File
  deleteFile(uploadID: any, subExpID: any) {
    this.openConfirmationDialogDelFile(uploadID, subExpID);
  }

  downloadFile(uploadId: number, userFileName: string): void {

    this.uploadService.downloadFile(uploadId)
      .subscribe(result => {

        // console.log('downloadresult', result);
        // let url = window.URL.createObjectURL(result);
        // window.open(url);

        const csvData = new Blob([result], { type: 'text/csv;charset=utf-8;' });
        let csvURL = null;
        // if (navigator.msSaveBlob) {
        //    csvURL = navigator.msSaveBlob(csvData, userFileName);
        // } else {
        //    csvURL = window.URL.createObjectURL(csvData);
        // }
        csvURL = window.URL.createObjectURL(csvData);
        const tempLink = document.createElement('a');
        tempLink.href = csvURL;
        tempLink.setAttribute('download', userFileName);
        document.body.appendChild(tempLink);
        tempLink.click();

      });
  }


  // Function defintion of pagination for Table File
  setPageTblFile(page: number) {
    // get pager object from service

    let filteredItems = this.uploadList;

    filteredItems = this.filterByString(this.uploadList, this.expfilter);

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

  filterByString(data: any, s: string): any {
    s = s.trim();
    return data.filter((e: any) => e.userFileName.toLowerCase().includes(s.toLowerCase()) ||
          e.userAnimalID.toLowerCase().includes(s.toLowerCase()) || e.sessionName.toLowerCase().includes(s.toLowerCase()));
  }


}
