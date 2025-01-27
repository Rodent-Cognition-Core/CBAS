import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Animal } from '../models/animal';

import { AuthenticationService } from './authentication.service';

@Injectable() export class ExperimentService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }


  // Function Defintion for getting some info from Upload Table and show them in Experiment page
  public getUploadInfoBySubExpId(id: any): any {

    return this.http
      .get('/api/upload/GetUploadInfoBySubExpId?subExpId=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }

  public deleteExperimentbyID(id: any): any {

    return this.http
      .delete('/api/experiment/DeleteExpById?expId=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });

  }

  public deleteFilebyID(id: any): any {

    return this.http
      .delete('/api/experiment/DeleteFileById?uploadID=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }





}
