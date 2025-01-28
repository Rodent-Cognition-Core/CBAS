import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
// import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-upload-result-dialog',
  templateUrl: './upload-result-dialog.component.html',
  styleUrls: ['./upload-result-dialog.component.scss']
})
export class UploadResultDialogComponent implements OnInit {

  uploadResultList: any;
  expName: string;

  constructor(public thisDialogRef: MatDialogRef<UploadResultDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.expName = '';
  }



  ngOnInit() {
    this.uploadResultList = this.data.uploadResult;
    this.expName = this.data.experimentName;
  }

  onCloseCancel(): void {

    this.thisDialogRef.close('Cancel');
  }

  downloadFile() {

  }

  resolveIssue() {

  }


}
