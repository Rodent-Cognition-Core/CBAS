import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { User } from '../models/user';

import { AuthenticationService } from './authentication.service';

// Data Structures

export interface UserStatusResponse {
  isUserApproved: boolean;
  isUserLocked: boolean;
}

@Injectable() export class ManageUserService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }


  public GetEmailApprovalAndUserLockedStatus(username: string): Observable<UserStatusResponse> {
    const url = '/api/manageuser/IsEmailApproved?UserName=${username}';
    return this.http
      .get<UserStatusResponse>(url, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });

  }


  // Function definition to get list of all users
  public GetAllUser(): any {

    return this.http
      .get('/api/manageuser/GetUserList', {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }

  public approve(user: User): any {

    const body: string = JSON.stringify(user);
    return this.http.post('/api/manageuser/ApproveUser', body, {
      headers: this.authenticationService.getAuthorizationHeader()
    });

    // return this.http
    //    .get("/api/manageuser/ApproveUser?Email=" + email, {
    //       headers: this.authenticationService.getAuthorizationHeader()

    //    });
  }

  public lock(email: any): any {

    return this.http
      .get('/api/manageuser/LockUser?Email=' + email, {
        headers: this.authenticationService.getAuthorizationHeader()

      });
  }





}
