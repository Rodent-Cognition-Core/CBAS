import { NgModule } from '@angular/core';

import { ProfileRoutingModule } from './profile-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ManageUserService } from '../services/manageuser.service';
import { ProfileComponent } from './profile.component';
import { IdentityService } from '../services/identity.service';
import { PISiteService } from '../services/piSite.service';
import { ProfileService } from '../services/profile.service';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  imports: [
    ProfileRoutingModule,
    SharedModule,
    MatDialogModule,
    MatButtonModule,
  ],
  declarations: [
    ProfileComponent
  ],
  providers: [ManageUserService,
    IdentityService,
    PISiteService,
    ProfileService, ],

  bootstrap: [ProfileComponent],
})
export class ProfileModule { }
