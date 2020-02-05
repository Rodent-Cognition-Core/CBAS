import { Component, OnInit, HostListener } from '@angular/core';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import { DataExtractionService } from '../services/dataextraction.service'
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { PagerService } from '../services/pager.service';

declare var $: any;

@Component({
    selector: 'app-data-link',
    templateUrl: './data-link.component.html',
    styleUrls: ['./data-link.component.scss']
})
export class DataLinkComponent implements OnInit {

    linkGuid: string;
    // dataSource: MatTableDataSource<Element[]>;
    result: any;
    colNames: any;
    description: any;

    pager: any = {};
    pagedItems: any[];

    constructor(
        private dataExtractionService: DataExtractionService,
        private spinnerService: Ng4LoadingSpinnerService,
        private route: ActivatedRoute,
        private pagerService: PagerService,
    ) {
        this.description = "";
        this.colNames = [];

        this.route.queryParams.subscribe(params => {
            this.linkGuid = params['linkguid'];

            this.GetDataByLinkGuid();
        });
    }

    @HostListener('window:resize', ['$event'])
    onResize(event) {
        $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
    }

    ngOnInit() {

    }

    GetDataByLinkGuid() {
        this.spinnerService.show();

        this.dataExtractionService.getDataByLinkGuid(this.linkGuid).subscribe(data => {

            console.log(data);

            this.result = data.listOfRows;
            //console.log(this.result);
            this.description = data.description;
            this.colNames = [];


            //var subTaskWithStimulation = [3, 8, 9, 10, 21, 22, 23, 24, 25, 26, 27];

            //if (subTaskWithStimulation.indexOf(data.subTaskID) > 0) {

            //    console.log(this.result);

            //    this.result = this.result.map(function (a) {
            //        var test_str = a.Schedule_Name;
            //        var start_pos = test_str.indexOf('_') + 1;
            //        var end_pos = test_str.indexOf('_', start_pos);
            //        var text_to_get = '';
            //        if (end_pos > start_pos) {
            //            text_to_get = test_str.substring(start_pos, end_pos);

            //            if (text_to_get.toLowerCase().indexOf('ms') > 0) {
            //                text_to_get = text_to_get.toLowerCase().replace('ms', '');
            //            } else if (text_to_get.toLowerCase().indexOf('s') > 0) {

            //                text_to_get = text_to_get.replace(',', '.');
            //                if (text_to_get.indexOf('0') == 0) { // replace 04s with 0.4s
            //                    text_to_get = ['0.', text_to_get.slice(1)].join('');
            //                }

            //                text_to_get = (parseFloat(text_to_get.toLowerCase().replace('s', '')) * 1000).toString();

            //            } else {
            //                text_to_get = '';
            //            }
            //        }

            //        delete a.Schedule_Name;
            //        a.Schedule_Name = test_str;

            //        a.Stimulation_Duration_ms = text_to_get;

            //        return a;
            //    });

            //    //console.log(this.result);
            //}

            this.setPage(1);
            //this.colNames = [];

            if (this.result.length > 0) {
                var a = this.result[0];
                Object.keys(a).forEach(function (key) { return console.log(key); });
                for (var key in a) {

                    this.colNames.push(key);



                }
            }


            setTimeout(() => {
                this.spinnerService.hide();

                $('.pane-hScroll').scroll(function () {
                    $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
                });

            }, 500);


        });

    }

    DownloadCsv() {
        let csv: '';

        //var items = this.result;
        var items = this.result;

        // Loop the array of objects
        for (let row = 0; row < items.length; row++) {
            let keysAmount = Object.keys(items[row]).length

            let keysCounter = 0

            // If this is the first row, generate the headings
            if (row === 0) {

                // Loop each property of the object
                for (let key in items[row]) {
                    console.log(1)
                    console.log(key);

                    // This is to not add a comma at the last cell
                    // The '\r\n' adds a new line
                    if (key == 'AnimalID') { csv = ''; }
                    csv += key + (keysCounter + 1 < keysAmount ? ',' : '\r\n')
                    console.log(csv)
                    keysCounter++
                }
            }

            keysCounter = 0;
            for (let key in items[row]) {

                csv += items[row][key] + (keysCounter + 1 < keysAmount ? ',' : '\r\n')
                //console.log(csv)
                keysCounter++
            }


            keysCounter = 0
        }


        var blob = new Blob([csv], { type: 'text/csv' });
        var filename = 'exported_' + new Date().toLocaleString() + '.csv';
        if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveBlob(blob, filename);
        }
        else {
            var elem = window.document.createElement('a');
            elem.href = window.URL.createObjectURL(blob);
            elem.download = filename;
            document.body.appendChild(elem);
            elem.click();
            document.body.removeChild(elem);
        }


    }

    // Function defintion to add pagination to table 
    setPage(page: number) {
        // get pager object from service
        this.pager = this.pagerService.getPager(this.result.length, page, 20);

        if (page < 1 || page > this.pager.totalPages) {
            this.pagedItems = [];
            return;
        }

        // get current page of items
        this.pagedItems = this.result.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }


}
