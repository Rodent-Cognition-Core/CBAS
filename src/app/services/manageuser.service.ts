import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { User } from '../models/user';

import { AuthenticationService } from './authentication.service';

@Injectable() export class ManageUserService {

    

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    public GetEmailApprovalAndUserLockedStatus(username: any): any {
                
        return this.http
            .get("/api/manageuser/IsEmailApproved?UserName=" + username, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    
    // Function definition to get list of all users
    public GetAllUser(): any {

        return this.http
            .get("/api/manageuser/GetUserList", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    public approve(user: User): any {

        const body: string = JSON.stringify(user);
        return this.http.post("/api/manageuser/ApproveUser", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });

        //return this.http
        //    .get("/api/manageuser/ApproveUser?Email=" + email, {
        //       headers: this.authenticationService.getAuthorizationHeader()
               
        //    });
    }

    public lock(email: any): any {

        return this.http
            .get("/api/manageuser/LockUser?Email=" + email, {
                headers: this.authenticationService.getAuthorizationHeader()

            });
    }


    


}
