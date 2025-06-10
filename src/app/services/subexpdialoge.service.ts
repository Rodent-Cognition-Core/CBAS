
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

//import { Experiment } from '../models/experiment';
import { SubExperiment } from '../models/subexperiment';
import { AuthenticationService } from './authentication.service';

@Injectable() export class SubExpDialogeService {

    //public taskAnalysises: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    // Function Definition to get list of list of animal age
    public getAllAge(): any {

        return this.http
            .get("/api/subexperiment/GetAllAge", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }


    public getAllSubExp(id: any): any {

        return this.http
            .get("/api/subexperiment/GetAllSubExpbyExpID?expID=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Adding subexperiment
    public createSubExp(subexperiment: SubExperiment): Observable<any> {

        const body: string = JSON.stringify(subexperiment);
            

        return this.http.post("/api/subexperiment/CreateSubExperiment", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });

        
    }

    // Function Definition for updating SubExperiemnt when Edit button in clicked!
    public updateSubExp(subexperiment: SubExperiment): any {

        const body: string = JSON.stringify(subexperiment);
        return this.http.post("/api/subexperiment/UpdateSubExperiment", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });

    }

    // Function Definition: deleting SubExperiment when Del button is clicked! 
    public deleteSubExperimentbyID(id: any): any {

        return this.http
            .delete("/api/subexperiment/DeleteSubExpById?subExpId=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }

    // Function Definition to get all images from database for PAL and PD task
    public getAllImages(): any {

        return this.http
            .get("/api/subexperiment/GetAllImages", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }


}
