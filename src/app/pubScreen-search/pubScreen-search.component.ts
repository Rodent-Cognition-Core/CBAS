import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UntypedFormControl } from '@angular/forms';
import { ReplaySubject ,  Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';

@Component({
    selector: 'app-pubscreen-search',
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

    isAdmin: boolean;
    isFullDataAccess: boolean;
   

    _pubSCreenSearch: Pubscreen;

    public authorMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService, public dialogAuthor: MatDialog)
    {
        this.isAdmin = false;
        this.isFullDataAccess = false;
        this._pubSCreenSearch = {
            abstract: '', author: [], authorString: '', authourID: [], cellTypeID: [], celltypeOther: '',
            diseaseID: [], diseaseOther: '', doi: '', id: 0, keywords: '', methodID: [], methodOther: '',
            neurotransOther: '', paperType: '', paperTypeID: 0, paperTypeIdSearch: [], reference: '', regionID: [],
            search: '', sexID: [], source: '', specieID: [], specieOther: '', strainID: [], strainMouseOther: '',
            strainRatOther: '', subMethodID: [], subModelID: [], subRegionID: [], subTaskID: [], taskID: [], taskOther: '',
            title: '', transmitterID: [], year: '', yearFrom: 0, yearID: [], yearTo: 0
        }
    }

    ngOnInit() {

        this.GetAuthorList();
        this.pubScreenService.getPaperType().subscribe((data: any) => { this.paperTypeList = data; });
        this.pubScreenService.getTask().subscribe((data: any) => { this.taskList = data; });
        this.pubScreenService.getSpecie().subscribe((data: any) => { this.specieList = data; });
        this.pubScreenService.getSex().subscribe((data: any) => { this.sexList = data; });
        this.pubScreenService.getStrain().subscribe((data: any) => { this.strainList = data; });
        this.pubScreenService.getDisease().subscribe((data: any) => { this.diseaseList = data; });
        this.pubScreenService.getRegion().subscribe((data: any) => { this.regionList = data; });
        this.pubScreenService.getCellType().subscribe((data: any) => { this.cellTypeList = data; });
        this.pubScreenService.getMethod().subscribe((data: any) => { this.methodList = data; });
        this.pubScreenService.getNeurotransmitter().subscribe((data: any) => { this.neurotransmitterList = data; });

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
    }

    ngOnDestroy() {
        this._onDestroy.next();
        this._onDestroy.complete();
    }

    GetAuthorList() {


        this.pubScreenService.getAuthor().subscribe((data: any) => {
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
            this.authorList.filter((x : any) => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
        );
    }

    selectedRegionChange(SelectedRegion : any) {

        this.pubScreenService.getRegionSubRegion().subscribe((data: any) => {
            this.regionSubregionList = data;
            //console.log(this.regionSubregionList);
            var filtered = this.regionSubregionList.filter(function (item : any) {
                return SelectedRegion.indexOf(item.rid) !== -1;
            });

            //console.log(filtered);
            this.subRegionList = JSON.parse(JSON.stringify(filtered));
        });

        //console.log(this.subRegionList);
    }

    setDisabledValSearch() {
        
        return false;

    }

    openDialog(Publication : any): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '900px',
            width: '1100px',
            data: { publicationObj: Publication }

        });

        dialogref.afterClosed().subscribe((_result : any) => {
            //console.log('the dialog was closed');
            //this.DialogResult = result;
            //this.GetExpSelect();
        });
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
        //console.log(this._pubSCreenSearch);

        this.pubScreenService.searchPublication(this._pubSCreenSearch).subscribe(data => {

            this.searchResultList = data;
            //console.log(this.searchResultList);
        });

    }

}
