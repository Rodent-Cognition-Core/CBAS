import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { AuthenticationService } from '../services/authentication.service';
import { CogbytesDialogueComponent } from '../cogbytesDialogue/cogbytesDialogue.component';
import { ExpDialogeComponent } from '../expDialoge/expDialoge.component';
import { AnimalDialogComponent } from '../animal-dialog/animal-dialog.component';
import { CogbytesUpload } from '../models/cogbytesUpload'
import { CogbytesService } from '../services/cogbytes.service'
import { AnimalService } from '../services/animal.service';
import { PagerService } from '../services/pager.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { map } from 'rxjs/operators';
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { CONFRIMREPOSITORYDETLETE } from '../shared/messages';


@Component({
    selector: 'app-cogbytes',
    templateUrl: './cogbytes.component.html',
    styleUrls: ['./cogbytes.component.scss']
})


export class CogbytesComponent implements OnInit, OnDestroy {

    readonly DATASET = 1;
    public uploadKey: number;
    panelOpenState: boolean;
    showGeneratedLink: boolean;
    public repModel: any;
    pager: any = {};
    pagedItems: any[];
    expfilter: any = '';
    experimentID: any

    // Definiing List Variables
    repList: any;
    uploadList: any;
    authorList: any;
    piList: any;
    AnimalList: any;

    _cogbytesUpload: CogbytesUpload;


    isAdmin: boolean;
    isUser: boolean;
    isFullDataAccess: boolean;

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(
        public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        //public dialogAuthor: MatDialog,
        private cogbytesService: CogbytesService,
        private animalService: AnimalService,
        private spinnerService: NgxSpinnerService,
        private pagerService: PagerService,
        public dialogRefLink: MatDialog,
        public dialogRef: MatDialog
    )
    {
        this.uploadKey = 0;
        this.panelOpenState = false;
        this.showGeneratedLink = false;
        this.isAdmin = false;
        this.isUser = false;
        this.isFullDataAccess = false;
        this.repModel = null;

        this._cogbytesUpload = {
            additionalNotes: '', ageID: [], dateUpload: '', description: '', fileTypeId: 0, genoID: [],
            housing: '', id: 0, imageDescription: '', imageIds: '', interventionDescription: '', isIntervention: false,
            lightCycle: '', name: '', numSubjects: 0, repId: 0, sexID: [], specieID: [], strainID: [], taskBattery: '', taskID: []
        }
        this.pagedItems = [];
        this.experimentID = 1;

        this.resetFormVals();
    }

    ngOnInit() {
        this.panelOpenState = false;


        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");

        if (this.isAdmin || this.isUser) {

        this.cogbytesService.getRepositories().subscribe((data : any) => { this.repList = data; /*console.log(data) */});
            this.GetAuthorList();
            this.GetPIList();
        }
    }


    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    resetFormVals() {

    }

    GetRepositories() {
        this.GetAuthorList();
        this.GetPIList();
        this.cogbytesService.getRepositories().subscribe((data: any) => { this.repList = data; });
        //return this.repList;
    }

    GetUploads( _event? : any) {
        if (this.repModel != null) {
            let repID = this.getRep().id;
            this.cogbytesService.getUploads(repID).subscribe((data: any) => { this.uploadList = data; });
        }
    }

    ClosePanel(_event? : any) {
        this.panelOpenState = false;
    }

    NewUpload(_event? : any) {
        this.GetUploads();
        this.ClosePanel();
    }


    //// Opening Dialog for adding a new repository.
    openDialogAddRepository(): void {
        let dialogref = this.dialog.open(CogbytesDialogueComponent, {
            height: '850px',
            width: '1200px',
            data: {
                repObj: null,
            }

        });

        dialogref.afterClosed().subscribe((_result : any) => {
            //console.log('the dialog was closed');
            this.repModel = null;
            this.GetRepositories();
           
        });
    }

    //// Opening Dialog for editing an existing repository.
    openDialogEditRepository(): void {
        let dialogref = this.dialog.open(CogbytesDialogueComponent, {
            height: '850px',
            width: '1200px',
            data: {
                repObj: this.repList[this.repList.map(function (x : any) { return x.id }).indexOf(this.repModel)],
            }

        });

        dialogref.afterClosed().subscribe((_result : any) => {
            //console.log('the dialog was closed');
            this.GetRepositories();
        });
    }

