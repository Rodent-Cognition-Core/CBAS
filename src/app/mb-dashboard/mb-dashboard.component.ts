import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

declare var spotfire: any;

@Component({
    selector: 'app-mb-dashboard',
    templateUrl: './mb-dashboard.component.html',
    styleUrls: ['./mb-dashboard.component.scss']
})
export class MBDashboardComponent implements OnInit, OnDestroy {

    app: any;

   
    constructor(
        public dialog: MatDialog,
        private spinnerService: NgxSpinnerService,
        private router: Router,
        private route: ActivatedRoute,
        public dialogRefLink: MatDialog) {

    }

    ngOnInit() {

        this.spinnerService.show();
        this.loadAnalysis("View_MB_Data")
        
    }

    ngOnDestroy() {
        this.destroySpotfireApplication();
    }

    
    loadAnalysis(spotfireAnalysisName: string) {

        //console.log('loaded');
        var customization = new spotfire.webPlayer.Customization();
        customization.showClose = false;
        customization.showDodPanel = false;
        customization.showHelp = false;
        customization.showStatusBar = false;
        customization.showCollaboration = false;

        this.app = new spotfire.webPlayer.Application("https://mouse.robarts.ca/spotfire/wp/", customization);
        var configuration = '';

        var onError = function (errorCode : any, description : any) {
            console.log('<span style="color: red;">[' + errorCode + "]: " + description + "</span>");
        };
        var onOpenedfunction = function () {
            //console.log('onopened');
        };

        this.app.onError(onError);
        this.app.onOpened(onOpenedfunction);


        this.app.open("/Public/" + spotfireAnalysisName, "mousebytes-dashboard-container", configuration);

       
        this.spinnerService.hide();

    }

    private destroySpotfireApplication() {
        if (this.app) {
            if (typeof this.app.close === 'function') {
                this.app.close();
            }
            this.app = null;
        }
        const contentPanel = document.getElementById("mousebytes-dashboard-container");
        if (contentPanel) {
            contentPanel.innerHTML = "";
        }
    }
}
