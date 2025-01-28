import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
// import { DataExtraction } from '../models/dataextraction';

import { AuthenticationService } from './authentication.service';

@Injectable() export class SearchExperimentService {

  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }



  // Function definition to get list of all users
  public getSearchList(): any {

    return this.http
      .get('/api/searchexp/GetSearchList', {
        // headers: this.authenticationService.getAuthorizationHeader()
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  public getSearchByExpID(expID: any): any {

    return this.http
      .get('/api/searchexp/GetSearchByExpID?expID=' + expID, {
        // headers: this.authenticationService.getAuthorizationHeader()
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  downloadExpDs(expDsFileName: any): Observable<Blob> {

    return this.http.get('/api/searchexp/DownloadExpDs?expDsFileName=' + expDsFileName,
      { headers: new HttpHeaders().set('Content-Type', 'application/json'), responseType: 'blob' });

  };


}
