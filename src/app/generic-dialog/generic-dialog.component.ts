import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';

@Component({
    selector: 'app-generic-dialog',
    templateUrl: './generic-dialog.component.html',
    styleUrls: ['./generic-dialog.component.scss']
})
export class GenericDialogComponent implements OnInit {

    constructor(public dialogRef: MatDialogRef<GenericDialogComponent>) { }

    public title: string;
    public message: string;

    ngOnInit() {
    }

}
