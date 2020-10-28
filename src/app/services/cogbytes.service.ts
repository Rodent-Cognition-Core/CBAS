import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { Cogbytes } from '../models/cogbytes';
import { CogbytesUpload } from '../models/cogbytesUpload'


import { AuthenticationService } from './authentication.service';

@Injectable() export class CogbytesService {



    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }



    // Extracting file type List From DB
    public getFileTypes(): any {

        return this.http
            .get("/api/cogbytes/GetFileTypes", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extractingl ist of Task List From DB
    public getTask(): any {

        return this.http
            .get("/api/cogbytes/GetTask", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Species List From DB
    public getSpecies(): any {

        return this.http
            .get("/api/cogbytes/GetSpecies", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    //// Extracting Sex List From DB
    public getSex(): any {

        return this.http
            .get("/api/cogbytes/GetSex", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    //// Extracting Strain List From DB
    public getStrain(): any {

        return this.http
            .get("/api/cogbytes/GetStrain", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    //// Extracting Genos List From DB
    public getGenos(): any {

        return this.http
            .get("/api/cogbytes/GetGenos", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Strain List From DB
    public getAges(): any {

        return this.http
            .get("/api/cogbytes/GetAges", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    //// Adding New Author to the database
    public addAuthor(authorFirstName: string, authorLastName: string, authorAffilation: string): any {

        var obj = {
            'firstName': authorFirstName,
            'lastName': authorLastName,
            'affiliation': authorAffilation,
        }; 

        const body: string = JSON.stringify(obj);

        // Sends an authenticated request.
        return this.http
            .post("/api/cogbytes/AddAuthor", body, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }

    //// Extracting List of Autors from database
    public getAuthor(): any {

        return this.http
            .get("/api/cogbytes/GetAuthor", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    //// Adding New PI to the database
    public addPI(piFullName: string, piAffiliation: string, piEmail: string): any {

        var obj = {
            'PIFullName': piFullName,
            'PIEmail': piEmail,
            'PIInstitution': piAffiliation,
        };

        const body: string = JSON.stringify(obj);

        // Sends an authenticated request.
        return this.http
            .post("/api/cogbytes/AddPI", body, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }

    //// Extracting List of PIs from database
    public getPI(): any {

        return this.http
            .get("/api/cogbytes/GetPI", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Adding a new publication to System
    public addRepository(repObj: Cogbytes) : any {

        
        const body: string = JSON.stringify(repObj);
        // Sends an authenticated request.
        return this.http.post("/api/cogbytes/AddRepository", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    //// Editing a publication 
    //public EditPublication(publicationId: number, pubscreenObj: Pubscreen) {

    //    const body: string = JSON.stringify(pubscreenObj);
    //    // Sends an authenticated request.
    //    return this.http.post("/api/pubScreen/EditPublication?publicationId=" + publicationId, body, {
    //        headers: this.authenticationService.getAuthorizationHeader()
    //    });
    //}

    //// Deleting a publication when Del button is clicked!
    //public deletePublicationById(id: any): any {

    //    return this.http
    //        .delete("/api/pubScreen/DeletePublicationById?pubId=" + id, {
    //            headers: this.authenticationService.getAuthorizationHeader()
    //        });

    //}


    //// Searching publication based on search criteria
    //public searchPublication(searchPubscreenObj: Pubscreen) {

    //    const body: string = JSON.stringify(searchPubscreenObj);

    //    return this.http
    //        .post("/api/pubScreen/SearchPublication", body, {
    //            headers: new HttpHeaders().set('Content-Type', 'application/json')
    //        });

    //}

    //// Get list of all years in database
    //public getAllYears(): any {

    //    return this.http
    //        .get("/api/pubScreen/GetAllYear", {
    //            headers: new HttpHeaders().set('Content-Type', 'application/json')
    //        });

    //}

    //// Get some paper info based on Doi from pubMed
    //public getPaparInfoFromDOI(doi: any): any {
    //    return this.http
    //        .get("/api/pubScreen/GetPaperInfoByDOI?DOI=" + doi, {
    //            headers: new HttpHeaders().set('Content-Type', 'application/json')
    //        });
    //}

    //// Get some paper info based on PubmedKey
    //public getPaparInfoFromPubmedKey(pubKey: any): any {
    //    return this.http
    //        .get("/api/pubScreen/GetPaperInfoByPubKey?PubKey=" + pubKey, {
    //            headers: new HttpHeaders().set('Content-Type', 'application/json')
    //        });
    //}

    //// Get some paper info based on Doi from Biorxiv
    //public getPaparInfoFromDOIBio(doi: any): any {
    //    return this.http
    //        .get("/api/pubScreen/GetPaparInfoFromDOIBio?DOI=" + doi, {
    //            headers: new HttpHeaders().set('Content-Type', 'application/json')
    //        });
    //}


    //// Get some paper info based on Doi from Biorxiv
    //public getPaperInfo(id: any): any {
    //    return this.http
    //        .get("/api/pubScreen/GetPaparInfoByID?ID=" + id, {
    //            headers: this.authenticationService.getAuthorizationHeader()
    //        });
    //}






}







