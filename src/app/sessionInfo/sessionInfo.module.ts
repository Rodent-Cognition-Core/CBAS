import { NgModule } from '@angular/core';

import { SessionInfoRoutingModule } from './sessionInfo-routing.module';
import { SharedModule } from '../shared/shared.module';

import { SessionInfoService } from '../services/sessionInfo.service';

import { SessionInfoComponent } from './sessionInfo.component';

@NgModule({
    imports: [
        SessionInfoRoutingModule,
        SharedModule
    ],
    declarations: [
        SessionInfoComponent
    ],
    providers: [
        SessionInfoService
    ]
})
export class SessionInfoModule { }
