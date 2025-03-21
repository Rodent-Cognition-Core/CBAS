import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog} from '@angular/material/dialog';
import { UntypedFormControl } from '@angular/forms';
import { ReplaySubject ,  Subject } from 'rxjs';
import { PubScreenService } from '../services/pubScreen.service';
import { Pubscreen } from '../models/pubscreen';
import { AuthenticationService } from '../services/authentication.service';
import { PubscreenDialogeComponent } from '../pubscreenDialoge/pubscreenDialoge.component';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
    selector: 'app-pubscreen-edit',
    templateUrl: './pubScreenEdit.component.html',
    styleUrls: ['./pubScreenEdit.component.scss']
})
export class PubScreenEditComponent implements OnInit {

    
    paperInfo: any;
   
    paperLinkGuid: string;
    isAdmin: boolean;
    isFullDataAccess: boolean;
    isLoaded: boolean;

    panelOpenState: boolean


    _pubSCreenSearch: Pubscreen;

    public authorMultiFilterCtrl: UntypedFormControl = new UntypedFormControl();
    public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
    /** Subject that emits when the component has been destroyed. */
    private _onDestroy = new Subject<void>();

    constructor(public dialog: MatDialog,
        private authenticationService: AuthenticationService,
        private pubScreenService: PubScreenService,
        private route: ActivatedRoute,
        private spinnerService: NgxSpinnerService,
        public dialogAuthor: MatDialog) {

        this.isLoaded = false;
        this.paperLinkGuid = '';
        this.isAdmin = false;
        this.isFullDataAccess = false;
        this.panelOpenState = false;
        this.route.queryParams.subscribe(params => {
            this.paperLinkGuid = params['paperlinkguid'].split(" ")[0];

            this.GetDataByLinkGuid(this.paperLinkGuid);
        });
        this._pubSCreenSearch = {
            abstract: '', author: [], authorString: '', authourID: [], cellTypeID: [], celltypeOther: '',
            diseaseID: [], diseaseOther: '', doi: '', id: 0, keywords: '', methodID: [], methodOther: '',
            neurotransOther: '', paperType: '', paperTypeID: 0, paperTypeIdSearch: [], reference: '', regionID: [],
            search: '', sexID: [], source: '', specieID: [], specieOther: '', strainID: [], strainMouseOther: '',
            strainRatOther: '', subMethodID: [], subModelID: [], subRegionID: [], subTaskID: [], taskID: [], taskOther: '',
            title: '', transmitterID: [], year: '', yearFrom: 0, yearID: [], yearTo: 0
        }
    }

    ngOnInit() {

        this.isAdmin = this.authenticationService.isInRole("administrator");
        this.isFullDataAccess = this.authenticationService.isInRole("fulldataaccess");
    }

    GetDataByLinkGuid(paperLinkGuid : any) {
        this.spinnerService.show();

        this.pubScreenService.getDataByLinkGuid(paperLinkGuid).subscribe((data : any) => {

            this.paperInfo = data;
            console.log(this.paperInfo)

            this.isLoaded = true;



        });
        if (this.isLoaded) {
            this.spinnerService.hide();
        }


    }

    // Edit publication
    openDialogEditPublication(Publication : any): void {
        let dialogref = this.dialog.open(PubscreenDialogeComponent, {
            height: '850px',
            width: '1200px',
            data: { publicationObj: Publication, isPublic: Boolean }

        });

        dialogref.afterClosed().subscribe((_result : any) => {
            console.log('the dialog was closed');
            //this.search();
        });
    }

    

}
