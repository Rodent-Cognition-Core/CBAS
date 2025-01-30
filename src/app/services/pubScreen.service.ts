import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Pubscreen } from '../models/pubscreen';


import { AuthenticationService } from './authentication.service';

@Injectable() export class PubScreenService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }



  // Extracting PaperType List From DB
  public getPaperType(): any {

    return this.http
      .get('/api/pubScreen/GetPaperType', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extractingl ist of Task List From DB
  public getTask(): any {

    return this.http
      .get('/api/pubScreen/GetTask', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Regions & Sub-regions List From DB
  public getTaskSubTask(): any {

    return this.http
      .get('/api/pubScreen/GetTaskSubTask', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Species List From DB
  public getSpecie(): any {

    return this.http
      .get('/api/pubScreen/GetSpecie', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Sex List From DB
  public getSex(): any {

    return this.http
      .get('/api/pubScreen/GetSex', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Strain List From DB
  public getStrain(): any {

    return this.http
      .get('/api/pubScreen/GetStrain', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Strain List From DB
  public getDisease(): any {

    return this.http
      .get('/api/pubScreen/GetDisease', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting SubModel List From DB
  public getSubModels(): any {

    return this.http
      .get('/api/pubScreen/GetSubModels', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Regions & Sub-regions List From DB
  public getRegionSubRegion(): any {

    return this.http
      .get('/api/pubScreen/GetRegionSubRegion', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Regions List Only From DB
  public getRegion(): any {

    return this.http
      .get('/api/pubScreen/GetRegion', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Cell types List From DB
  public getCellType(): any {

    return this.http
      .get('/api/pubScreen/GetCellType', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Method List From DB
  public getMethod(): any {

    return this.http
      .get('/api/pubScreen/GetMethod', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting Method List From DB
  public getSubMethod(): any {

    return this.http
      .get('/api/pubScreen/GetSubMethod', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Extracting NeuroTransmitter List From DB
  public getNeurotransmitter(): any {

    return this.http
      .get('/api/pubScreen/GetNeurotransmitter', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Adding New Author to the database
  public addAuthor(authorFirstName: string, authorLastName: string, authorAffilation: string): any {

    const obj = {
      'firstName': authorFirstName,
      'lastName': authorLastName,
      'affiliation': authorAffilation,
    };

    const body: string = JSON.stringify(obj);

    // Sends an authenticated request.
    return this.http
      .post('/api/pubScreen/AddAuthor', body, {
        headers: this.authenticationService.getAuthorizationHeader()
      });

  }

  // Extracting List of Autors from database
  public getAuthor(): any {

    return this.http
      .get('/api/pubScreen/GetAuthor', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Adding a new publication to System
  public addPublication(pubscreenObj: Pubscreen) {

    const body: string = JSON.stringify(pubscreenObj);
    // Sends an authenticated request.
    return this.http.post('/api/pubScreen/AddPublication', body, {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }

  // Editing a publication
  public editPublication(publicationId: number, pubscreenObj: Pubscreen) {

    const body: string = JSON.stringify(pubscreenObj);
    // Sends an authenticated request.
    return this.http.post('/api/pubScreen/EditPublication?publicationId=' + publicationId, body, {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }

  // Editing a publication
  public editPublicationPublic(publicationId: number, pubscreenObj: Pubscreen) {

    const body: string = JSON.stringify(pubscreenObj);
    // Sends an authenticated request.
    return this.http.post('/api/pubScreen/EditPublicationPublic?publicationId=' + publicationId, body, {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });
  }

  // Deleting a publication when Del button is clicked!
  public deletePublicationById(id: any): any {

    return this.http
      .delete('/api/pubScreen/DeletePublicationById?pubId=' + id, {
        headers: this.authenticationService.getAuthorizationHeader()
      });

  }


  // Searching publication based on search criteria
  public searchPublication(searchPubscreenObj: Pubscreen) {

    const body: string = JSON.stringify(searchPubscreenObj);

    return this.http
      .post('/api/pubScreen/SearchPublication', body, {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Get list of all years in database
  public getAllYears(): any {

    return this.http
      .get('/api/pubScreen/GetAllYear', {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });

  }

  // Get some paper info based on Doi from pubMed
  public getPaparInfoFromDOI(doi: any): any {
    return this.http
      .get('/api/pubScreen/GetPaperInfoByDOI?DOI=' + doi, {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  // Get some paper info based on PubmedKey
  public getPaparInfoFromPubmedKey(pubKey: any): any {
    return this.http
      .get('/api/pubScreen/GetPaperInfoByPubKey?PubKey=' + pubKey, {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  // Get some paper info based on Doi from Biorxiv
  public getPaparInfoFromDOIBio(doi: any): any {
    return this.http
      .get('/api/pubScreen/GetPaparInfoFromDOIBio?DOI=' + doi, {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  // Get some paper info based on Doi from Crossref
  public getPaparInfoFromDOICrossref(doi: any): any {
    return this.http
      .get('/api/pubScreen/GetPaparInfoFromDOICrossref?DOI=' + doi, {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  // Get some paper info based on Doi from Biorxiv
  public getPaperInfo(id: any): any {
    return this.http
      .get('/api/pubScreen/GetPaparInfoByID?ID=' + id, {
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  // Get papers in the pubmed queue
  public getPubmedQueue(): any {
    return this.http
      .get('/api/pubScreen/GetPubmedQueue', {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }

  // Adding a new paper from queue to system
  public addQueuePaper(pubmedID: number, doi: string) {

    // Sends an authenticated request.
    return this.http.get('/api/pubScreen/AddQueuePaper?pubmedID=' + pubmedID + '&doi=' + doi, {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }

  // Reject paper from pubscreen and remove from queue
  public rejectQueuePaper(pubmedID: number, doi: string): any {

    return this.http
      .delete('/api/pubScreen/RejectQueuePaper?pubmedID=' + pubmedID + '&doi=' + doi, {
        headers: this.authenticationService.getAuthorizationHeader()
      });

  }

  public getPubCount(): any {
    return this.http.get<{ pubCount: number; featureCount: number }>('/api/pubScreen/GetPubCount', {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });
  }

  public addCSVPapers(): any {
    return this.http
      .get('/api/pubScreen/AddCSVPapers', {
        headers: this.authenticationService.getAuthorizationHeader()
      });
  }

  getDataByLinkGuid(paperLinkGuid: any): any {
    return this.http
      .get('/api/pubScreen/GetDataByLinkGuid?paperLinkGuid=' + paperLinkGuid, {
        // headers: this.authenticationService.getAuthorizationHeader()
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }

  getGuidByDoi(doi: any): any {
    return this.http
      .get('/api/pubScreen/GetGuidByDoi?doi=' + doi, {
        // headers: this.authenticationService.getAuthorizationHeader()
        headers: new HttpHeaders().set('Content-Type', 'application/json')
      });
  }
}







