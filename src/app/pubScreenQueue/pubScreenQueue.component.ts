import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
// import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
// import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject ,  Subject } from 'rxjs';
// import { take, takeUntil } from 'rxjs/operators';
// import { ManageUserService } from '../services/manageuser.service';
// import { PagerService } from '../services/pager.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
// import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
// import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
// import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
// import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { CONFIRMACCEPTPAPERTOPUBSCREEN, CONFIRMREJECTPAPER, PAPERREJECTED, SUCCESSFULLYADDEDPUBLICATION  } from '../shared/messages';


@Component({
  selector: 'app-pubscreenqueue',
  templateUrl: './pubScreenQueue.component.html',
  styleUrls: ['./pubScreenQueue.component.scss']
  })
export class PubScreenQueueComponent implements OnInit {

  pubmedQueue: any;
  linkModel: any;

  isAdmin: boolean;
  isUser: boolean;
  isFullDataAccess: boolean;

  showGeneratedLink: any;

  private _onDestroy = new Subject<void>();

  constructor(public dialog: MatDialog,
    private authenticationService: AuthenticationService,
    private pubScreenService: PubScreenService,
    public dialogAuthor: MatDialog,
    private spinnerService: NgxSpinnerService,
    public dialogRefLink: MatDialog,
    public dialogRef: MatDialog) {

    this.isAdmin = false;
    this.isUser = false;
    this.isFullDataAccess = false;

  }

  ngOnInit() {

    this.isAdmin = this.authenticationService.isInRole('administrator');
    this.isUser = this.authenticationService.isInRole('user');
    this.isFullDataAccess = this.authenticationService.isInRole('fulldataaccess');

    this.pubScreenService.getPubmedQueue().subscribe(
      (data: any) => {
        this.pubmedQueue = data;
        // console.log(this.pubmedQueue);
      }
    );
  }

  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }


  acceptPub(row: any) {
    this.openAcceptDialog(row.pubmedID, row.doi);
  }

  rejectPub(row: any) {
    this.openRejectDialog(row.pubmedID, row.doi);
  }

  openAcceptDialog(pubmedID: any, doi: string) {
    const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
      disableClose: false
    });
    dialogRef.componentInstance.confirmMessage = CONFIRMACCEPTPAPERTOPUBSCREEN;

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.spinnerService.show();

        // console.log(pubmedID);

        this.pubScreenService.addQueuePaper(pubmedID, doi).subscribe(data => {
          alert(SUCCESSFULLYADDEDPUBLICATION);
          this.pubScreenService.getPubmedQueue().subscribe(
            (data2: any) => {
              this.pubmedQueue = data2;
              // console.log(this.pubmedQueue);

            }
          );
        });
        this.spinnerService.hide();

      }
      // this.dialogRef = null;
      dialogRef.close();
    });
  }

  openRejectDialog(pubmedID: any, doi: any) {
    const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
      disableClose: false
    });
    dialogRef.componentInstance.confirmMessage = CONFIRMREJECTPAPER;

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.spinnerService.show();

        // console.log(pubmedID);

        this.pubScreenService.rejectQueuePaper(pubmedID, doi).subscribe((result2: any) => {
          alert(PAPERREJECTED);
          this.pubScreenService.getPubmedQueue().subscribe(
            (data: any) => {
              this.pubmedQueue = data;
              // console.log(this.pubmedQueue);
              this.spinnerService.hide();
            }
          );
        });
      }
      // this.dialogRef = null;
      dialogRef.close();
    });
  }

  addCSVPapers() {
    this.pubScreenService.addCSVPapers().subscribe();
  }

  getLink(doi: string) {

    this.pubScreenService.getGuidByDoi(doi).subscribe((data: any) => {

      this.showGeneratedLink = true;
      const guid = data.paperLinkGuid;

      const dialogRefLink = this.dialog.open(NotificationDialogComponent, {
      });
      dialogRefLink.componentInstance.message = 'http://localhost:4200/pubScreen-edit?paperlinkguid=' + guid;



      // } else {
      //    console.log('Not Done!');

      // }
    });

  }

}


