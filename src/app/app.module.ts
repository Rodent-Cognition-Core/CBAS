import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

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
import { AuthorDialogeComponent } from './authorDialoge/authorDialoge.component';
import { TermsDialogeComponent } from './termsDialoge/termsDialoge.component';
import { CogbytesDialogueComponent } from './cogbytesDialogue/cogbytesDialogue.component';
import { CogbytesAuthorDialogueComponent } from './cogbytesAuthorDialogue/cogbytesAuthorDialogue.component';
import { CogbytesPIDialogeComponent } from './cogbytesPIDialoge/cogbytesPIDialoge.component';


import { UploadResultDialogComponent } from './upload-result-dialog/upload-result-dialog.component';
import { DeleteConfirmDialogComponent } from './delete-confirm-dialog/delete-confirm-dialog.component';
import { NotificationDialogComponent } from './notification-dialog/notification-dialog.component';
import { GenericDialogComponent } from './generic-dialog/generic-dialog.component';

import { SubExpDialogeComponent } from './sub-exp-dialoge/sub-exp-dialoge.component';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { ScrollService } from './shared/scroll.service';
import { CountUpModule } from 'ngx-countup';

import { provideHttpClient } from '@angular/common/http';

import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { DROPZONE_CONFIG } from 'ngx-dropzone-wrapper';
import { DropzoneConfigInterface } from 'ngx-dropzone-wrapper';
import { RouterModule, Routes } from '@angular/router';
// import { GuidelineDataLabComponent } from './guidelineDataLab/guidelineDataLab.component';
// import { DataLinkComponent } from './data-link/data-link.component';
// import { GenomicsComponent } from './genomics/genomics.component';
// import { DownloadDsComponent } from './download-ds/download-ds.component';

export function initOAuth(oAuthConfig: OAuthConfig): Function {
  return () => oAuthConfig.load();
}

// declare global {
//  interface Navigator {
//      msSaveBlob?: (blob: any, defaultName?: string) => boolean
//  }
// }

// declare global {
//    interface Navigator {
//        msSaveBlob: (blobOrBase64: Blob | string, filename: string) => void
//    }
// }

// const ROUTES: Routes = [
//    { path: '', component: HomeComponent },
//    { path: 'guidline-datalab', component: GuidelineDataLabComponent },
//    { path: 'data-link', component: DataLinkComponent },
//    { path: 'genomics', component: GenomicsComponent },
//    { path: 'download-dataset', component: DownloadDsComponent },
// ];

@NgModule({
  imports: [
  BrowserModule,
  AppRoutingModule,
// RouterModule.forRoot(ROUTES),
  BrowserAnimationsModule,
  SharedModule,
  OAuthModule.forRoot(),
  CountUpModule,
  FontAwesomeModule,
  DropzoneModule,


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
  provideHttpClient()

  ],
  bootstrap: [AppComponent]
  })
export class AppModule { }
