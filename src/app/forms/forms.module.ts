import { NgModule } from '@angular/core';
import { FormsRoutingModule } from './forms-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsComponent } from './forms.component';
import { RequestService } from '../services/request.service';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';



@NgModule({
  imports: [
    FormsRoutingModule,
    SharedModule,
    MatDialogModule,
    MatButtonModule,



  ],
  declarations: [
    FormsComponent,

  ],
  providers: [
    RequestService,


  ],

  bootstrap: [FormsComponent],

  exports: [FormsComponent],

})
export class FormsModule { }

