import { NgModule } from '@angular/core';
import { PubScreenSearchRoutingModule } from './pubScreen-search-routing.module';
import { SharedModule } from '../shared/shared.module';
import { PubScreenSearchComponent } from './pubScreen-search.component';
import { PubScreenService } from '../services/pubScreen.service';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        PubScreenSearchRoutingModule,
        SharedModule,
        

    ],
    declarations: [
        PubScreenSearchComponent,
        
    ],
    providers: [
        PubScreenService,
        PagerService,
        
    ],
    bootstrap: [PubScreenSearchComponent],
    

})
export class PubScreenSearchModule { } 

