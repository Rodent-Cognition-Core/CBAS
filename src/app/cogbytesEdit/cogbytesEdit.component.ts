import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import {
    MatDialog, MatDialogRef,
    MAT_DIALOG_DATA
} from '@angular/material/dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
// import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject ,  Subject } from 'rxjs';
import { CogbytesService } from '../services/cogbytes.service';
// import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
// import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { LoadingService } from '../services/loadingservice'


@Component({
  selector: 'app-cogbytesedit',
  templateUrl: './cogbytesEdit.component.html',
  styleUrls: ['./cogbytesEdit.component.scss']
  })
export class CogbytesEditComponent implements OnInit {


  repObj: any;
  repoList: any;
  uploadObj: any;



  repoLinkGuid: string;
  isAdmin: boolean;
  isFullDataAccess: boolean;
  panelOpenState: boolean;


  public authorMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
  public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(public dialog: MatDialog,
    private authenticationService: AuthenticationService,
    private cogbytesService: CogbytesService,
    private route: ActivatedRoute,
    private spinnerService: LoadingService,
    public dialogAuthor: MatDialog) {

    // this.isLoaded = false;
    this.repoLinkGuid = '';
    this.isAdmin = false;
    this.isFullDataAccess = false;
    this.panelOpenState = false;
    this.route.queryParams.subscribe(params => {
      this.repoLinkGuid = params['repolinkguid'].split(' ')[0];

      this.getDataByLinkGuid(this.repoLinkGuid);
    });

  }

  ngOnInit() {

    this.isAdmin = this.authenticationService.isInRole('administrator');
    this.isFullDataAccess = this.authenticationService.isInRole('fulldataaccess');
  }

  getDataByLinkGuid(repoLinkGuid: string) {
    this.spinnerService.show();

    this.cogbytesService.getDataByLinkGuid(repoLinkGuid).subscribe((data: any) => {

      this.repObj = data[0];
      this.repoList = data;
      // console.log(this.repObj);
      // console.log(this.repoList);

    });

    this.spinnerService.hide();

  }

  // Downloading file
  downloadFile(file: any): void {

    const path = file.permanentFilePath + '\\' + file.sysFileName;
    this.cogbytesService.downloadFile(path)
      .subscribe(result => {

        // console.log('downloadresult', result);
        // let url = window.URL.createObjectURL(result);
        // window.open(url);

        const fileData = new Blob([result]);
        let csvURL = null;

        csvURL = window.URL.createObjectURL(fileData);

        const tempLink = document.createElement('a');
        tempLink.href = csvURL;
        tempLink.setAttribute('download', file.userFileName);
        document.body.appendChild(tempLink);
        tempLink.click();

      },
      error => {
        if (error.error instanceof Error) {
          console.log('An error occurred:', error.error.message);
        } else {
          console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
        }
      });
  }






}
