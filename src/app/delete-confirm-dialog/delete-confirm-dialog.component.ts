import { Component, Input } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'confirm-dialog',
    templateUrl: './delete-confirm-dialog.component.html',
})
export class DeleteConfirmDialogComponent {

    public confirmMessage: string;
    constructor(public dialogRef: MatDialogRef<DeleteConfirmDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any) {
        this.confirmMessage = '';
    }
}
