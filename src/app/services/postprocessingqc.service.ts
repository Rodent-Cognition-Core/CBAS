import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
