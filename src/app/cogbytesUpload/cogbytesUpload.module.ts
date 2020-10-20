import { NgModule } from '@angular/core';

import { CogbytesUploadRoutingModule } from './cogbytesUpload-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { CogbytesUploadComponent } from './cogbytesUpload.component';
import { IdentityService } from '../services/identity.service';
import { CogbytesService } from '../services/cogbytes.service';

@NgModule({
    imports: [
        CogbytesUploadRoutingModule,
        SharedModule
    ],
    declarations: [
        CogbytesUploadComponent
    ],
    providers: [ManageUserService, CogbytesService, IdentityService,],

    bootstrap: [CogbytesUploadComponent],
})
export class CogbytesModule { }
