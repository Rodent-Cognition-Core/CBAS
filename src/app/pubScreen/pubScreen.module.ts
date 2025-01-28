import { NgModule } from '@angular/core';

import { PubSCreenRoutingModule } from './pubScreen-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { PubScreenComponent } from './pubScreen.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
import { CountUpModule } from 'ngx-countup';


@NgModule({
  imports: [
  PubSCreenRoutingModule,
  SharedModule,
  CountUpModule
  ],
  declarations: [
  PubScreenComponent
  ],
  providers: [ManageUserService, PubScreenService, IdentityService, ],

  bootstrap: [PubScreenComponent],

  exports: [PubScreenComponent],
  })
export class PubScreenModule { }
