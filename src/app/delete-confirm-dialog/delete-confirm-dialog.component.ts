import { Component, Input } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'confirm-dialog',
    templateUrl: './delete-confirm-dialog.component.html',
})
export class DeleteConfirmDialogComponent {
    constructor(public dialogRef: MatDialogRef<DeleteConfirmDialogComponent>) { }

    public confirmMessage: string;
}
