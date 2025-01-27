import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject ,  Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Animal } from '../models/animal';

import { AuthenticationService } from './authentication.service';

export interface strainResponse {
  id: number;
  strain: string;
  link: string;
}

export interface genoResponse {
  id: number;
  genotype: string;
  link: string;
  description: string;
}

export interface animalResponse {
  id: number;
  expid: number;
  gid: number;
  sid: number;
  userid: string;
  sex: string;
  genotype: string;
  strain: string;
}
@Injectable() export class AnimalService {



  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService) { }


  public getAnimalInfo(id: any): Observable<animalResponse[]> {

    return this.http
      .get<animalResponse[]>('/api/animal/GetAnimalInfoByExpID?expId=' + id, {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }

  public getCountOfAnimals(): Observable<number> {

    return this.http
      .get<number>('/api/animal/GetCountOfAnimals', {
      headers: new HttpHeaders().set('Content-Type', 'application/json')
    });
  }

  // Function defintion to add/create the animal
  public createAnimal(animal: Animal): Observable<any> {

    const body: string = JSON.stringify(animal);
    return this.http.put<any>('/api/animal/CreateAnimal', body, {
      headers: this.authenticationService.getAuthorizationHeader()
    });

  }

  // definiton to Edit/update the animal
  public updateAniaml(animal: Animal): Observable<any> {

    const body: string = JSON.stringify(animal);
    return this.http.post<any>('/api/animal/UpdateAnimal', body, {
      headers: {
        ...this.authenticationService.getAuthorizationHeader(),
        'Content-Type': 'application/json'

      }
    });
  }

  // Function definition to delete the animal
  public deleteAnimalbyID(id: any): Observable<any> {

    return this.http
      .delete<any>('/api/animal/DeleteAnimalById?animalID=' + id, {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }

  // Function definition to get list of all strains
  public getAllStraine(): Observable<strainResponse[]> {

    return this.http
      .get<strainResponse[]>('/api/animal/GetAllStrain', {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }


  // Function definition to get list of all Genotypes
  public getAllGeno(id: any): Observable<genoResponse[]> {

    return this.http
      .get<genoResponse[]>('/api/animal/GetAllGenoByStrainID?ID=' + id, {
      headers: this.authenticationService.getAuthorizationHeader()
    });
  }

  // Check If edited UserAnimalID exist in Table Animal
  public IsUserAnimalIDExist(id: any, expID: any): Observable<boolean> {

    return this.http
      .put<boolean>('/api/animal/UserAnimalIDExist?UserAnimalID=' + id + '&ExpID=' + expID, {
      headers: this.authenticationService.getAuthorizationHeader()
    });

  }

  public EditUserAnimalID(editedUserAnimalId: any, animalId: any, expId: any): Observable<string> {

    return this.http
      .get<string>('/api/animal/EditUserAnimalID?EditedUserAnimalId=' + editedUserAnimalId + '&OldAnimalId=' + animalId + '&ExpId=' + expId, {
      headers: this.authenticationService.getAuthorizationHeader()
    });

  }






}
