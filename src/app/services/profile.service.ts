import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { User } from '../models/user';

import { AuthenticationService } from './authentication.service';

@Injectable() export class ProfileService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }


  // Function Definition fro extracting userinfo

  public getUserInfo(): any {

    return this.http
      .get('/api/profile/GetUserInfo', {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }

  public updateProfile(obj: User): any {


    const body: string = JSON.stringify(obj);
    // console.log(body);
    return this.http
      .post('/api/Profile/UpdateProfile', body, {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }



}
