import { NgModule } from '@angular/core';
import { ContactUsRoutingModule } from './contact-us-routing.module';
import { SharedModule } from '../shared/shared.module';
// import { MatSelectModule } from '@angular/material/select';
import { ContactUsComponent } from './contact-us.component';
import { AuthenticationService } from '../services/authentication.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { PagerService } from '../services/pager.service';


@NgModule({
  imports: [
  ContactUsRoutingModule,
  SharedModule,
  FontAwesomeModule
// MatSelectModule,


  ],
  declarations: [
  ContactUsComponent,

  ],
  providers: [
  AuthenticationService,
  PagerService,

  ],
  bootstrap: [ContactUsComponent],

  exports: [ContactUsComponent],
  })
export class ContactUsModule { }

