import { NgModule } from '@angular/core';
import { AnimalInfoRoutingModule } from './animal-info-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { AnimalInfoComponent } from './animal-info.component';
import { AnimalService } from '../services/animal.service';
import { AuthenticationService } from '../services/authentication.service';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { PagerService } from '../services/pager.service';

@NgModule({
  imports: [
    AnimalInfoRoutingModule,
    SharedModule,
    MatSelectModule,
    MatDialogModule,
    MatButtonModule,

  ],
  declarations: [
    AnimalInfoComponent,

  ],
  providers: [
    AuthenticationService,
    AnimalService,
    PagerService,

  ],
  bootstrap: [AnimalInfoComponent],


})
export class AnimalInfoModule { }

