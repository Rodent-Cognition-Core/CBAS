
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
//import { BehaviorSubject } from 'rxjs/BehaviorSubject';
//import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';

import { Experiment } from '../models/experiment';
import { AuthenticationService } from './authentication.service';

@Injectable() export class ExpDialogeService {

    //public taskAnalysises: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }

    /**
     * Gets all task analysises.
     */

    //public getAllExperiment(): any {

    //    return this.http
    //        .get("/api/experiment/GetAllExp", {
    //            headers: this.authenticationService.getAuthorizationHeader()
    //        });
    //}

    public create(experiment: Experiment): any {

        const body: string = JSON.stringify(experiment);
            

        return this.http.post("/api/experiment/CreateExperiment", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
        
    }

    // Function Definition for updating Experiemnt when Edit button in clicked!
    public updateExp(experiment: Experiment): any {

        const body: string = JSON.stringify(experiment);
        return this.http.post("/api/experiment/UpdateExperiment", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });

    }

    // Function Definition to get all images in database for PAL and PD task
    public getAllImages(): any {

        return this.http
            .get("/api/experiment/GetAllImages", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Function Definition to get all Species from DB: tbl Species
    public getAllSpecies(): any {

        return this.http
            .get("/api/experiment/GetAllSpecies", {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

}
