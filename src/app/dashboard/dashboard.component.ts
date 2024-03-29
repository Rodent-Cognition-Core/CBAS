import { Component, OnInit, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AuthenticationService } from '../services/authentication.service';
import { DashboardService } from '../services/dashboard.service';
import { UploadService } from '../services/upload.service';
import { AnimalDialogComponent } from '../animal-dialog/animal-dialog.component';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { PagerService } from '../services/pager.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { AnimalService } from '../services/animal.service';
import { ExperimentService } from '../services/experiment.service';
import { CONFIRMDELETE } from '../shared/messages';



@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

    experimentName: string;
    experimentID: number;
    uploadLogList: any;
    uploadErrorLogList: any;
    pager: any = {};
    pagedItems: any[];
    expfilter: any = '';
    dialogRefDelErrorList: MatDialogRef<DeleteConfirmDialogComponent>;
    dialogRefDelAnimal: MatDialogRef<DeleteConfirmDialogComponent>;
    dialogRefDelFile: MatDialogRef<DeleteConfirmDialogComponent>;

    constructor(private dashboardService: DashboardService, private experimentService: ExperimentService,
        private uploadService: UploadService,
        private authenticationService: AuthenticationService, private pagerService: PagerService,
        private spinnerService: Ng4LoadingSpinnerService, private animalService: AnimalService,
        public dialog: MatDialog) { }


    ngOnInit() {

    }

    // Get list major errors
    GetUploadLogList(selectedExperimentID) {
        this.dashboardService.getUploadInfoById(selectedExperimentID).subscribe(data => {
            this.uploadLogList = data;
            this.setPage(1);

            console.log(this.uploadLogList);

        });


    }

    // Get list of minor errors
    GetUploadErrorLogList(selectedExperimentID) {
        this.dashboardService.getUploadErrorLogById(selectedExperimentID).subscribe(data => {
            this.uploadErrorLogList = data;

        });

    }

    SelectedExpChanged(experiment) {
        this.GetUploadLogList(experiment.expID);
        this.GetUploadErrorLogList(experiment.expID);
        this.experimentName = experiment.expName;
        this.experimentID = experiment.expID;

    }
    
    // Delete Animal Dialog
    openConfirmationDialogDelAnimal(animalID, expId) {
        this.dialogRefDelAnimal = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRefDelAnimal.componentInstance.confirmMessage = CONFIRMDELETE

        this.dialogRefDelAnimal.afterClosed().subscribe(result => {
            if (result) {
                this.animalService.deleteAnimalbyID(animalID).map(res => {
                    //location.reload()
                    this.GetUploadLogList(this.experimentID);
                }).subscribe();
            }
            this.dialogRefDelAnimal = null;
        });
    }


    // Delete animal 
    delAnimal(animalID, expId) {
        this.openConfirmationDialogDelAnimal(animalID, expId);
    }
    
    DownloadFile(uploadId: number, userFileName: string): void {

        this.uploadService.downloadFile(uploadId)
            .subscribe(result => {

                // console.log('downloadresult', result);
                //let url = window.URL.createObjectURL(result);
                //window.open(url);

                var csvData = new Blob([result], { type: 'text/csv;charset=utf-8;' });
                var csvURL = null;
                if (navigator.msSaveBlob) {
                    csvURL = navigator.msSaveBlob(csvData, userFileName);
                } else {
                    csvURL = window.URL.createObjectURL(csvData);
                }
                var tempLink = document.createElement('a');
                tempLink.href = csvURL;
                tempLink.setAttribute('download', userFileName);
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

    // Function definition for handling missing animal info (e.g. Age, Sex, Strain, and Genotype)
    ResolveIssue(animal: number, uploadId: number): void {
        console.log(animal);
        let dialogref = this.dialog.open(AnimalDialogComponent, {
            height: '480px',
            width: '450px',
            data: { experimentId: this.experimentID, animalObj: animal }

        });

        // When "Resolve Issue" button is clicked, the modal should open and ask for the identifier fields.Once the Submit button in this Modal is clicked, the following steps should be considered:
        //   Update Table Animal: done
        //   Update Dashboard table in Dashboard page
        //   Find the PrimaryKey of UserAnimalID and find it in Upload table and update the following fields:
        //   Clean ErrorMessage
        //   Set IsIdentifierPassed to TRUE
        //   Set IsUploaded to True and Insert the XML data to SessionInfo
        //   Set / update DateUpload
        // **Find AnimalID in tbl Uploal and all of them should be found, updated in this table and thier data are sent to tbl sessioninfo annd markerinfo

        dialogref.afterClosed().subscribe(result => {
            if (result == true) {

                console.log('show spinner');
                this.spinnerService.show();

                this.uploadService.setUploadAsResolved(uploadId).subscribe(data => {

                    this.GetUploadLogList(this.experimentID);

                    setTimeout(() => {
                        this.spinnerService.hide();
                    }, 500);

                    //this.uploadLogList = this.uploadLogList.filter(obj => obj.uploadID !== uploadId);
                    //For updating dashboards, all the files with the same animal ID & Same error should get disapeared from the uploadLogList

                },
                    error => {
                        if (error.error instanceof Error) {
                            console.log('An error occurred:', error.error.message);
                        } else {
                            console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                        }
                    });

            }
            // if after close is false and there is not need to do resolve issue
            // just databind the table again
            else {
                this.GetUploadLogList(this.experimentID);
            }
        });

    } // End of ResolveIssue Function


    // Function defintion to add pagination to tabl Uploadlist (minor errors)
    setPage(page: number) {
        var filteredItems = this.uploadLogList;

        filteredItems = this.filterByString(this.uploadLogList, this.expfilter);

        // get pager object from service
        this.pager = this.pagerService.getPager(filteredItems.length, page, 10);

        if (page < 1 || page > this.pager.totalPages) {
            this.pagedItems = [];
            return;
        }

        // get current page of items
        this.pagedItems = filteredItems.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }

    search(): any {
        this.setPage(1);
    }

    filterByString(data, s): any {
        s = s.trim();
        return data.filter(e => e.userFileName.includes(s) || e.userAnimalID.includes(s)); // || e.another.includes(s)
        //    .sort((a, b) => a.userFileName.includes(s) && !b.userFileName.includes(s) ? -1 : b.userFileName.includes(s) && !a.userFileName.includes(s) ? 1 : 0);
    }

    //****************Clear uploadErrorLogList********************************************

    openConfirmationDialogDelErrorList(expID) {
        this.dialogRefDelErrorList = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRefDelErrorList.componentInstance.confirmMessage = CONFIRMDELETE

        this.dialogRefDelErrorList.afterClosed().subscribe(result => {
            if (result) {

                this.dashboardService.clearUploadLogTblbyExpID(expID).map(res => {
                    //location.reload()
                    this.uploadErrorLogList = [];
                    this.GetUploadLogList(expID);

                }).subscribe();

            }
            this.dialogRefDelErrorList = null;
        });
    }

    // ******************Clear UploadErrorList Table*****************

    ClearUploadErrorLogList(expID) {

        this.openConfirmationDialogDelErrorList(expID);

    }

    // ****************** Delete file for both QC error and missing animal info ***************
    // Delete File 
    deleteFile(uploadID) {
        this.openConfirmationDialogDelFile(uploadID);
    }

    // Delete File Dialog
    openConfirmationDialogDelFile(uploadID) {
        this.dialogRefDelFile = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRefDelFile.componentInstance.confirmMessage = CONFIRMDELETE;

        this.dialogRefDelFile.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.experimentService.deleteFilebyID(uploadID).map(res => {
                    this.spinnerService.hide();
                    //location.reload()
                    this.GetUploadLogList(this.experimentID);
                }).subscribe();
            }
            this.dialogRefDelFile = null;
        });
    }


}



