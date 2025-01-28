import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthenticationService } from './authentication.service';

export interface GetPISiteResponse {
  piSite: string;
}

@Injectable() export class PISiteService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }


  public getPISite(): Observable<GetPISiteResponse> {
    const url = '/api/PISite/GetPISite';
    return this.http
      .get<GetPISiteResponse>(url, {
      // headers: this.authenticationService.getAuthorizationHeader()
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    }).pipe(
      map((response) => response),
      catchError((error) => throwError(error)));
  }

  public getPISitebyUserID(): any {

    const pilist = this.http
      .get('/api/PISite/GetPISitebyUserID', {
        headers: this.authenticationService.getAuthorizationHeader()
      });
    return pilist;
  }






}
