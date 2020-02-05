import { NgModule } from '@angular/core';

import { ManageUserRoutingModule } from './manage-user-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { ManageUserComponent } from './manage-user.component';
import { IdentityService } from '../services/identity.service';
import { PagerService } from '../services/pager.service';

@NgModule({
    imports: [
        ManageUserRoutingModule,
        SharedModule
    ],
    declarations: [
        ManageUserComponent
    ],
    providers: [ManageUserService, PagerService, IdentityService,]
})
export class ManageUserModule { }
