import { NgModule } from '@angular/core';
import { ContactUsRoutingModule } from './contact-us-routing.module';
import { SharedModule } from '../shared/shared.module';
import { ContactUsComponent } from './contact-us.component';
import { PagerService } from '../services/pager.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';


@NgModule({
    imports: [
        ContactUsRoutingModule,
        SharedModule,
        FontAwesomeModule
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

