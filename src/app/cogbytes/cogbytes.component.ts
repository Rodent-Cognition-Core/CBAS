import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subject, forkJoin } from 'rxjs';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { AuthenticationService } from '../services/authentication.service';
import { CogbytesDialogueComponent } from '../cogbytesDialogue/cogbytesDialogue.component';
import { ExpDialogeComponent } from '../expDialoge/expDialoge.component';
import { AnimalDialogComponent } from '../animal-dialog/animal-dialog.component';
import { CogbytesUpload } from '../models/cogbytesUpload';
import { CogbytesService } from '../services/cogbytes.service';
import { AnimalService } from '../services/animal.service';
import { PagerService } from '../services/pager.service';
import { UploadService } from '../services/upload.service';
import { ExperimentService } from '../services/experiment.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { map } from 'rxjs/operators';
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { CONFRIMREPOSITORYDETLETE, CONFIRMDELETE, PLEASERUNPREPROCESSING, POSTPROCESSINGDONE } from '../shared/messages';
import { SubExpDialogeComponent } from '../sub-exp-dialoge/sub-exp-dialoge.component';
import { UploadComponent } from '../upload/upload.component';
import { UploadResultDialogComponent } from '../upload-result-dialog/upload-result-dialog.component';
import { PostProcessingQcService } from '../services/postprocessingqc.service';
import { SubExpDialogeService } from '../services/subexpdialoge.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';
import { Experiment } from '../models/experiment';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';


@Component({
    selector: 'app-cogbytes',
    templateUrl: './cogbytes.component.html',
    styleUrls: ['./cogbytes.component.scss']
})


export class CogbytesComponent implements OnInit, OnDestroy, AfterViewInit {

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
    experiments: any[] = [];

    _cogbytesUpload: CogbytesUpload;


    isAdmin: boolean;
    isUser: boolean;
    isFullDataAccess: boolean;

    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    // For animal table
    displayedColumns: string[] = ['select', 'animalId', 'sex', 'strain', 'genotype', 'actions'];
    dataSource = new MatTableDataSource<any>();
    selection = new SelectionModel<any>(true, []);

    @ViewChild(MatPaginator) paginator!: MatPaginator;

