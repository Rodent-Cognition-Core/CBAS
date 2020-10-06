import { NgModule } from '@angular/core';

import { CogbytesRoutingModule } from './cogbytes-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { CogbytesComponent } from './cogbytes.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';

@NgModule({
    imports: [
        CogbytesRoutingModule,
        SharedModule
    ],
    declarations: [
        CogbytesComponent
    ],
    providers: [ManageUserService, PubScreenService, IdentityService,],

    bootstrap: [CogbytesComponent],
})
export class CogbytesModule { }
