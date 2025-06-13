import { Component, OnInit, OnDestroy } from '@angular/core'; // Import OnDestroy
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

    ngOnDestroy() {
        this.destroySpotfireApplication();
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
        this.destroySpotfireApplication();

        var customization = new spotfire.webPlayer.Customization();
        customization.showClose = false;
        customization.showDodPanel = false;
        customization.showHelp = false;
        customization.showReloadAnalysis = true;
        customization.showStatusBar = false;
        customization.showCollaboration = false;

        this.app = new spotfire.webPlayer.Application("https://mouse.robarts.ca/spotfire/wp/", customization);
        var configuration = 'mbusername="' + this.user.userName + '";';
        // Use arrow functions to preserve the 'this' context of the DataVisualizationComponent
        const onError = (errorCode : any, description : any) => {
            console.log('<span style="color: red;">[' + errorCode + "]: " + description + "</span>");
            this.spinnerService.hide();
        };
        const onOpenedfunction = () => {
            //console.log('onopened');
            this.spinnerService.hide();
        };

        this.app.onError(onError);
        this.app.onOpened(onOpenedfunction);


        this.app.open("/Public/User_Based_Visualizations/" + spotfireAnalysisName, "datavisual-dashboard-container", configuration);
    }

    private destroySpotfireApplication() {
        if (this.app) {
            if (typeof this.app.close == 'function') {
                this.app.close();
            }
            this.app = null;
        }
        const contentPanel = document.getElementById("datavisual-dashboard-container");
        if (contentPanel) {
            contentPanel.innerHTML = "";
        }
    }
}
