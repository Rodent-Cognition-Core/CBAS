import { NgModule } from '@angular/core';

import { RBT_TouchScreen_FeaturesRoutingModule } from './rBT_TouchScreen_Features-routing.module';
import { SharedModule } from '../shared/shared.module';

import { RBT_TouchScreen_FeaturesService } from '../services/rBT_TouchScreen_Features.service';

import { RBT_TouchScreen_FeaturesComponent } from './rBT_TouchScreen_Features.component';

@NgModule({
    imports: [
        RBT_TouchScreen_FeaturesRoutingModule,
        SharedModule
    ],
    declarations: [
        RBT_TouchScreen_FeaturesComponent
    ],
    providers: [
        RBT_TouchScreen_FeaturesService
    ]
})
export class Mapping_SessionInfoModule { }
