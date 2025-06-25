import { NgModule } from '@angular/core';
import { CogbytesRoutingModule } from './cogbytes-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { CogbytesComponent } from './cogbytes.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
import { CogbytesService } from '../services/cogbytes.service'
import { AnimalService } from '../services/animal.service';
import { PagerService } from '../services/pager.service';
import { CogbytesUploadModule } from '../cogbytesUpload/cogbytesUpload.module';
import { CogbytesSearchModule } from '../cogbytesSearch/cogbytesSearch.module';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@NgModule({
    imports: [
        CogbytesRoutingModule,
        CogbytesUploadModule,
        CogbytesSearchModule,
        SharedModule,
        MatDialogModule,
    ],
    declarations: [
        CogbytesComponent
    ],
    providers: [ManageUserService, PubScreenService, IdentityService, CogbytesService,
                AnimalService, PagerService, {
        provide: MatDialogRef,
        useValue: {}
    },
        { provide: MAT_DIALOG_DATA, useValue: {} },],

    bootstrap: [CogbytesComponent],
})
export class CogbytesModule { }
