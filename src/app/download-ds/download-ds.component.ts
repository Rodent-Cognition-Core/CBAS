import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SearchExperimentService } from '../services/searchexperiment.service';
import { DOWNLOADERROR } from '../shared/messages';

//declare global {
//    interface Navigator {
//        msSaveBlob: (blobOrBase64: Blob | string, filename: string) => void
//    }
//}

@Component({
    selector: 'app-download-ds',
    templateUrl: './download-ds.component.html',
    styleUrls: ['./download-ds.component.scss']
})
export class DownloadDsComponent implements OnInit {

    text: string;
    dsName: string;
    expObj: any;

    constructor(
        private route: ActivatedRoute,
        private searchexperimentService: SearchExperimentService,

    ) {
        this.text = '';
        this.dsName = '';
        this.route.queryParams.subscribe(params => {
            this.dsName = params['ds'];
            if (params['ds'] == null) {
                this.dsName = "";
            }
        });
    }

    ngOnInit() {
        //console.log(this.dsName);
        
        if (this.dsName != "") {
            this.searchexperimentService.GetSearchByExpID(parseInt(this.dsName)).subscribe((data : any) => {
                this.expObj = data[0];
                this.expObj.age = this.expObj.age.replaceAll('<br/>', ', ');
                this.expObj.strain = this.expObj.strain.replaceAll('<br/>', ', ');
                //console.log(this.expObj);

                //this.text = "Data download should start shortly!";
                //this.DownloadDsFile(this.dsName);

            })
            

        } else {
            this.text = DOWNLOADERROR;
        }
    }

    
    DownloadDsFile(file: string): void {

        let path = "ds_" + file;
        this.searchexperimentService.downloadExpDs(path)
            .subscribe(result => {

                var fileData = new Blob([result]);
                var csvURL = null;
                //if (navigator.msSaveBlob) {
                //    csvURL = navigator.msSaveBlob(fileData, path + '.csv');
                //} else {
                //    csvURL = window.URL.createObjectURL(fileData);
                //}
                csvURL = window.URL.createObjectURL(fileData);
                var tempLink = document.createElement('a');
                tempLink.href = csvURL;
                tempLink.setAttribute('download', path + '.csv');
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


}
