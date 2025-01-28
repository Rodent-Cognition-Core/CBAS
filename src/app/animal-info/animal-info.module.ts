import { NgModule } from '@angular/core';
import { AnimalInfoRoutingModule } from './animal-info-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MatSelectModule } from '@angular/material/select';
import { AnimalInfoComponent } from './animal-info.component';
import { AnimalService } from '../services/animal.service';
import { AuthenticationService } from '../services/authentication.service';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
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

