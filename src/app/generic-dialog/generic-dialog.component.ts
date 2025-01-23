import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'app-generic-dialog',
    templateUrl: './generic-dialog.component.html',
    styleUrls: ['./generic-dialog.component.scss']
})
export class GenericDialogComponent implements OnInit {

    public title: string;
    public message: string;

    constructor(public dialogRef: MatDialogRef<GenericDialogComponent>) {
        this.title = '';
        this.message = '';

    }


    ngOnInit() {
    }

}
