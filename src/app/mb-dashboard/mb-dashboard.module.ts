import { NgModule } from '@angular/core';
import { MBDashboardRoutingModule } from './mb-dashboard-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MBDashboardComponent } from './mb-dashboard.component';
import { DataExtractionService } from '../services/dataextraction.service';

@NgModule({
  imports: [
  MBDashboardRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  MBDashboardComponent,

  ],
  providers: [
  DataExtractionService,

  ],
  bootstrap: [MBDashboardComponent],

  exports: [MBDashboardComponent],
  })
export class MBDashboardModule { }

