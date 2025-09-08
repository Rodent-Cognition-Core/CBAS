import { NgModule } from '@angular/core';
import { CogbytesSearchRoutingModule } from './cogbytesSearch-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { CogbytesSearchComponent } from './cogbytesSearch.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
import { CogbytesService } from '../services/cogbytes.service';
import { PISiteService } from '../services/piSite.service';

@NgModule({
    imports: [
        CogbytesSearchRoutingModule,
        SharedModule
    ],
    declarations: [
        CogbytesSearchComponent
    ],
    providers: [ManageUserService, PubScreenService, CogbytesService, IdentityService, PISiteService, PubScreenService],

    bootstrap: [CogbytesSearchComponent],

    exports: [CogbytesSearchComponent],
})
export class CogbytesSearchModule { }