    // Delete File Dialog
    deleteRepository(_file? : any) {
        const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRef.componentInstance.confirmMessage = CONFRIMREPOSITORYDETLETE;

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.cogbytesService.deleteRepository(this.getRep().id).pipe(map((_res: any) => {

                })).subscribe();
                this.spinnerService.hide();
                this.repModel = null;
            }
            //this.dialogRef = null;
            dialogRef.close();
        });
    }

    createNewTouchscreenDataset(Experiment : any): void {
        const dialogRef = this.dialog.open(ExpDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: { experimentObj: Experiment }
        });

        dialogRef.afterClosed().subscribe((_result : any) => {
            this.getRep();
        })
    }

    addNewAnimal(animal?: any): void {
        if (typeof animal == 'undefined') {
            animal = null;
        }
        const dialogRef = this.dialog.open(AnimalDialogComponent, {
            height: '480px',
            width: '450px',
            data: { experimentId: this.experimentID, animalObj: animal}
        });

        dialogRef.afterClosed().subscribe((_result : any) => {
            this.GetAnimalInfo(this.experimentID);
        })
    }

    refreshTouchscreenDataset(): void {}

    getRepID() : number {
        return this.repList[this.repList.map(function (x: any) { return x.id }).indexOf(this.repModel)].id;
    }


    GetAuthorList() {


        this.cogbytesService.getAuthor().subscribe((data: any) => {
            this.authorList = data;
        });

        return this.authorList;
    }


    GetPIList() {

        this.cogbytesService.getPI().subscribe((data: any) => {
            this.piList = data;
        });

        return this.piList;
    }

    // Function for getting string of repository authors
    getRepAuthorString(rep: any) {
        let authorString: string = "";
        for (let id of rep.authourID) {
            let firstName: string = this.authorList[this.authorList.map(function (x : any) { return x.id }).indexOf(id)].firstName;
            let lastName: string = this.authorList[this.authorList.map(function (x: any) { return x.id }).indexOf(id)].lastName;
            authorString += firstName + "-" + lastName + ", ";
        }
        return authorString.slice(0, -2);
    }

    // Function for getting string of repository PIs
    getRepPIString(rep: any) {
        let PIString: string = "";
        for (let id of rep.piid) {
            PIString += this.piList[this.piList.map(function (x: any) { return x.id }).indexOf(id)].piFullName + ", ";
        }
        return PIString.slice(0, -2);
    }

    getRep(): any {
        return this.repList[this.repList.map(function (x: any) { return x.id }).indexOf(this.repModel)];
    }

    // Get Guid by RepoID
    getLink(repID: number) {

        this.cogbytesService.getGuidByRepID(repID).subscribe((data : any) => {

            this.showGeneratedLink = true;
            var guid = data.repoLinkGuid;

            const dialogRefLink = this.dialog.open(NotificationDialogComponent, {
            });
            dialogRefLink.componentInstance.message = "http://localhost:4200/comp-edit?repolinkguid=" + guid;

        });

    }

    getLinkURL() {
        return "http://localhost:4200/comp-edit?repolinkguid=" + this.getRep().repoLinkGuid;
    }

    GetAnimalInfo(selectedExperimentID: number) {
        this.animalService.getAnimalInfo(selectedExperimentID).subscribe(data => {
            this.AnimalList = data;
            this.setPage(1);
            //console.log(this.AnimalList)

        });

    }

    setPage(page: number) {


        var filteredItems = this.AnimalList;

        filteredItems = this.filterByString(this.AnimalList, this.expfilter);

        // get pager object from service
        this.pager = this.pagerService.getPager(filteredItems.length, page, 10);

        if (page < 1 || page > this.pager.totalPages) {
            this.pagedItems = [];
            return;
        }

        // get current page of items
        this.pagedItems = filteredItems.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }

    filterByString(data: any, s: string): any {
        s = s.trim();
        return data.filter((e : any) => e.userAnimalID.includes(s)); // || e.another.includes(s)
        //    .sort((a, b) => a.userFileName.includes(s) && !b.userFileName.includes(s) ? -1 : b.userFileName.includes(s) && !a.userFileName.includes(s) ? 1 : 0);
    }

}


