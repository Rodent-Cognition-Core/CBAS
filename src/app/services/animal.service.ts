import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators';
import { _throw } from 'rxjs/observable/throw';
import { Animal } from '../models/animal';

import { AuthenticationService } from './authentication.service';

@Injectable() export class AnimalService {

    

    constructor(
        private http: HttpClient,
        private authenticationService: AuthenticationService) { }


    public getAnimalInfo(id: any): any {

        return this.http
            .get("/api/animal/GetAnimalInfoByExpID?expId="+id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    public getCountOfAnimals(): any {

        return this.http
            .get("/api/animal/GetCountOfAnimals", {
                headers: new HttpHeaders().set('Content-Type', 'application/json')
            });
    }

    // Function defintion to add/create the animal
    public createAnimal(animal: Animal): any {

        const body: string = JSON.stringify(animal);
        return this.http.post("/api/animal/CreateAnimal", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });

    }

    // definiton to Edit/update the animal
    public updateAniaml(animal: Animal): any {

        const body: string = JSON.stringify(animal);
        return this.http.post("/api/animal/UpdateAnimal", body, {
            headers: this.authenticationService.getAuthorizationHeader()
        });
    }

    // Function definition to delete the animal
    public deleteAnimalbyID(id: any): any {

        return this.http
            .delete("/api/animal/DeleteAnimalById?animalID=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Function definition to get list of all strains
    public getAllStraine(): any {

        return this.http
            .get("/api/animal/GetAllStrain", {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }


    //Function definition to get list of all Genotypes
    public getAllGeno(id: any): any {

        return this.http
            .get("/api/animal/GetAllGenoByStrainID?ID=" + id, {
                headers: this.authenticationService.getAuthorizationHeader()
            });
    }

    // Check If edited UserAnimalID exist in Table Animal
    public IsUserAnimalIDExist(id: any, expID: any): any {

        return this.http
            .get("/api/animal/UserAnimalIDExist?UserAnimalID=" + id + "&ExpID=" + expID, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }

    public EditUserAnimalID(editedUserAnimalId: any, animalId: any, expId: any): any {

        return this.http
            .get("/api/animal/EditUserAnimalID?EditedUserAnimalId=" + editedUserAnimalId + "&OldAnimalId=" + animalId + "&ExpId=" + expId, {
                headers: this.authenticationService.getAuthorizationHeader()
            });

    }
    


    


}
