import { NgModule } from '@angular/core';
import { FormsRoutingModule } from './forms-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsComponent } from './forms.component';
import { RequestService } from '../services/request.service';
import { MatDialogModule, MatButtonModule } from '@angular/material';



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

