import { NgModule } from '@angular/core';

import { Mapping_SessionInfoRoutingModule } from './mapping_SessionInfo-routing.module';
import { SharedModule } from '../shared/shared.module';

import { Mapping_SessionInfoService } from '../services/mapping_SessionInfo.service';

import { Mapping_SessionInfoComponent } from './mapping_SessionInfo.component';

@NgModule({
    imports: [
        Mapping_SessionInfoRoutingModule,
        SharedModule
    ],
    declarations: [
        Mapping_SessionInfoComponent
    ],
    providers: [
        Mapping_SessionInfoService
    ]
})
export class Mapping_SessionInfoModule { }
