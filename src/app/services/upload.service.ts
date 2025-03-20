import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


import { AuthenticationService } from './authentication.service';

@Injectable() export class UploadService {

    //public mapping_SessionInfo: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    public getAllExperiment(): any {

        return this.http
            .get("/api/experiment/GetAllExp", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    //public GetUploadResult(): any {

    //    return this.http
    //        .get("/api/upload/UploadFiles", {
    //            headers: this.authenticationService.getAuthorizationHeader()
    //        });
    //};



    public setUploadAsResolved(uploadId: number): any {

        return this.http
            .get("/api/upload/SetUploadAsResolved?uploadId=" + uploadId, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    downloadFile(id: number): Observable<Blob> {

        return this.http.get("/api/upload/downloadFile?uploadId=" + id,
            { headers: this.authenticationService.getAuthorizationHeader(), responseType: "blob" });

    }

    public getSessionInfo(): any {

        return this.http
            .get("/api/upload/GetSessionInfo", {
                //headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

}
