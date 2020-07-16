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

@Component({
    selector: 'app-pubScreen-search',
    templateUrl: './pubScreen-search.component.html',
    styleUrls: ['./pubScreen-search.component.scss']
})
export class PubScreenSearchComponent implements OnInit {

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
    yearSearchModel: any
    authorMultiSelect: any;

    _pubSCreenSearch = new Pubscreen();

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
    paperInfoFromDoiList: any;
    filteredAutorList: any;

    constructor(public dialog: MatDialog,
        private pubScreenService: PubScreenService, public dialogAuthor: MatDialog)
    { }

    ngOnInit() {

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

        console.log(this.subRegionList);
    }

    setDisabledValSearch() {
        
        return false;

    }

    // Function definition for searching publications based on search criteria
    search() {

        this._pubSCreenSearch.title = this.titleModel;
        this._pubSCreenSearch.abstract = this.abstractModel;
        this._pubSCreenSearch.keywords = this.keywordsModel;
        this._pubSCreenSearch.doi = this.doiModel;
        this._pubSCreenSearch.year = this.yearModel;
        this._pubSCreenSearch.years = this.yearSearchModel;
        this._pubSCreenSearch.authourID = this.authorModel;
        this._pubSCreenSearch.paperTypeID = this.paperTypeModel;
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
        console.log(this._pubSCreenSearch);

        this.pubScreenService.searchPublication(this._pubSCreenSearch).subscribe(data => {

            this.searchResultList = data;
            console.log(this.searchResultList);
        });

    }

}
