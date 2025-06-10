import { NgModule } from '@angular/core';
import { ExperimentRoutingModule } from './experiment-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MatSelectModule } from '@angular/material/select';
import { ExperimentComponent } from './experiment.component';
import { ExperimentService } from '../services/experiment.service';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button'
import { UploadService } from '../services/upload.service';
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
        ExperimentService,
        UploadService,
        PagerService,

    ],
    bootstrap: [ExperimentComponent],
    

})
export class ExperimentModule { }

