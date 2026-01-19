import { NgModule } from '@angular/core';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { SharedModule } from '../shared/shared.module';
import { UploadService } from '../services/upload.service';
import { DashboardComponent } from './dashboard.component';
import { DashboardService } from '../services/dashboard.service';
import { PagerService } from '../services/pager.service';
import { AnimalService } from '../services/animal.service';
import { PubScreenService } from '../services/pubScreen.service';
import { ExperimentService } from '../services/experiment.service';

@NgModule({
    imports: [
        DashboardRoutingModule,
        SharedModule
    ],
    declarations: [
        DashboardComponent
    ],
    providers: [DashboardService, UploadService, PagerService, AnimalService, ExperimentService, PubScreenService]
})
export class DashboardModule { }
