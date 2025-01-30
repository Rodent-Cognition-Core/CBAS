import { NgModule } from '@angular/core';
import { CogbytesEditRoutingModule } from './cogbytesEdit-routing.module';
import { SharedModule } from '../shared/shared.module';
import { CogbytesEditComponent } from './cogbytesEdit.component';
import { AuthenticationService } from '../services/authentication.service';
import { PagerService } from '../services/pager.service';
import { CogbytesService } from '../services/cogbytes.service';


@NgModule({
  imports: [
  CogbytesEditRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  CogbytesEditComponent,

  ],
  providers: [
  AuthenticationService,
  CogbytesService,
  PagerService,

  ],
  bootstrap: [CogbytesEditComponent],


  })
export class CogbytesEditModule { }

