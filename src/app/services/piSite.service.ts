import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from "rxjs";
import { AuthenticationService } from './authentication.service';

export interface getPISiteResponse {
    PISite: string;
}

@Injectable() export class PISiteService {

    

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    public getPISite(): Observable<getPISiteResponse> {
        const url = "/api/PISite/GetPISite"
        return this.http
            .get<getPISiteResponse>(url, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            }).pipe(
            map((response) => {
                return response;
            }),
            catchError((error) => {
                return throwError(error);
            }));
    }

    public getPISitebyUserID(): any {

        return this.http
            .get("/api/PISite/GetPISitebyUserID", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }



    


}
