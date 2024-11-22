import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from "rxjs";
import { SubExperiment } from '../models/subexperiment';
import { AuthenticationService } from './authentication.service';

@Injectable() export class PostProcessingQcService {

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    public postProcessSubExperiment(subExp: SubExperiment): any {

        const body: string = JSON.stringify(subExp);

        return this.http.post("/api/PostProcessingQc/PostProcessSubExperiment", body , {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    public getPostProcessingResult(SubExpId: number): any {

        return this.http
            .get("/api/PostProcessingQc/GetPostProcessingResult?SubExpId=" + SubExpId, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

}
