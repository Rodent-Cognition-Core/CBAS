import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cogbytes } from '../models/cogbytes';
import { CogbytesUpload } from '../models/cogbytesUpload'
import { CogbytesSearch } from '../models/cogbytesSearch'


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
    public addPI(piFullName: string, piAffiliation: string, piEmail: string, country: string): any {

        var obj = {
            'PIFullName': piFullName,
            'PIEmail': piEmail,
            'PIInstitution': piAffiliation,
            'InstitutionCountry' : country
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

    public getTouchscreenPIFromRepo(repObj: any): any {
        const body: string = JSON.stringify(repObj);
        return this.http.post("/api/cogbytes/GetPIDFromRepo", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        })
    }

    // Adding a new publication to System
    public addRepository(repObj: Cogbytes) : any {

        
        const body: string = JSON.stringify(repObj);
        // Sends an authenticated request.
        return this.http.post("/api/cogbytes/AddRepository", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    //// Extracting List of repositories from database
    public getRepositories(): any {

        return this.http
            .get("/api/cogbytes/GetRepositories", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    public getRepositoryMetadata(repID: number): any {

        return this.http
            .get("/api/cogbytes/GetRepositoryMetadata?repID=" + repID, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Editing a publication 
    public editRepository(repositoryID: number, repObj: Cogbytes) {

        const body: string = JSON.stringify(repObj);
        // Sends an authenticated request.
        return this.http.post("/api/cogbytes/EditRepository?repositoryID=" + repositoryID, body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    // Adding a new publication to System
    public addUpload(repObj: CogbytesUpload): any {


        const body: string = JSON.stringify(repObj);
        // Sends an authenticated request.
        return this.http.post("/api/cogbytes/AddUpload", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    //// Extracting List of upload files from database
    public getUploadFiles(uploadID: number): any {

        return this.http
            .get("/api/cogbytes/GetUploadFiles?uploadID=" + uploadID, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    //// Extracting List of repositories from database
    public getUploads(repID: number): any {

        return this.http
            .get("/api/cogbytes/GetUploads?repID=" + repID, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Editing an upload 
    public editUpload(uploadID: number, uploadObj: CogbytesUpload) {

        const body: string = JSON.stringify(uploadObj);
        // Sends an authenticated request.
        return this.http.post("/api/cogbytes/EditUpload?uploadID=" + uploadID, body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }


    downloadFile(path: string): Observable<Blob> {

        return this.http.get("/api/cogbytes/downloadFile?path=" + path,
            { headers: new HttpHeaders().set('Content-Type', 'application/json'), responseType: "blob" });

    }

    // Deleting a file
    public deleteFile(id: any, path: any): any {

        return this.http
            .delete("/api/cogbytes/DeleteFile?fileId=" + id + "&path=" + path, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Deleting an upload
    public deleteUpload(id: any): any {

        return this.http
            .delete("/api/cogbytes/DeleteUpload?uploadId=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Deleting a repository
    public deleteRepository(id: any): any {

        return this.http
            .delete("/api/cogbytes/DeleteRepository?repID=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    public searchRepositories(searchCogbytesObj: CogbytesSearch) {

        const body: string = JSON.stringify(searchCogbytesObj);

        return this.http
            .post("/api/cogbytes/SearchRepositories", body, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    public showAllRepositories() {

        return this.http
            .get("/api/cogbytes/ShowAllRepositories", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });

    }

    //// Extracting List of repositories from database
    public getAllRepositories(): any {

        return this.http
            .get("/api/cogbytes/GetAllRepositories", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Function definition to get the repo based on its Guid
    getDataByLinkGuid(repoLinkGuid: any): any {
        return this.http
            .get("/api/Cogbytes/GetDataByLinkGuid?repoLinkGuid=" + repoLinkGuid, {
                // headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    public getMetaDataByLinkGuid(repoLinkGuid: any): any {
        return this.http
            .get("/api/Cogbytes/GetMetaDataByLinkGuid?repoLinkGuid=" + repoLinkGuid, {
                // headers: this.authenticationService.getAuthorizationHeader()
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }


    public getGuidByRepID(repID: number): any {

        return this.http
            .get("/api/cogbytes/GetGuidByRepID?repID=" + repID, {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }




}







