import { NgModule } from '@angular/core';
import { PubScreenSearchRoutingModule } from './pubScreen-search-routing.module';
import { SharedModule } from '../shared/shared.module';
import { PubScreenSearchComponent } from './pubScreen-search.component';
import { PubScreenService } from '../services/pubScreen.service';
import { AuthenticationService } from '../services/authentication.service';
import { PagerService } from '../services/pager.service';


@NgModule({
  imports: [
  PubScreenSearchRoutingModule,
  SharedModule,
// MatSelectModule,


  ],
  declarations: [
  PubScreenSearchComponent,

  ],
  providers: [
  AuthenticationService,
  PubScreenService,
  PagerService,

  ],
  bootstrap: [PubScreenSearchComponent],


  })
export class PubScreenSearchModule { }

