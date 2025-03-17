import { NgModule } from '@angular/core';
import { PubScreenEditRoutingModule } from './pubScreenEdit-routing.module';
import { SharedModule } from '../shared/shared.module';
import { PubScreenEditComponent } from './pubScreenEdit.component';
import { PubScreenService } from '../services/pubScreen.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        PubScreenEditRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        PubScreenEditComponent,
        
    ],
    providers: [
        PubScreenService,
        PagerService,
        
    ],
    bootstrap: [PubScreenEditComponent],
    

})
export class PubScreenEditModule { } 

