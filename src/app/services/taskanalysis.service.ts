import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

import { TaskAnalysis } from '../models/taskAnalysis';

import { AuthenticationService } from './authentication.service';

@Injectable() export class TaskAnalysisService {

    public taskAnalysises: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }

    /**
     * Gets all task analysises.
     */
    public getAll(): void {
        // Sends an authenticated request.
        this.http
            .get("/api/taskAnalysis/GetAll", {
                headers: this.authenticationService.getAuthorizationHeader()
            })
            .subscribe((data: any) => {
                this.taskAnalysises.next(data);
            },
            (error: HttpErrorResponse) => {
                if (error.error instanceof Error) {
                    //console.log('An error occurred:', error.error.message);
                } else {
                    //console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                }
            });
    }

    public getAllSelect(): any {

        return this.http
            .get("/api/taskAnalysis/GetAll", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }


    public create(taskAnalysis: TaskAnalysis): Observable<any> {

        const body: string = JSON.stringify(taskAnalysis);

        return this.http.post("/api/taskAnalysis/CreateTaskAnalysis", body ,{
            headers: this.authenticationService.getAuthorizationHeader()
        }).pipe(
            map((res: any) => { /*console.log(res);*/return res; }));

    }

    public getTaskAnalysisByID(taskAnalysisID: number): Observable<any> {

        return this.http.post("/api/taskAnalysis/GetTaskAnalysisByID", taskAnalysisID, {
            headers: this.authenticationService.getAuthorizationHeader()
        }).pipe(
            map((res: any) => {
                //console.log('TaskAnalysis created successfully:', res);
                return res; // Return response for further processing if needed
            }),
            catchError((error: any) => {
                //console.error('Error creating TaskAnalysis:', error);
                return throwError(() => error); // Rethrow the error for the caller to handle
            })
        );

    }


}