    constructor(
        public dialog: MatDialog,
        public dialogRefDelAnimal: MatDialog,
        private authenticationService: AuthenticationService,
        //public dialogAuthor: MatDialog,
        private cogbytesService: CogbytesService,
        private animalService: AnimalService,
        private experimentService: ExperimentService,
        private spinnerService: NgxSpinnerService,
        private pagerService: PagerService,
        public dialogRefLink: MatDialog,
        public dialogRef: MatDialog,
        private postProcessingQcService: PostProcessingQcService,
        private subexpDialogeService: SubExpDialogeService,
        private snackBar: MatSnackBar,
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
            additionalNotes: '', dateUpload: '', description: '', fileTypeId: 0, genoID: [],
            housing: '', id: 0, imageDescription: '', imageIds: '', interventionDescription: '', isIntervention: false,
            lightCycle: '', name: '', numSubjects: 0, repId: 0, sexID: [], specieID: [], strainID: [], taskBattery: '', taskID: [], startAge: null, endAge: null
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

    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
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
            const rep = this.getRep();
            this.GetAnimalInfo(rep.id);
            this.experiments = rep?.experiment || [];
            if (this.experiments.length > 0) {
                const subExpObservables = this.experiments.map(exp =>
                    this.subexpDialogeService.getAllSubExp(exp.expID).pipe(
                        map((subExperimentArray: any[]) => ({
                            ...exp,
                            subexperiment: subExperimentArray
                        }))
                )
             );
             
             forkJoin(subExpObservables).subscribe(
                (updatedExp: any[]) => {
                    this.experiments = updatedExp;
                }
             )
            }
            if (rep) {
                this.cogbytesService.getUploads(rep.id).subscribe((data: any) => { this.uploadList = data; });
            }
        } else {
            // When no repository is selected, clear the lists.
            this.uploadList = [];
            this.experiments = [];
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
            data: { experimentObj: Experiment,
                    repoData: this.getRep()
             }
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
            const rep = this.getRep();
            if (rep) {
                this.GetAnimalInfo(rep.id);
            }
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
        if (!this.repList || !this.repModel) {
            return null;
        }
        return this.repList.find((x: any) => x.id === this.repModel);
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

    GetAnimalInfo(repID: number) {
        this.animalService.getAnimalInfoRepository(repID).subscribe((data: any) => {
            this.AnimalList = data;
            this.dataSource.data = data;
            this.dataSource.paginator = this.paginator;
        });
    }

    getRepCognitiveTaskString(repID: number) {

    }

    getRepSpeciesString(repID: number) {
        
    }

    getRepSexString(repID: number) {
        
    }

    getRepStrainString(repID: number) {
        
    }

    getRepGenotypeString(repID: number) {
        
    }

    getRepAgeString(repID: number) {
        return { startAge: 0, endAge: 0 };
    }

    getRepDiseaseModelString(repID: number) {
        
    }

    getRepSubModelString(repID: number) {
        
    }

    getRepRegionString(repID: number) {
        
    }

    getRepSubRegionString(repID: number) {
        
    }

    getRepCellTypeString(repID: number) {
        
    }

    getRepMethodString(repID: number) {
        
    }

    getRepSubMethodString(repID: number) {
        
    }

    getRepNeurotransmitterString(repID: number) {
        
    }

    applyFilter(filterValue: string) {
        this.dataSource.filter = filterValue.trim().toLowerCase();
    }

    /** Whether the number of selected elements matches the total number of rows. */
    isAllSelected() {
        const numSelected = this.selection.selected.length;
        const numRows = this.dataSource.data.length;
        return numSelected === numRows;
    }

    /** Selects all rows if they are not all selected; otherwise clear selection. */
    masterToggle() {
        this.isAllSelected() ?
            this.selection.clear() :
            this.dataSource.data.forEach(row => this.selection.select(row));
    }

    deleteSelectedAnimals() {
        const numSelected = this.selection.selected.length;
        if (numSelected === 0) { return; }

        const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRef.componentInstance.confirmMessage = `Are you sure you want to delete ${numSelected} animal(s)?`;

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                const deletionObservables = this.selection.selected.map(animal =>
                    this.animalService.deleteAnimalbyID(animal.animalID)
                );

                forkJoin(deletionObservables).subscribe({
                    next: () => {
                        this.snackBar.open('Selected animals deleted successfully.', 'Close', { duration: 3000 });
                        const rep = this.getRep();
                        if (rep) {
                            this.GetAnimalInfo(rep.id);
                        }
                        this.selection.clear();
                    },
                    error: (err) => {
                        this.spinnerService.hide();
                        this.snackBar.open('An error occurred while deleting animals.', 'Close', { duration: 3000 });
                        console.error(err);
                    },
                    complete: () => {
                        this.spinnerService.hide();
                    }
                });
            }
        });
    }

    duplicateSelectedAnimals() {
        const selectedAnimals = this.selection.selected;
        if (selectedAnimals.length === 0) {
            return;
        }

        selectedAnimals.forEach(animal => {
            const newAnimal = { ...animal };
            delete newAnimal.animalID; // No ID means it's a new animal
            newAnimal.userAnimalID = `Copy of ${animal.userAnimalID}`; // Suggest a new name
            this.addNewAnimal(newAnimal);
        });

        this.selection.clear();
    }

    delAnimal(animalID: number, expId: number) {
        this.openConfirmationDialogDelAnimal(animalID, expId);
    }
    
    openConfirmationDialogDelAnimal(animalID: number, expId: number) {
        const dialogRefDelAnimal = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRefDelAnimal.componentInstance.confirmMessage = CONFIRMDELETE;



        dialogRefDelAnimal.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.animalService.deleteAnimalbyID(animalID).pipe(map((res) => {

                    const rep = this.getRep();
                    if (rep) {
                        this.GetAnimalInfo(rep.id);
                    }
                    
                    this.spinnerService.hide();

                })).subscribe();
            }
            //this.dialogRefDelAnimal = null;
            dialogRefDelAnimal.close();
        });
    }

    createSubExperiment(exp: any): void {
        const dialogRef = this.dialog.open(SubExpDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: { expObj: exp }
        });

        dialogRef.afterClosed().subscribe(result => {
            this.GetRepositories();
        });
    }

    openUploadDialog(exp: any, subExp: any): void {
        // const exp = this.getRep().experiment.find((e: any) => e.subExperiments.some((se: any) => se.subExpID === subExp.subExpID));
        const dialogRef = this.dialog.open(UploadComponent, {
            height: '800px',
            width: '1000px',
            data: {
                experiment: exp,
                subExperiment: subExp
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            // Handle dialog close
        });
    }

    openTimeSeriesUploadDialog(exp: any, subExp: any): void {
        const dialogRef = this.dialog.open(NotificationDialogComponent, {
        });
        dialogRef.componentInstance.message = "Time series upload is not yet implemented.";
    }

    openUploadResultDialog(subExp: any): void {
        // This is a placeholder. In a real scenario, you would fetch the
        // latest upload result for the sub-experiment.
        const mockUploadResult: any[] = [];

        const dialogRef = this.dialog.open(UploadResultDialogComponent, {
            height: '900px',
            width: '850px',
            data: {
                uploadResult: mockUploadResult,
                experimentName: this.getRep().title
            }
        });
    }

    openConfirmationDialog(expID : any) {
        const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRef.componentInstance.confirmMessage = CONFIRMDELETE;


        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                this.experimentService.deleteExperimentbyID(expID).pipe(map((_res : any) => {

                    
                    this.spinnerService.hide();
                    

                    location.reload()

                })).subscribe();
            }
            //this.dialogRef = null;
            dialogRef.close();
        });
    }

    getRepTitle(repGuid : any) {
        console.log(repGuid);
        if (repGuid == "") {
            return "N/A";
        }
        return this.repList[this.repList.map(function (x: any) { return x.repoLinkGuid }).indexOf(repGuid)].title;
    }

    runPostQC(subExpObj : any) {
        this.spinnerService.show();
        this.postProcessingQcService.postProcessSubExperiment(subExpObj).subscribe((errorMessage : string) => {
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
                this.GetRepositories();
            }
        });
    }

    openPostProcessingResult(subExpID : any) {
        this.postProcessingQcService.getPostProcessingResult(subExpID).subscribe((errorMessage : string) => {
            if (errorMessage == "")
                errorMessage = PLEASERUNPREPROCESSING;

            const dialogRefPostProcessingResult = this.dialog.open(GenericDialogComponent, {
                disableClose: false
            });
            dialogRefPostProcessingResult.componentInstance.message = errorMessage;
        });
    }

    openDialogSubExp(SubExperiment : any, ExpID : any): void {
        //console.log(SubExperiment);
        var Experiment = this.getRep().experiment.find((x : any) => x.expID === ExpID);
        let dialogref = this.dialog.open(SubExpDialogeComponent, {
            width: '600px',
            data: { subexperimentObj: SubExperiment, expObj: Experiment} // change it for editing

        });

        dialogref.afterClosed().subscribe((_result : any) => {
            //console.log('the dialog was closed');
            this.GetRepositories();
        });
    } 

    delSubExp(subExp : any) {
        //console.log(subExp)
        this.openSubExpConfirmationDialog(subExp);
    }

    openSubExpConfirmationDialog(subExp : any) {
        const dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        dialogRef.componentInstance.confirmMessage = CONFIRMDELETE

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.subexpDialogeService.deleteSubExperimentbyID(subExp.subExpID).pipe(map((_res : any) => {
                   // location.reload()
                    this.GetRepositories();
                    this.spinnerService.hide();
                })).subscribe();
            }
            //this.dialogRef = null;
            dialogRef.close();
        });
    }
}