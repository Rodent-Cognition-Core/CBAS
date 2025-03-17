import { NgModule } from '@angular/core';
import { ContactUsRoutingModule } from './contact-us-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ContactUsComponent } from './contact-us.component';
import { PagerService } from '../services/pager.service';


@NgModule({
    imports: [
        ContactUsRoutingModule,
        SharedModule,
        //MatSelectModule,
        

    ],
    declarations: [
        ContactUsComponent,
        
    ],
    providers: [
        PagerService,
        
    ],
    bootstrap: [ContactUsComponent],

    exports: [ContactUsComponent],
})
export class ContactUsModule { } 

