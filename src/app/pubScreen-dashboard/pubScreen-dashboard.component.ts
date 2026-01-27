import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

declare var spotfire: any;

@Component({
    selector: 'app-ps-dashboard',
    templateUrl: './pubScreen-dashboard.component.html',
    styleUrls: ['./pubScreen-dashboard.component.scss']
})
export class PSDashboardComponent implements OnInit, OnDestroy {

    app: any;

   
    constructor(
        public dialog: MatDialog,
        private spinnerService: NgxSpinnerService,
        private router: Router,
        private route: ActivatedRoute,
        public dialogRefLink: MatDialog) {

    }

    ngOnInit() {

        this.loadAnalysis("pubscreen_dashboard")
        
    }
    
    loadAnalysis(spotfireAnalysisName: string) {
        var customization = new spotfire.webPlayer.Customization();
        customization.showClose = false;
        customization.showDodPanel = false;
        customization.showHelp = false;
        customization.showReloadAnalysis = true;
        customization.showStatusBar = false;
        customization.showCollaboration = false;

        this.app = new spotfire.webPlayer.Application("https://mouse.robarts.ca/spotfire/wp/", customization);
        var configuration = '';

        var onError = function (errorCode : any, description : any) {
            console.log('<span style="color: red;">[' + errorCode + "]: " + description + "</span>");
        };
        var onOpenedfunction = function () {};

        this.app.onError(onError);
        this.app.onOpened(onOpenedfunction);


        this.app.open("/Public/" + spotfireAnalysisName, "contentpanel", configuration);

       
        this.spinnerService.hide();

    }

    ngOnDestroy() {
        if (this.app) {
            this.app.close();
        }
    }

   
}
