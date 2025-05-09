import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

declare var spotfire: any;

@Component({
    selector: 'app-mb-dashboard',
    templateUrl: './mb-dashboard.component.html',
    styleUrls: ['./mb-dashboard.component.scss']
})
export class MBDashboardComponent implements OnInit {

    app: any;

   
    constructor(
        public dialog: MatDialog,
        private spinnerService: NgxSpinnerService,
        private router: Router,
        private route: ActivatedRoute,
        public dialogRefLink: MatDialog) {

    }

    ngOnInit() {

        this.loadAnalysis("View_MB_Data")
        
    }

    
    loadAnalysis(spotfireAnalysisName: string) {

        //console.log('loaded');
        var customization = new spotfire.webPlayer.Customization();
        //customization.showCustomizableHeader = false;
        //customization.showToolBar = false;
        customization.showClose = false;
        //customization.showTopHeader = false;

        //customization.showTopHeader = false;

        //customization.showToolBar = true;

        //customization.showAbout = false;
        //customization.showAnalysisInfo = false;
        //customization.showAnalysisInformationTool = false;
        //customization.showClose = false;
        //customization.showCustomizableHeader = false;
        customization.showDodPanel = false;

        //customization.showExportFile = true;

        //customization.showExportVisualization = false;
        //customization.showFilterPanel = true;
        customization.showHelp = false;
        //customization.showLogout = false;
        //customization.showPageNavigation = false;
        customization.showReloadAnalysis = true;
        customization.showStatusBar = false;
        //customization.showUndoRedo = false;
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


        this.app.open("/Public/" + spotfireAnalysisName, "contentpanel", configuration);

       
        this.spinnerService.hide();

    }

   
}
