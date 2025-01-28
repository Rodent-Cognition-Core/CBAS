import { NgModule } from '@angular/core';
import { ExperimentRoutingModule } from './experiment-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { ExperimentComponent } from './experiment.component';
import { ExperimentService } from '../services/experiment.service';
import { AuthenticationService } from '../services/authentication.service';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { UploadService } from '../services/upload.service';
// import { PostProcessingQcService } from '../services/postprocessingqc.service';
import { PagerService } from '../services/pager.service';
import { OrderModule } from 'ngx-order-pipe';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';


@NgModule({
  imports: [
    ExperimentRoutingModule,
    SharedModule,
    MatSelectModule,
    MatDialogModule,
    MatButtonModule,
    OrderModule,
    NgxDatatableModule,
  ],
  declarations: [
    ExperimentComponent,

  ],
  providers: [
    AuthenticationService,
    ExperimentService,
    UploadService,
    // PostProcessingQcService,
    PagerService,

  ],
  bootstrap: [ExperimentComponent],


})
export class ExperimentModule { }

