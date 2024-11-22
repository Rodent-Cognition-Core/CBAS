import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule, MatDialogModule, MatInputModule, MatFormFieldModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

//import { MatDividerModule } from '@angular/material/divider';
//import { MatFileUploadModule } from 'angular-material-fileupload';

import { AppRoutingModule } from './app-routing.module';
import { SharedModule } from './shared/shared.module';

import { AuthGuard } from './services/auth.guard';
import { AuthenticationService } from './services/authentication.service';
import { IdentityService } from './services/identity.service';
import { PISiteService } from './services/piSite.service';
import { ManageUserService } from './services/manageuser.service';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';

import { OAuthModule } from 'angular-oauth2-oidc';
import { OAuthConfig } from './oauth.config';
import { ExpDialogeComponent } from './expDialoge/expDialoge.component';
import { PubscreenDialogeComponent } from './pubscreenDialoge/pubscreenDialoge.component';
import { AnimalDialogComponent } from './animal-dialog/animal-dialog.component';
import { PasswordDialogComponent } from './password-dialog/password-dialog.component';
import { ReqTaskDialogeComponent } from './reqTaskDialoge/reqTaskDialoge.component';
import { ReqPIDialogeComponent } from './reqPIDialoge/reqPIDialoge.component';
import { ReqAgeDialogeComponent } from './reqAgeDialoge/reqAgeDialoge.component';
import { ReqMouseLineDialogeComponent } from './reqMouseLineDialoge/reqMouseLineDialoge.component';
import { ReqGeneralDialogeComponent } from './reqGeneralDialoge/reqGeneralDialoge.component';
import { ReqPubTaskDialogeComponent } from './reqPubTaskDialoge/reqPubTaskDialoge.component';
import { ReqPubModelDialogeComponent } from './reqPubModelDialoge/reqPubModelDialoge.component';
import { ReqPubSubMethodDialogeComponent } from './reqPubSubMethodDialoge/reqPubSubMethodDialoge.component';
import { AuthorDialogeComponent } from './authorDialoge/authorDialoge.component'
import { TermsDialogeComponent } from './termsDialoge/termsDialoge.component';
import { CogbytesDialogueComponent } from './cogbytesDialogue/cogbytesDialogue.component'
import { CogbytesUploadComponent } from './cogbytesUpload/cogbytesUpload.component'
import { CogbytesAuthorDialogueComponent } from './cogbytesAuthorDialogue/cogbytesAuthorDialogue.component'
import { CogbytesPIDialogeComponent } from './cogbytesPIDialoge/cogbytesPIDialoge.component'
import { CogbytesSearchComponent } from './cogbytesSearch/cogbytesSearch.component'


import { UploadResultDialogComponent } from './upload-result-dialog/upload-result-dialog.component';
import { DeleteConfirmDialogComponent } from './delete-confirm-dialog/delete-confirm-dialog.component';
import { NotificationDialogComponent } from './notification-dialog/notification-dialog.component';
import { GenericDialogComponent } from './generic-dialog/generic-dialog.component';


import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';
import { SubExpDialogeComponent } from './sub-exp-dialoge/sub-exp-dialoge.component';
//import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

import { FlexLayoutModule } from '@angular/flex-layout';
import { OwlModule } from 'ngx-owl-carousel';

import { ScrollService } from './shared/scroll.service';
import { CountUpModule } from 'countup.js-angular2';

import { Angulartics2Module } from 'angulartics2';
import { Angulartics2GoogleAnalytics } from 'angulartics2/ga';
import { RouterModule, Routes } from '@angular/router'
import { GuidelineComponent } from './guideline/guideline.component';
import { GuidelineDataLabComponent } from './guidelineDataLab/guidelineDataLab.component';
import { DataExtractionComponent } from './data-extraction/data-extraction.component';
import { DataLinkComponent } from './data-link/data-link.component';
import { DataVisualizationComponent } from './data-visualization/data-visualization.component';
import { MBDashboardComponent } from './mb-dashboard/mb-dashboard.component';
import { PSDashboardComponent } from './pubScreen-dashboard/pubScreen-dashboard.component';
import { GenomicsComponent } from './genomics/genomics.component';
import { VideoTutorialComponent } from './video-tutorial/video-tutorial.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { DownloadDsComponent } from './download-ds/download-ds.component';
import { FormsComponent } from './forms/forms.component';
import { TermsComponent } from './terms/terms.component';
import { SearchExperimentComponent } from './search-experiment/search-experiment.component';
import { PubScreenComponent } from './pubScreen/pubScreen.component';

