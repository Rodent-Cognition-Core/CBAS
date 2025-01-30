import { NgModule } from '@angular/core';
import { PSDashboardRoutingModule } from './pubScreen-dashboard-routing.module';
import { SharedModule } from '../shared/shared.module';
import { PSDashboardComponent } from './pubScreen-dashboard.component';
import { DataExtractionService } from '../services/dataextraction.service';

@NgModule({
  imports: [
  PSDashboardRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  PSDashboardComponent,

  ],
  providers: [
  DataExtractionService,

  ],
  bootstrap: [PSDashboardComponent],


  })
export class PSDashboardModule { }

