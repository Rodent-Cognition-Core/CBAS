import { NgModule, APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { SharedModule } from './shared/shared.module';
import { AuthGuard } from './services/auth.guard';
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
import { CogbytesAuthorDialogueComponent } from './cogbytesAuthorDialogue/cogbytesAuthorDialogue.component'
import { CogbytesPIDialogeComponent } from './cogbytesPIDialoge/cogbytesPIDialoge.component'
import { UploadResultDialogComponent } from './upload-result-dialog/upload-result-dialog.component';
import { DeleteConfirmDialogComponent } from './delete-confirm-dialog/delete-confirm-dialog.component';
import { NotificationDialogComponent } from './notification-dialog/notification-dialog.component';
import { GenericDialogComponent } from './generic-dialog/generic-dialog.component';
import { NgxSpinnerModule } from 'ngx-spinner'
import { SubExpDialogeComponent } from './sub-exp-dialoge/sub-exp-dialoge.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ScrollService } from './shared/scroll.service';
import { CountUpModule } from 'ngx-countup';
import { DropzoneModule } from 'ngx-dropzone-wrapper'
import { Angulartics2Module } from 'angulartics2';


export function initOAuth(oAuthConfig: OAuthConfig): Function {
    return () => oAuthConfig.load();
}

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        SharedModule,
        OAuthModule.forRoot(),
        NgxSpinnerModule,
        FlexLayoutModule,
        CarouselModule,
        CountUpModule,
        FontAwesomeModule,
        DropzoneModule,
        Angulartics2Module.forRoot(),


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
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
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
        IdentityService,
        PISiteService,
        ManageUserService,
        ScrollService,

    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
