import { NgModule } from '@angular/core';

import { PubSCreenRoutingModule } from './pubScreen-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { PubScreenComponent } from './pubScreen.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';

@NgModule({
    imports: [
        PubSCreenRoutingModule,
        SharedModule
    ],
    declarations: [
        PubScreenComponent
    ],
    providers: [ManageUserService, PubScreenService, IdentityService,],

    bootstrap: [PubScreenComponent],
})
export class PubScreenModule { }
