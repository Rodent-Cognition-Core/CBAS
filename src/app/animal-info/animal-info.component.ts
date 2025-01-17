import { Component, OnInit, /*NgModule*/ } from '@angular/core';
import { AnimalService } from '../services/animal.service';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthenticationService } from '../services/authentication.service';
import { AnimalDialogComponent } from '../animal-dialog/animal-dialog.component';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { Location } from '@angular/common';
import { PagerService } from '../services/pager.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { CONFIRMDELETE } from '../shared/messages';
import { map } from 'rxjs/operators'
//import { OAuthService } from 'angular-oauth2-oidc';
//mport { SharedModule } from '../shared/shared.module';


@Component({
    selector: 'app-animal-info',
    templateUrl: './animal-info.component.html',
    styleUrls: ['./animal-info.component.scss'],

})
export class AnimalInfoComponent implements OnInit {

    // selectedExpValueAnimal: string;
    experimentName: string;
    experimentID: any;
    AnimalList: any;
    pager: any = {};
    expfilter: any = '';
    pagedItems: any[];
    dialogRefDelAnimal: MatDialogRef<DeleteConfirmDialogComponent>

    constructor(private animalService: AnimalService,
        private authenticationService: AuthenticationService,
        public dialog: MatDialog,
        private location: Location,
        private pagerService: PagerService,
        private spinnerService: NgxSpinnerService,
    ) {
        this.experimentName = '';
        this.pagedItems = [];
    }

    ngOnInit() {
        // this.GetAnimalInfo();

    }

    GetAnimalInfo(selectedExperimentID: number) {
        this.animalService.getAnimalInfo(selectedExperimentID).subscribe(data => {
            this.AnimalList = data;
            this.setPage(1);
            //console.log(this.AnimalList)

        });

    }

    SelectedExpChanged(experiment: any) {
        this.GetAnimalInfo(experiment.expID);
        this.experimentName = experiment.expName;
        this.experimentID = experiment.expID;

    }

    openDialog(Animal: any): void {
        //console.log(Animal);
        let dialogref = this.dialog.open(AnimalDialogComponent, {
            height: '480px',
            width: '450px',
            data: { experimentId: this.experimentID, animalObj: Animal }

        });

        dialogref.afterClosed().subscribe(result => {
            this.GetAnimalInfo(this.experimentID);
        });
    }

    // Delete Animal Dialog
    openConfirmationDialogDelAnimal(animalID: number, expId: number) {
        this.dialogRefDelAnimal = this.dialog.open(DeleteConfirmDialogComponent, {
            disableClose: false
        });
        this.dialogRefDelAnimal.componentInstance.confirmMessage = CONFIRMDELETE;



        this.dialogRefDelAnimal.afterClosed().subscribe(result => {
            if (result) {
                this.spinnerService.show();
                this.animalService.deleteAnimalbyID(animalID).pipe(map((res) => {


                    this.GetAnimalInfo(expId);
                    // location.reload()

                    this.spinnerService.hide();

                })).subscribe();
            }
            //this.dialogRefDelAnimal = null;
            this.dialogRefDelAnimal.close();
        });
    }

    // Delete animal 
    delAnimal(animalID: number, expId: number) {
        this.openConfirmationDialogDelAnimal(animalID, expId);
    }

    // Function definition for adding apagination to tbl Animal
    setPage(page: number) {


        var filteredItems = this.AnimalList;

        filteredItems = this.filterByString(this.AnimalList, this.expfilter);

        // get pager object from service
        this.pager = this.pagerService.getPager(filteredItems.length, page, 10);

        if (page < 1 || page > this.pager.totalPages) {
            this.pagedItems = [];
            return;
        }

        // get current page of items
        this.pagedItems = filteredItems.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }

    search(): any {
        this.setPage(1);
    }

    filterByString(data: any, s: string): any {
        s = s.trim();
        return data.filter((e : any) => e.userAnimalID.includes(s)); // || e.another.includes(s)
        //    .sort((a, b) => a.userFileName.includes(s) && !b.userFileName.includes(s) ? -1 : b.userFileName.includes(s) && !a.userFileName.includes(s) ? 1 : 0);
    }

}
