import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
//import { DataExtraction } from '../models/dataextraction';

import { AuthenticationService } from './authentication.service';

@Injectable() export class SearchExperimentService {

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }



    // Function definition to get list of all users
    public GetSearchList(): any {

        return this.http
            .get("/api/searchexp/GetSearchList", {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    downloadExpDs(expDsFileName): Observable<Blob> {

        return this.http.get("/api/searchexp/DownloadExpDs?expDsFileName=" + expDsFileName,
            { headers: new HttpHeaders().set('Content-Type', 'application/json'), responseType: "blob" });

    };

    
}