export function initOAuth(oAuthConfig: OAuthConfig): Function {
    return () => oAuthConfig.load();
}

declare global {
    interface Navigator {
        msSaveBlob?: (blob: any, defaultName?: string) => boolean
    }
}

const ROUTES: Routes = [
    { path: '', component: HomeComponent },
    { path: 'guidline', component: GuidelineComponent },
    { path: 'guidline', component: GuidelineDataLabComponent },
    { path: 'guidline', component: DataExtractionComponent },
    { path: 'guidline', component: DataLinkComponent },
    { path: 'guidline', component: DataVisualizationComponent },
    { path: 'guidline', component: MBDashboardComponent },
    { path: 'guidline', component: GenomicsComponent },
    { path: 'guidline', component: VideoTutorialComponent },
    { path: 'guidline', component: ContactUsComponent },
    { path: 'guidline', component: DownloadDsComponent },
    { path: 'guidline', component: FormsComponent },
    { path: 'guidline', component: TermsComponent },
    { path: 'guidline', component: SearchExperimentComponent },
    { path: 'guidline', component: PubScreenComponent },
];

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        SharedModule,
        OAuthModule.forRoot(),
        MatButtonModule,
        MatDialogModule,
        MatInputModule,
        FormsModule,
        ReactiveFormsModule,
        Ng4LoadingSpinnerModule.forRoot(),
        //MatSelectModule,
        MatFormFieldModule,
        //NgxMatSelectSearchModule,
        FlexLayoutModule,
        OwlModule,
        CountUpModule,
        RouterModule.forRoot(ROUTES),
        Angulartics2Module.forRoot([Angulartics2GoogleAnalytics]),


    ],
    declarations: [
        AppComponent,
        HomeComponent,
        ExpDialogeComponent,
        PubscreenDialogeComponent,
        AnimalDialogComponent,
        PasswordDialogComponent,
        ReqTaskDialogeComponent,
        ReqPIDialogeComponent,
        ReqAgeDialogeComponent,
        ReqMouseLineDialogeComponent,
        ReqGeneralDialogeComponent,
        ReqPubTaskDialogeComponent,
        ReqPubModelDialogeComponent,
        ReqPubSubMethodDialogeComponent,
        TermsDialogeComponent,
        AuthorDialogeComponent,
        UploadResultDialogComponent,
        DeleteConfirmDialogComponent,
        GenericDialogComponent,
        SubExpDialogeComponent,
        NotificationDialogComponent,
        CogbytesDialogueComponent,
        CogbytesAuthorDialogueComponent,
        CogbytesPIDialogeComponent,
        //CogbytesUploadComponent,
       

    ],
    exports: [

     ],
    providers: [
        Title,
        OAuthConfig,
        {
            provide: APP_INITIALIZER,
            useFactory: initOAuth,
            deps: [OAuthConfig],
            multi: true
        },
        AuthGuard,
        AuthenticationService,
        IdentityService,
        PISiteService,
        ManageUserService,
        ScrollService,

    ],
    entryComponents: [
        ExpDialogeComponent,
        PubscreenDialogeComponent,
        AnimalDialogComponent,
        PasswordDialogComponent,
        UploadResultDialogComponent,
        DeleteConfirmDialogComponent,
        GenericDialogComponent,
        SubExpDialogeComponent,
        NotificationDialogComponent,
        ReqTaskDialogeComponent,
        ReqPIDialogeComponent,
        ReqAgeDialogeComponent,
        ReqGeneralDialogeComponent,
        ReqPubTaskDialogeComponent,
        ReqPubModelDialogeComponent,
        ReqPubSubMethodDialogeComponent,
        TermsDialogeComponent,
        ReqMouseLineDialogeComponent,
        AuthorDialogeComponent,
        CogbytesDialogueComponent,
        CogbytesUploadComponent,
        CogbytesAuthorDialogueComponent,
        CogbytesPIDialogeComponent,
        CogbytesSearchComponent,
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
