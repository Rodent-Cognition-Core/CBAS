import { Component, OnInit } from '@angular/core';
declare var spotfire: any;
import { NgxSpinnerService } from 'ngx-spinner';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatTable, MatTableDataSource } from '@angular/material/table'
import { NotificationDialogComponent } from '../notification-dialog/notification-dialog.component';
import { AuthenticationService } from '../services/authentication.service';
import { User } from '../models/user'

@Component({
    selector: 'app-data-visualization',
    templateUrl: './data-visualization.component.html',
    styleUrls: ['./data-visualization.component.scss']
})
export class DataVisualizationComponent implements OnInit {

    selectedCogTaskValue: any = '';
    dialogRefLink: MatDialogRef<NotificationDialogComponent>;
    app: any;
    user: User;

    // SessionInfo Features' names
    public cogTaskList: any[] = [
        { cogTaskID: 1, name: '5-CSRTT' },
        { cogTaskID: 2, name: 'PAL' },
        { cogTaskID: 3, name: 'PD' },
        { cogTaskID: 4, name: 'iCPT' }
    ];

    //private filteringSchemeNames: any = {
    //    "1": "Filtering scheme (1)",
    //    "2": "Filtering scheme (2)",
    //    "3": "Filtering scheme (3)"
    //};

    //private dataTableNames: any = {
    //    "1": "vw_dPALsPAL_visualization",
    //    "2": "vw_dPALsPAL_visualization"
    //};

    //dataColumnName: "ExpName";


    constructor(
        public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private spinnerService: NgxSpinnerService,
        private router: Router,
        private route: ActivatedRoute) {

    }

    ngOnInit() {

        this.user = this.authenticationService.getUser();
        this.route.queryParams.subscribe(params => {

            if (params['taskid'] != null) {
                this.selectedCogTaskValue = +params['taskid'];
                this.selectCogTaskChange(false);
            }
        });


    }

    ngAfterViewInit() {

    }

    selectCogTaskChange(reload) {

        this.spinnerService.show();

        if (this.selectedCogTaskValue == 1) {
            this.loadAnalysis("MB_5C_userbased");
        }
        else if (this.selectedCogTaskValue == 2) {
            this.loadAnalysis("MB_PAL_userbased");
        }
        else if (this.selectedCogTaskValue == 3) {
            this.loadAnalysis("MB_PD_userbased");

        }
        else if (this.selectedCogTaskValue == 4) {
            this.loadAnalysis("MB_CPT_userbased");

        }

        if (reload) {
            this.router.navigateByUrl('data-visualization?taskid=' + this.selectedCogTaskValue);

        }


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
        var configuration = 'mbusername=\"' + this.user.userName + '\";';

        var onError = function (errorCode, description) {
            console.log('<span style="color: red;">[' + errorCode + "]: " + description + "</span>");
        };
        var onOpenedfunction = function () {
            //console.log('onopened');
        };

        this.app.onError(onError);
        this.app.onOpened(onOpenedfunction);


        this.app.open("/Public/User_Based_Visualizations/" + spotfireAnalysisName, "contentpanel", configuration);

        // 1. click btn -> getActiveFilteringScheme -> get which items are checked and add them to query string masalan: https://mousebytes.ca/data-visualization?taskid=2&sara=1,3
        // 2. when user comes to this page with querystring set, get values from querystring and set filters


        this.spinnerService.hide();

    }

    //GenerateLink() {
    //    // get filter
    //    if (this.selectedCogTaskValue == 1) { // 5-CSRTT

    //    } if (this.selectedCogTaskValue == 2) { //PAL

    //    } if (this.selectedCogTaskValue == 3) { // PD

    //    }
    //    //var filterQuerystring = "";
    //    //this.app.analysisDocument.filtering.getActiveFilteringScheme(
    //    //    function (args) {
    //    //        console.log('test');
    //    //        args.getDefaultFilterColumns(
    //    //            spotfire.webPlayer.includedFilterSettings.ALL_WITH_UNCHECKED_HIERARCHY_NODES,
    //    //            function (args2) {
    //    //                console.log(args2); //log(dump(args2, "spotfire.webPlayer.Document.FilteringScheme.getDefaultFilterColumns", 2));

    //    //                // this is to select ExpName filter
    //    //                var expNameFilter = args2.filter(obj => {
    //    //                    return obj.dataColumnName === "ExpName"
    //    //                })

    //    //                console.log(expNameFilter);
    //    //                console.log(expNameFilter[0]);
    //    //                console.log(expNameFilter[0].filterSettings);
    //    //                console.log(expNameFilter[0].filterSettings.values);
    //    //                console.log(expNameFilter[0].filterSettings.values.toString());

    //    //                filterQuerystring = "&filters=" + expNameFilter[0].filterSettings.values.toString();


    //    //            });
    //    //    });

    //    //this.dialogRefLink = this.dialog.open(NotificationDialogComponent, {
    //    //});

    //    //this.dialogRefLink.componentInstance.message = "https://mousebytes.ca/" + "?taskid=" + this.selectedCogTaskValue + filterQuerystring;



    //};
    //function getFilteringScheme("Filtering scheme Reversal", callback);

    //SetFilters() {
    //    // set filter
    //    var filterColumn = {
    //        filteringSchemeName: "Filtering scheme (2)",
    //        dataTableName: "vw_dPALsPAL_visualization",
    //        dataColumnName: "ExpName",
    //        filteringOperation: spotfire.webPlayer.filteringOperation.REPLACE,
    //        filterSettings: {
    //            includeEmpty: true,
    //            values: ["DP_AD_PAL", "FB_AD_PAL"]
    //        }
    //    };

    //    var filteringOperation = spotfire.webPlayer.filteringOperation.REPLACE;
    //    this.app.analysisDocument.filtering.setFilter(
    //        filterColumn,
    //        filteringOperation);
    //    console.log("[spotfire.webPlayer.Document.Filtering.setFilter]");

    //}
}
