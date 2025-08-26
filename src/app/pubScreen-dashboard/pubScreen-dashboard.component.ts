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
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.spinnerService.show();
        this.loadAnalysis("pubscreen_dashboard");
    }
    
    ngOnDestroy() {
        this.destroySpotfireApplication();
    }

    loadAnalysis(spotfireAnalysisName: string) {
        this.destroySpotfireApplication(); 

        var customization = new spotfire.webPlayer.Customization();
        customization.showClose = false;
        customization.showDodPanel = false;
        customization.showHelp = false;
        customization.showStatusBar = false;
        customization.showCollaboration = false;

        this.app = new spotfire.webPlayer.Application("https://mouse.robarts.ca/spotfire/wp/", customization);
        var configuration = '';

        // Use arrow functions to preserve 'this' context for spinnerService
        const onError = (errorCode: any, description: any) => {
            console.log('<span style="color: red;">[' + errorCode + "]: " + description + "</span>");
            this.spinnerService.hide();
        };
        const onOpenedfunction = () => {
            //console.log('onopened');
            this.spinnerService.hide();
        };

        this.app.onError(onError);
        this.app.onOpened(onOpenedfunction);

        this.app.open("/Public/" + spotfireAnalysisName, "pubscreen-dashboard-container", configuration);
    }

    private destroySpotfireApplication() {
        if (this.app) {
            if (typeof this.app.close === 'function') {
                this.app.close();
            }
            this.app = null;
        }
        const contentPanel = document.getElementById("pubscreen-dashboard-container");
        if (contentPanel) {
            contentPanel.innerHTML = "";
        }
    }
}
