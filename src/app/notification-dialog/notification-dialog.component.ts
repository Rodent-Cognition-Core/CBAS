import { Component, Input } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';

@Component({
    selector: 'message-dialog',
    templateUrl: './notification-dialog.component.html',
})
export class NotificationDialogComponent {
    constructor(public dialogRef: MatDialogRef<NotificationDialogComponent>) { }

    public message: string;
}
