import { Component, OnInit } from '@angular/core';
declare var spotfire: any;
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import { MatTableDataSource, MatDialogRef, MatDialog } from '@angular/material';
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { filterQueryId } from '@angular/core/src/view/util';

@Component({
    selector: 'app-mb-dashboard',
    templateUrl: './mb-dashboard.component.html',
    styleUrls: ['./mb-dashboard.component.scss']
})
export class MBDashboardComponent implements OnInit {

    
    dialogRefLink: MatDialogRef<NotificationDialogComponent>;
    app: any;

   
    constructor(
        public dialog: MatDialog,
        private spinnerService: Ng4LoadingSpinnerService,
        private router: Router,
        private route: ActivatedRoute) {

    }

    ngOnInit() {

        this.loadAnalysis("View_MB_Data")
        
    }

    ngAfterViewInit() {

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

        this.app = new spotfire.webPlayer.Application("https://mouse.cac.queensu.ca/spotfire/wp/", customization);
        var configuration = '';

        var onError = function (errorCode, description) {
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
