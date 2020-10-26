import { NgModule } from '@angular/core';

import { CogbytesUploadRoutingModule } from './cogbytesUpload-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { CogbytesUploadComponent } from './cogbytesUpload.component';
import { IdentityService } from '../services/identity.service';
import { CogbytesService } from '../services/cogbytes.service';
import { CogbytesUpload } from '../models/cogbytesUpload';

import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@NgModule({
    imports: [
        CogbytesUploadRoutingModule,
        SharedModule,
        MatDialogModule,
    ],
    declarations: [
        CogbytesUploadComponent
    ],
    providers: [ManageUserService, CogbytesService, IdentityService, {
        provide: MatDialogRef,
        useValue: {}
    },
        { provide: MAT_DIALOG_DATA, useValue: {} },],

    bootstrap: [CogbytesUploadComponent],

    exports: [CogbytesUploadComponent],
})
export class CogbytesUploadModule { }
