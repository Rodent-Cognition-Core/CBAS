import { Component, OnInit, HostListener } from '@angular/core';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import { DataExtractionService } from '../services/dataextraction.service';
import { LoadingService } from '../services/loadingservice'
import { PagerService } from '../services/pager.service';

declare let $: any;

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
    private spinnerService: LoadingService,
    private route: ActivatedRoute,
    private pagerService: PagerService,
  ) {
    this.description = '';
    this.colNames = [];
    this.linkGuid = '';
    this.pagedItems = [];

    this.route.queryParams.subscribe(params => {
      this.linkGuid = params['linkguid'];

      this.getDataByLinkGuid();
    });
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    $('.pane-vScroll').width($('.pane-hScroll').width() + $('.pane-hScroll').scrollLeft());
  }

  ngOnInit() {

  }

  getDataByLinkGuid() {
    this.spinnerService.show();

    this.dataExtractionService.getDataByLinkGuid(this.linkGuid).subscribe((data: any) => {

      // console.log(data);

      this.result = data.listOfRows;
      // console.log(this.result);
      this.description = data.description;
      this.colNames = [];



      this.setPage(1);
      // this.colNames = [];

      if (this.result.length > 0) {
        const a = this.result[0];
        Object.keys(a).forEach(function (key) {
          return /* console.log(key)*/;
        });
        for (const key in a) {
          if (Object.prototype.hasOwnProperty.call(a, key)) {
            this.colNames.push(key);
          }

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

  downloadCsv() {
    let csv: string;
    csv = '';

    // var items = this.result;
    const items = this.result;

    // Loop the array of objects
    for (let row = 0; row < items.length; row++) {
      const keysAmount = Object.keys(items[row]).length;

      let keysCounter = 0;

      // If this is the first row, generate the headings
      if (row === 0) {

        // Loop each property of the object
        for (const key in items[row]) {
          if (Object.prototype.hasOwnProperty.call(items[row], key)) {
            // console.log(1)
            // console.log(key);

            // This is to not add a comma at the last cell
            // The '\r\n' adds a new line
            if (key === 'AnimalID') {
              csv = '';
            }
            csv += key + (keysCounter + 1 < keysAmount ? ',' : '\r\n');
            // console.log(csv)
            keysCounter++;
          }
        }
      }

      keysCounter = 0;
      for (const key in items[row]) {
        if (Object.prototype.hasOwnProperty.call(items[row], key)) {
          csv += items[row][key] + (keysCounter + 1 < keysAmount ? ',' : '\r\n');
          // console.log(csv)
          keysCounter++;
        }
      }


      keysCounter = 0;
    }


    const blob = new Blob([csv], { type: 'text/csv' });
    const filename = 'exported_' + new Date().toLocaleString() + '.csv';
    const _win = window.navigator as any;
    if (_win.msSaveOrOpenBlob) {
      _win.msSaveBlob(blob, filename);
    } else {
      const elem = window.document.createElement('a');
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
