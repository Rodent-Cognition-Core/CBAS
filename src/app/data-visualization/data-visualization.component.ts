import { Component, OnInit, OnDestroy } from '@angular/core';
declare var spotfire: any;
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { AuthenticationService } from '../services/authentication.service';
import { User } from '../models/user'

@Component({
    selector: 'app-data-visualization',
    templateUrl: './data-visualization.component.html',
    styleUrls: ['./data-visualization.component.scss']
})
export class DataVisualizationComponent implements OnInit, OnDestroy {

    selectedCogTaskValue: any = '';
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

        this.user = { Email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' }
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

    selectCogTaskChange(reload : any) {

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

        var customization = new spotfire.webPlayer.Customization();
        customization.showClose = false;
        customization.showDodPanel = false;
        customization.showHelp = false;
        customization.showStatusBar = false;
        customization.showCollaboration = false;

        this.app = new spotfire.webPlayer.Application("https://mouse.robarts.ca/spotfire/wp/", customization);
        var configuration = 'mbusername="' + this.user.userName + '";';

        var onError = function (errorCode : any, description : any) {
            console.log('<span style="color: red;">[' + errorCode + "]: " + description + "</span>");
        };
        var onOpenedfunction = function () {};

        this.app.onError(onError);
        this.app.onOpened(onOpenedfunction);


        this.app.open("/Public/User_Based_Visualizations/" + spotfireAnalysisName, "contentpanel-dataviz", configuration);

        // 1. click btn -> getActiveFilteringScheme -> get which items are checked and add them to query string masalan: https://mousebytes.ca/data-visualization?taskid=2&sara=1,3
        // 2. when user comes to this page with querystring set, get values from querystring and set filters


        this.spinnerService.hide();

    }

    ngOnDestroy() {
        if (this.app) {
            this.app.close();
        }
    }
}
