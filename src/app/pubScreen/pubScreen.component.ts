import { Component, OnInit, Inject, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, Validators, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { NgModel } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Subject } from 'rxjs/Subject';
import { take, takeUntil } from 'rxjs/operators';
import { ManageUserService } from '../services/manageuser.service';
import { PagerService } from '../services/pager.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';


@Component({
    selector: 'app-pubScreen',
    templateUrl: './pubScreen.component.html',
    styleUrls: ['./pubScreen.component.scss']
})
export class PubScreenComponent implements OnInit {



    authorModel: any;
    titleModel: any;
    abstractModel: any;
    yearModel: any;
    keywordsModel: any;
    doiModel: any;
    paperTypeModel: any;
    cognitiveTaskModel: any;
    specieModel: any;
    sexModel: any;
    strainModel: any;
    diseaseModel: any;
    regionModel: any;
    subRegionModel: any;
    cellTypeModel: any;
    addingOptionModel: any;
    methodModel: any;
    neurotransmitterModel: any;
    yearFromSearchModel: any;
    yearToSearchModel: any;
    authorMultiSelect: any;
    subTaskModel: any;

    panelOpenState = false;

    // Definiing List Variables 
    paperTypeList: any;
    taskList: any;
    specieList: any;
    sexList: any;
    strainList: any;
    diseaseList: any;
    regionSubregionList: any
    regionList: any;
    subRegionList: any;
    cellTypeList: any;
    methodList: any;
    neurotransmitterList: any;
    authorList: any;
    authorList2: any;
    searchResultList: any;
    yearList: any;
    subTaskList: any;
    taskSubTaskList: any;
    paperInfoFromDoiList: any;
    checkYear: boolean;

    isAdmin: boolean;
    isUser: boolean;

    //yearFrom = new FormControl('', []);
    yearTo = new FormControl('', []);

    _pubSCreenSearch = new Pubscreen();

