import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { Request } from '../models/request';

import { AuthenticationService } from './authentication.service';

@Injectable() export class RequestService {

    

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    // Adding new Task
    public addTask(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddTask", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
                
            });
    }

    // Adding new PI
    public addPI(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddPI", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')

            });
    }

    // Adding new Age
    public addAge(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddAge", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')

            });
    }


    // Adding new MouseLine
    public addMouseLine(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddMouseLine", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')

            });
    }

    // Adding new Pubscreen Task Request
    public addPubTask(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddPubTask", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')

            });
    }

    // Adding new Pubscreen Task Request
    public addPubSubModel(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddPubSubModel", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')

            });
    }

    // Adding new General Request
    public addGeneral(request: Request): any {

        const body: string = JSON.stringify(request);

        return this.http
            .post("/api/Request/AddGeneral", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')

            });
    }  
    


}
