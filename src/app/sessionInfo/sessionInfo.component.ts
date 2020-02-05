import { Component, OnInit } from '@angular/core';
import { DataSource } from '@angular/cdk/collections';
import { Observable } from 'rxjs/Observable';

import { SessionInfoService } from '../services/sessionInfo.service';

@Component({
    selector: 'app-sessionInfo',
    templateUrl: './sessionInfo.component.html',
    styleUrls: ['./sessionInfo.component.scss']
})
export class SessionInfoComponent implements OnInit {

   

    ngOnInit() {
        
    }

}

/**
 * Data source to provide data rendered in the table.
 */

