import { NgModule } from '@angular/core';

import { PubScreenQueueRoutingModule } from './pubScreenQueue-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { PubScreenQueueComponent } from './pubScreenQueue.component';
import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';

@NgModule({
  imports: [
    PubScreenQueueRoutingModule,
    SharedModule
  ],
  declarations: [
    PubScreenQueueComponent
  ],
  providers: [ManageUserService, PubScreenService, IdentityService, ],

  bootstrap: [PubScreenQueueComponent],
})
export class PubScreenQueueModule { }
