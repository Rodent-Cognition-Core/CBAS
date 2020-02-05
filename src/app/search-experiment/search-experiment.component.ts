import { Component, OnInit, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { SearchExperimentService } from '../services/searchexperiment.service';
import { PagerService } from '../services/pager.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { IdentityService } from '../services/identity.service';

@Component({
    selector: 'app-search-experiment',
    templateUrl: './search-experiment.component.html',
    styleUrls: ['./search-experiment.component.scss']
})
export class SearchExperimentComponent implements OnInit {

    pagedItems: any[];
    searchList: any;
    expfilter: any = '';
    pager: any = {};

    constructor(private pagerService: PagerService,
        public dialog: MatDialog,
        private searchexperimentService: SearchExperimentService, ) { }

    ngOnInit() {

        this.searchexperimentService.GetSearchList().subscribe(data => {
            this.searchList = data;
            this.setPage(1);
            console.log(this.searchList);

        })
    }


    // Function defintion to add pagination to tabl Uploadlist (minor errors)
    setPage(page: number) {
        var filteredItems = this.searchList;

        filteredItems = this.filterByString(this.searchList, this.expfilter);

        // get pager object from service
        this.pager = this.pagerService.getPager(filteredItems.length, page, 20);

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
        return data.filter(e => e.expName.toUpperCase().includes(s.toUpperCase()) || e.cognitiveTask.toUpperCase().includes(s.toUpperCase()) || e.age.toUpperCase().includes(s.toUpperCase()) || e.strain.toUpperCase().includes(s.toUpperCase())
            || e.status.toUpperCase().includes(s.toUpperCase()) || e.genotype.toUpperCase().includes(s.toUpperCase()) || e.age.includes(s) ||
            e.username.toUpperCase().includes(s.toUpperCase()) || e.period.toUpperCase().includes(s.toUpperCase())); 

    }

}
