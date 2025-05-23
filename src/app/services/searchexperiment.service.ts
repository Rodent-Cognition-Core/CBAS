import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Observable } from 'rxjs';

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

    public GetSearchByExpID(expID : any): any {

        return this.http
            .get("/api/searchexp/GetSearchByExpID?expID=" + expID, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    downloadExpDs(expDsFileName : any): Observable<Blob> {

        return this.http.get("/api/searchexp/DownloadExpDs?expDsFileName=" + expDsFileName,
            { headers: new HttpHeaders().set('Content-Type', 'application/json'), responseType: "blob" });

    }

    
}
