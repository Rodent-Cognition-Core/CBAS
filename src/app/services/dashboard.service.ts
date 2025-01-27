import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Animal } from '../models/animal';

import { AuthenticationService } from './authentication.service';

@Injectable() export class DashboardService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }


  // This function gets
  public getUploadInfoById(id: any): any {

    return this.http
      .get('/api/upload/GetUploadInfoByID?expId=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }

  public getUploadErrorLogById(id: any): any {

    return this.http
      .get('/api/upload/GetUploadErrorLogByID?expId=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });

  }

  public clearUploadLogTblbyExpID(id: any): any {

    return this.http
      .delete('/api/upload/DelUploadLogTblbyId?expId=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }


  // public createAnimal(animal: Animal): any {

  //    const body: string = JSON.stringify(animal);
  //    return this.http.post("/api/animal/CreateAnimal", body, {
  //        headers: this.authenticationService.getAuthorizationHeader()
  //    });

  // }

  // public updateAniaml(animal: Animal): any {

  //    const body: string = JSON.stringify(animal);
  //    return this.http.post("/api/animal/UpdateAnimal", body, {
  //        headers: this.authenticationService.getAuthorizationHeader()
  //    });
  // }







}
