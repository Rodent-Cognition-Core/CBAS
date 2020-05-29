import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { DataExtraction } from '../models/dataextraction';

import { AuthenticationService } from './authentication.service';

@Injectable() export class DataExtractionService {

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }

    // Getting list of all Tasks in DataBase
    public getAllTask(): any {

        return this.http
            .get("/api/dataExtraction/GetAllTask", {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting list of all experiments for the selected taslid
    public getAllExpByTaskID(id: any, userGuid: any, speciesID: any): any {

        return this.http
            .get("/api/dataExtraction/GetExpByTaskID?taskId=" + id + "&userGuid=" + userGuid + "&speciesId=" + speciesID, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    public getUserGuid(): any {
        //console.log(this.authenticationService.getAuthorizationHeader());
        return this.http
            .get("/api/dataExtraction/GetUserGuid", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Getting list of Sub-Tasks for the selected taskid
    public getAllSubtaskByTaskID(id: any): any {

        return this.http
            .get("/api/dataExtraction/GetScheduleNameByTaskID?taskID=" + id, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting list of Marker_Info features for the selected TaskID, SubTaskID, and Exps
    public getMarkerInfoBySubTaskIDExpID(subtaskid: any, expids: any): any {

        const body: string = JSON.stringify(expids);
        
        return this.http
            .post("/api/dataExtraction/GetMarketInfoBySubTaskIdExpId?subtaskID=" + subtaskid,  body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Getting list of Intervention by Exp IDs
    public getAllInterventionByExpID(expids: any): any {

        const body: string = JSON.stringify(expids);
        return this.http
            .post("/api/dataExtraction/GetInterventionByExpIDs", body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting list of Animal Age for the selected ExpIDs
    public getAnimalAgebyExpIDs(expids: any): any {

        const body: string = JSON.stringify(expids);
        return this.http
            .post("/api/dataExtraction/GetAnimalAgeByExpIDs" , body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting list of Animal Sex for the selected ExpIDs
    public getAnimalSexbyExpIDs(expids: any): any {

        const body: string = JSON.stringify(expids);
        return this.http
            .post("/api/dataExtraction/GetAnimalSexByExpIDs", body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting list of Animal Genotype for the GenoIDList
    public getAnimalGenotypebyExpIDs(genoids: any): any {

        const body: string = JSON.stringify(genoids);
        return this.http
            .post("/api/dataExtraction/GetAnimalGenotypeByExpIDs", body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting list of Animal Strain for the selected ExpID
    public getAnimalStrainbyExpIDs(expids: any): any {

        const body: string = JSON.stringify(expids);
        return this.http
            .post("/api/dataExtraction/GetAnimalStrainByExpIDs", body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Getting data from database for all the selected values by the suer
    public getData(data_extraction: DataExtraction): any {

        const body: string = JSON.stringify(data_extraction);

        return this.http
            .post("/api/dataExtraction/GetData", body, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    getDataByLinkGuid(linkGuid: any): any {
        return this.http
            .get("/api/dataExtraction/GetDataByLinkGuid?linkGuid=" + linkGuid, {
                // headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }


    saveLink(linkGuid: any): any {
        return this.http
            .get("/api/dataExtraction/SaveLink?linkGuid=" + linkGuid, {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    IncreaseCounter() {

        return this.http
            .post("/api/dataExtraction/IncreaseCounter", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    
}
