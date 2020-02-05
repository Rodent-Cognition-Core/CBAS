import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';

import { AuthenticationService } from './authentication.service';

/**
 * Identity service (to Identity Web API controller).
 */
@Injectable() export class IdentityService {

    public users: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) {
    }

    /**
     * Gets all users.
     */
    public getAll(): void {
        // Sends an authenticated request.
        this.http
            .get("/api/identity/GetAll", {
                headers: this.authenticationService.getAuthorizationHeader()
            })
            .subscribe((data: any) => {
                this.users.next(data);
            },
            (error: HttpErrorResponse) => {
                if (error.error instanceof Error) {
                    console.log('An error occurred:', error.error.message);
                } else {
                    console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                }
            });
    }

    public generatePasswordResetToken(email): any {

        return this.http
            .get("/api/identity/GeneratePasswordResetToken?email=" + email, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    /**
     * Creates a new user.
     * @param model User's data
     * @return An IdentityResult
     */
    public create(model: any): Observable<any> {
        const body: string = JSON.stringify(model);

        // Sends an authenticated request.
        return this.http.post("/api/identity/Create", body, {
            headers: new HttpHeaders().set('Content-Type', 'application/json')
        }).pipe(
            map((response: Response) => {
                return response;
            }),
            catchError((error: any) => {
                return _throw(error);
            }));
    }

    /**
     * Deletes a user.
     * @param username Username of the user
     * @return An IdentityResult
     */
    public delete(username: string): void {
        const body: string = JSON.stringify(username);

        // Sends an authenticated request.
        this.http
            .post("/api/identity/Delete", body, {
                headers: this.authenticationService.getAuthorizationHeader()
            })
            .subscribe(() => {
                // Refreshes the users.
                this.getAll();
            },
            (error: HttpErrorResponse) => {
                if (error.error instanceof Error) {
                    console.log('An error occurred:', error.error.message);
                } else {
                    console.log(`Backend returned code ${error.status}, body was: ${error.error}`);
                }
            });
    }

    // Add other methods.

    // reset Password
    public submitNewPass(email: string, token: string, password: string): any {

        var obj = {
            'email': email,
            'token': token,
            'password': password
        };

        const body: string = JSON.stringify(obj);

        return this.http
            .post("/api/identity/ResetPassword", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // change Password
    public changePassword(currentPassword: string, newPassword: string): any {

        var obj = {
            'currentPassword': currentPassword,
            'newPassword': newPassword,
        };

        const body: string = JSON.stringify(obj);

        // Sends an authenticated request.
        return this.http
            .post("/api/profile/ChangePassword", body, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

       
    }
    
}
