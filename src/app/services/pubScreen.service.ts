import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { Pubscreen } from '../models/pubscreen';


import { AuthenticationService } from './authentication.service';

@Injectable() export class PubScreenService {



    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }



    // Extracting PaperType List From DB
    public getPaperType(): any {

        return this.http
            .get("/api/pubScreen/GetPaperType", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extractingl ist of Task List From DB
    public getTask(): any {

        return this.http
            .get("/api/pubScreen/GetTask", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Regions & Sub-regions List From DB
    public getTaskSubTask(): any {

        return this.http
            .get("/api/pubScreen/GetTaskSubTask", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Species List From DB
    public getSpecie(): any {

        return this.http
            .get("/api/pubScreen/GetSpecie", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Sex List From DB
    public getSex(): any {

        return this.http
            .get("/api/pubScreen/GetSex", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Strain List From DB
    public getStrain(): any {

        return this.http
            .get("/api/pubScreen/GetStrain", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Strain List From DB
    public getDisease(): any {

        return this.http
            .get("/api/pubScreen/GetDisease", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Regions & Sub-regions List From DB
    public getRegionSubRegion(): any {

        return this.http
            .get("/api/pubScreen/GetRegionSubRegion", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Regions List Only From DB
    public getRegion(): any {

        return this.http
            .get("/api/pubScreen/GetRegion", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Cell types List From DB
    public getCellType(): any {

        return this.http
            .get("/api/pubScreen/GetCellType", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting Method List From DB
    public getMethod(): any {

        return this.http
            .get("/api/pubScreen/GetMethod", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Extracting NeuroTransmitter List From DB
    public getNeurotransmitter(): any {

        return this.http
            .get("/api/pubScreen/GetNeurotransmitter", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Adding New Author to the database
    public addAuthor(authorFirstName: string, authorLastName: string, authorAffilation: string): any {

        var obj = {
            'firstName': authorFirstName,
            'lastName': authorLastName,
            'affiliation': authorAffilation,
        };

        const body: string = JSON.stringify(obj);

        // Sends an authenticated request.
        return this.http
            .post("/api/pubScreen/AddAuthor", body, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }

    // Extracting List of Autors from database
    public getAuthor(): any {

        return this.http
            .get("/api/pubScreen/GetAuthor", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Adding a new publication to System
    public addPublication(pubscreenObj: Pubscreen) {

        const body: string = JSON.stringify(pubscreenObj);
        // Sends an authenticated request.
        return this.http.post("/api/pubScreen/AddPublication", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    // Editing a publication 
    public EditPublication(publicationId: number, pubscreenObj: Pubscreen) {

        const body: string = JSON.stringify(pubscreenObj);
        // Sends an authenticated request.
        return this.http.post("/api/pubScreen/EditPublication?publicationId=" + publicationId, body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    // Deleting a publication when Del button is clicked!
    public deletePublicationById(id: any): any {

        return this.http
            .delete("/api/pubScreen/DeletePublicationById?pubId=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }


    // Searching publication based on search criteria
    public searchPublication(searchPubscreenObj: Pubscreen) {

        const body: string = JSON.stringify(searchPubscreenObj);

        return this.http
            .post("/api/pubScreen/SearchPublication", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Get list of all years in database
    public getAllYears(): any {

        return this.http
            .get("/api/pubScreen/GetAllYear", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    // Get some paper info based on Doi from pubMed
    public getPaparInfoFromDOI(doi: any): any {
        return this.http
            .get("/api/pubScreen/GetPaperInfoByDOI?DOI=" + doi, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Get some paper info based on PubmedKey
    public getPaparInfoFromPubmedKey(pubKey: any): any {
        return this.http
            .get("/api/pubScreen/GetPaperInfoByPubKey?PubKey=" + pubKey, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Get some paper info based on Doi from Biorxiv
    public getPaparInfoFromDOIBio(doi: any): any {
        return this.http
            .get("/api/pubScreen/GetPaparInfoFromDOIBio?DOI=" + doi, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }


    // Get some paper info based on Doi from Biorxiv
    public getPaperInfo(id: any): any {
        return this.http
            .get("/api/pubScreen/GetPaparInfoByID?ID=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }






}