    public authorMultiFilterCtrl: FormControl = new FormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    dialogRef: MatDialogRef<DeleteConfirmDialogComponent>;

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService,
        public dialogAuthor: MatDialog,
        private spinnerService: Ng4LoadingSpinnerService,) { }

    ngOnInit() {

        this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe(data => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe(data => { this.taskList = data; });
        this.pubScreenService.getSpecie().subscribe(data => { this.specieList = data; });
        this.pubScreenService.getSex().subscribe(data => { this.sexList = data; });
        this.pubScreenService.getStrain().subscribe(data => { this.strainList = data; });
        this.pubScreenService.getDisease().subscribe(data => { this.diseaseList = data; });
        this.pubScreenService.getRegion().subscribe(data => { this.regionList = data; });
        this.pubScreenService.getCellType().subscribe(data => { this.cellTypeList = data; });
        this.pubScreenService.getMethod().subscribe(data => { this.methodList = data; });
        this.pubScreenService.getNeurotransmitter().subscribe(data => { this.neurotransmitterList = data; });

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isUser = this.authenticationService.isInRole("user");
        this.yearList = this.GetYear(1970).sort().reverse();

        

    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    // Function definition to get list of years
    GetYear(startYear) {
        var currentYear = new Date().getFullYear(), years = [];
        startYear = startYear || 1980;
        while (startYear <= currentYear) {
            years.push(startYear++);
        }
        return years;
    }


    GetAuthorList() {


        this.pubScreenService.getAuthor().subscribe(data => {
            this.authorList = data;

            // load the initial expList
            this.filteredAutorList.next(this.authorList.slice());

            this.authorMultiFilterCtrl.valueChanges
                .pipe(takeUntil(this._onDestroy))
                .subscribe(() => {
                    this.filterAuthor();
                });

        });

        return this.authorList;
    }

    // handling multi filtered Author list
    private filterAuthor() {
        if (!this.authorList) {
            return;
        }

        // get the search keyword
        let searchAuthor = this.authorMultiFilterCtrl.value;

        if (!searchAuthor) {
            this.filteredAutorList.next(this.authorList.slice());
            return;
        } else {
            searchAuthor = searchAuthor.toLowerCase();
        }

        // filter the Author
        this.filteredAutorList.next(
            this.authorList.filter(x => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
        );
    }

    selectedTaskChange(SelectedTask) {

        this.pubScreenService.getTaskSubTask().subscribe(data => {
            this.taskSubTaskList = data;
            //console.log(this.SubtaskList);
            var filtered = this.taskSubTaskList.filter(function (item) {
                return SelectedTask.indexOf(item.taskID) !== -1;
            });

            //console.log(filtered);
            this.subTaskList = JSON.parse(JSON.stringify(filtered));
        });


    }


    selectedRegionChange(SelectedRegion) {

        this.pubScreenService.getRegionSubRegion().subscribe(data => {
            this.regionSubregionList = data;
            //console.log(this.regionSubregionList);
            var filtered = this.regionSubregionList.filter(function (item) {
                return SelectedRegion.indexOf(item.rid) !== -1;
            });

            //console.log(filtered);
            this.subRegionList = JSON.parse(JSON.stringify(filtered));
        });

       
    }

    setDisabledValSearch() {

        return false;

    }

    // Opening Dialog for adding a new publication.
    openDialogAddPublication(Publication): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: {
                publicationObj: Publication,
                //sourceOptionModel: this.sourceOptionModel,

            }

        });

        dialogref.afterClosed().subscribe(result => {
            console.log('the dialog was closed');
            //this.DialogResult = result;
            this.GetAuthorList();
        });
    }

    // Edit publication
    openDialogEditPublication(Publication): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: { publicationObj: Publication }

        });

        dialogref.afterClosed().subscribe(result => {
            console.log('the dialog was closed');
            this.search();
        });
    }
    
    selectYearToChange(yearFromVal, yearToVal) {
        console.log(yearToVal)
        yearFromVal = yearFromVal === null ? 0 : yearFromVal;
        
        yearToVal < yearFromVal ? this.yearTo.setErrors({ 'incorrect': true }) : false;

    }

    
    getErrorMessageYearTo() {
        //return this.yearTo.getError('Year To should be greater than Year From');
        return 'Year To should be greater than Year From'
    }

    // Function definition for searching publications based on search criteria
    search() {

        this._pubSCreenSearch.authourID = this.authorModel;
        this._pubSCreenSearch.title = this.titleModel;
        this._pubSCreenSearch.keywords = this.keywordsModel;
        this._pubSCreenSearch.doi = this.doiModel;
        //this._pubSCreenSearch.year = this.yearModel;
        //this._pubSCreenSearch.years = this.yearSearchModel;
        this._pubSCreenSearch.paperTypeIdSearch = this.paperTypeModel;
        this._pubSCreenSearch.taskID = this.cognitiveTaskModel;
        this._pubSCreenSearch.specieID = this.specieModel;
        this._pubSCreenSearch.sexID = this.sexModel;
        this._pubSCreenSearch.strainID = this.strainModel;
        this._pubSCreenSearch.diseaseID = this.diseaseModel;
        this._pubSCreenSearch.regionID = this.regionModel;
        this._pubSCreenSearch.subRegionID = this.subRegionModel;
        this._pubSCreenSearch.cellTypeID = this.cellTypeModel;
        this._pubSCreenSearch.methodID = this.methodModel;
        this._pubSCreenSearch.transmitterID = this.neurotransmitterModel;
        this._pubSCreenSearch.yearFrom = this.yearFromSearchModel;
        this._pubSCreenSearch.yearTo = this.yearToSearchModel;
        this._pubSCreenSearch.subTaskID = this.subTaskModel;

        console.log(this._pubSCreenSearch);

        this.pubScreenService.searchPublication(this._pubSCreenSearch).subscribe(data => {

            this.searchResultList = data;
            console.log(this.searchResultList);
        });

    }

    // Deleting publication
    delPub(pubRow) {
        this.openConfirmationDialog(pubRow.id);
    }

    // Deleting Experiment
    openConfirmationDialog(pubID) {
        this.dialogRef = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Are you sure you want to delete?"


        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();

                console.log(pubID);

                this.pubScreenService.deletePublicationById(pubID).map(res => {


                    this.spinnerService.hide();


                    location.reload()

                }).subscribe();
            }
            this.dialogRef = null;
        });
    }




}


