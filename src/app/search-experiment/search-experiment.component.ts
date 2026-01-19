import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { SearchExperimentService } from '../services/searchexperiment.service';
import { PagerService } from '../services/pager.service';

declare var $: any;

//declare global {
//    interface Navigator {
//        msSaveBlob: (blobOrBase64: Blob | string, filename: string) => void
//    }
//}

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
        private searchexperimentService: SearchExperimentService,) {

        this.pagedItems = [];
    }

    ngOnInit() {

        this.searchexperimentService.GetSearchList().subscribe((data : any) => {
            this.searchList = data;
            this.setPage(1);

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

        setTimeout(() => {

            $('.pane-hScroll').scroll(function () {
                $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
            });

        }, 500);
    }

    filterByString(data: any, s: string): any {
        s = s.trim();
        return data.filter((e : any) => e.expName.toUpperCase().includes(s.toUpperCase()) || e.cognitiveTask.toUpperCase().includes(s.toUpperCase()) || e.age.toUpperCase().includes(s.toUpperCase()) || e.strain.toUpperCase().includes(s.toUpperCase())
            || e.status.toUpperCase().includes(s.toUpperCase()) || e.genotype.toUpperCase().includes(s.toUpperCase()) || e.age.includes(s) ||
            e.username.toUpperCase().includes(s.toUpperCase()) || e.period.toUpperCase().includes(s.toUpperCase())); 

    }

    DownloadDsFile(file: any): void {

        let path = "ds_" + file;
        this.searchexperimentService.downloadExpDs(path)
            .subscribe(result => {
                var fileData = new Blob([result]);
                var csvURL = null;
                csvURL = window.URL.createObjectURL(fileData);
                var tempLink = document.createElement('a');
                tempLink.href = csvURL;
                tempLink.setAttribute('download', path + '.csv');
                document.body.appendChild(tempLink);
                tempLink.click();

            },
                error => {
                    if (error.error instanceof Error) {
                        //console.log('An error occurred:', error.error.message);
                    } else {
                        //console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                    }
                });
    }

}
