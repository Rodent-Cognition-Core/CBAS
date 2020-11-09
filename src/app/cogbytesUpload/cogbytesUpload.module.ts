import { NgModule } from '@angular/core';

import { CogbytesUploadRoutingModule } from './cogbytesUpload-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { CogbytesUploadComponent } from './cogbytesUpload.component';
import { IdentityService } from '../services/identity.service';
import { CogbytesService } from '../services/cogbytes.service';
import { CogbytesUpload } from '../models/cogbytesUpload';

import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { DROPZONE_CONFIG } from 'ngx-dropzone-wrapper';
import { DropzoneConfigInterface } from 'ngx-dropzone-wrapper';

import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@NgModule({
    imports: [
        CogbytesUploadRoutingModule,
        SharedModule,
        DropzoneModule,
        MatDialogModule,
    ],
    declarations: [
        CogbytesUploadComponent
    ],
    providers: [ManageUserService, CogbytesService, IdentityService, {
            provide: MatDialogRef,
            useValue: {}
        },
        {
            provide: MAT_DIALOG_DATA, useValue: {}
        },
        {
            provide: DROPZONE_CONFIG,
            useValue: {
                // Change this to your upload POST address:
                url: 'http://localhost:5000/api/upload/UploadFiles',
                // maxFilesize: 50,
                acceptedFiles: '.*',
                // headers: { 'Authorization': this.authenticationService.getAuthorizationHeader() },
                parallelUploads: 10000,
                uploadMultiple: true,
                autoProcessQueue: false
            }
        },
    ],

    bootstrap: [CogbytesUploadComponent],

    exports: [CogbytesUploadComponent],
})
export class CogbytesUploadModule { }
